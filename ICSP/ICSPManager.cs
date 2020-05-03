using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using ICSP.Client;
using ICSP.Logging;
using ICSP.Manager;
using ICSP.Manager.ConfigurationManager;
using ICSP.Manager.ConnectionManager;
using ICSP.Manager.DeviceManager;
using ICSP.Manager.DiagnosticManager;
using ICSP.Reflection;

namespace ICSP
{
  public class ICSPManager
  {
    private readonly Dictionary<ushort, Type> mMessages;

    private readonly Dictionary<ushort, DeviceInfoData> mDevices;

    public event EventHandler<ClientOnlineOfflineEventArgs> ClientOnlineStatusChanged;

    public event EventHandler<DynamicDeviceCreatedEventArgs> DynamicDeviceCreated;

    public event EventHandler<ICSPMsgDataEventArgs> DataReceived;
    public event EventHandler<ICSPMsgDataEventArgs> CommandNotImplemented;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    public event EventHandler<BlinkEventArgs> BlinkMessage;
    public event EventHandler<PingEventArgs> PingEvent;

    public event EventHandler<DeviceInfoEventArgs> DeviceInfo;
    public event EventHandler<PortCountEventArgs> PortCount;

    public event EventHandler<ChannelEventArgs> ChannelEvent;
    public event EventHandler<StringEventArgs> StringEvent;
    public event EventHandler<CommandEventArgs> CommandEvent;
    public event EventHandler<LevelEventArgs> LevelEvent;

    public event EventHandler<EventArgs> RequestDevicesOnlineEOT;
    public event EventHandler<ProgramInfoEventArgs> ProgramInfo;

    private ICSPClient mClient;

    private int GetPanelHierarchyCount; // Debug ...

    private readonly StateManager mStateManager;

    public ICSPManager()
    {
      mMessages = new Dictionary<ushort, Type>();

      mDevices = new Dictionary<ushort, DeviceInfoData>();

      mStateManager = new StateManager(this);

      var lTypes = TypeHelper.GetSublassesOfType(typeof(ICSPMsg));

      foreach(var type in lTypes)
      {
        if(type.IsAssignableFrom(typeof(ICSPMsg)))
          throw new ArgumentException("MessageType is not assignable from ICSPMsg", nameof(type));

        var lAttributes = AttributeHelper.GetList<MsgCmdAttribute>(type);

        foreach(var attribute in lAttributes)
        {
          if(!mMessages.ContainsKey(attribute.MsgCmd))
            mMessages.Add(attribute.MsgCmd, type);
        }
      }
    }

    public void Connect(string host)
    {
      Connect(host, ICSPClient.DefaultPort);
    }

    public void Connect(string host, int port)
    {
      if(string.IsNullOrWhiteSpace(host))
        throw new ArgumentNullException(nameof(host));

      Host = host;

      Port = port;

      if(mClient != null)
      {
        mClient.ClientOnlineStatusChanged -= OnClientOnlineStatusChanged;
        mClient.DataReceived -= OnDataReceived;
        mClient.Dispose();
      }

      mClient = new ICSPClient();

      mClient.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
      mClient.DataReceived += OnDataReceived;

      mClient.Connect(Host, Port);
    }

    public void Disconnect()
    {
      if(mClient != null)
        mClient.Disconnect();
    }

    private void OnClientOnlineStatusChanged(object sender, ClientOnlineOfflineEventArgs e)
    {
      if(e.ClientOnline)
      {
        var lRequest = MsgCmdDynamicDeviceAddressRequest.CreateRequest(mClient.LocalIpAddress);

        Send(lRequest);
      }
      else
      {
        CurrentSystem = 0;

        DynamicDevice = AmxDevice.Empty;

        mDevices.Clear();
      }

      ClientOnlineStatusChanged?.Invoke(this, e);
    }

    private void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
      try
      {
        Logger.LogDebug("{0} Bytes", e.Bytes.Length);
        Logger.LogDebug("Data 0x: {0}", BitConverter.ToString(e.Bytes).Replace("-", " "));

        var lMsgData = GetMsgData(e.Bytes);

        var lLastId = lMsgData.LastOrDefault().ID;

        foreach(var lData in lMsgData)
        {
          var lDataEventArgs = new ICSPMsgDataEventArgs(lData);

          DataReceived?.Invoke(this, lDataEventArgs);

          // No action needed
          if(lDataEventArgs.Handled)
            return;

          Type lMsgType = null;

          if(mMessages.ContainsKey(lData.Command))
            lMsgType = mMessages[lData.Command];

          if(lMsgType == null)
          {
            Logger.LogDebug(false, "-----------------------------------------------------------");

            Logger.LogWarn("Command: 0x{0:X4} ({1}) => Command not implemented", lData.Command, ICSPMsg.GetFrindlyName(lData.Command));

            CommandNotImplemented?.Invoke(this, lDataEventArgs);

            continue;
          }

          if(!(TypeHelper.CreateInstance(lMsgType, lData) is ICSPMsg lMsg))
            return;

          // Speed up
          if(Logger.LogLevel <= Serilog.Events.LogEventLevel.Debug)
            lMsg.WriteLog(lData.ID == lLastId);

          switch(lMsg)
          {
            case MsgCmdFileTransfer m:
            {
              /*
              

              -                               : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
              ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
              [AMX -> Panel] File Transfer    : 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff b5 | 02 04 | 00 00 00 01 | 00 10 | bb
              ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

              - FileType: Unused
              - Function: Create Remote Panel Directories
              ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
              [AMX -> Panel] File Transfer    : 02 | 01 1b | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 02 32 | 02 04 | 00 00 01 05 | 41 4d 58 50 61 6e 65 6c ... 09 (AMXPanel)
              [Panel <- AMX] Ack              : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 32 | 00 01 | 27
              [Panel <- AMX] FileTransfer (OK): 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 00 ab | 02 04 | 00 00 00 01 | 00 10 | b2
              -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
              [AMX -> Panel] File Transfer    : 02 | 01 1b | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 02 33 | 02 04 | 00 00 01 05 | 41 4d 58 50 61 6e 65 6c 2f 69 6d 61 67 65 73  ... af (AMXPanel/images)
              [Panel <- AMX] Ack              : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 33 | 00 01 | 28
              [Panel <- AMX] FileTransfer (OK): 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 00 ac | 02 04 | 00 00 00 01 | 00 10 | b3
              -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
              ...
              - FileType: Axcess2 Tokens
              - Function: Get Panel Manifest
              - Probably ...: Request : 0x0104 -> Create Access Token
              - Probably ...: Response: 0x0105 -> Valid Access Token
              - (0xD0, 0x07, AMXPanel/manifest.xma)
                                                02 | 01 1d | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 4b 5f | 02 04 | 00 04 01 04 | d0 07 | 41 4d 58 50 61 6e 65 6c 2f 6d 61 6e 69 66 65 73 74 2e 78 6d 61 ... 55
                                                02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 4b 5f | 00 01 | 9d
                                                02 | 00 1f | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 40 23 | 02 04 | 00 04 01 05 | 00 00 1f fa 00 00 c3 50 | 95
                                         Emtpy: 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff bc | 02 04 | 00 04 00 01 | 00 05 | bb


              - Probably ...: 0x0106 Get | Access Token
                                                02 | 00 1b | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 4b 60 | 02 04 | 00 04 01 06 | 00 00 c3 50 | c1
                                                02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 4b 60 | 00 01 | 9e
                                                7563636573732e786d6c0d0a343632347c33303830393634367c3539383636323738347c50505f4578706f72744661696c65642e786d6c0d0a333936387c33303830393634367c3539383636323738347c50505f496e666f496e636f6d706c6574652e786d6c0d0a333834307c33303830393634387c313932383732383139327c50505f4d69634c6f636b65642e786d6c0d0a38393530347c33303830393634337c333035333536343637327c50505f4f766572766965772e786d6c0d0a33343136307c33303830393634347c313238383539373337367c50505f526f6f6d53656c6563742e786d6c0d0a3231383632347c33303830393635307c333630383739333630307c50505f53657475702e786d6c0d0a333734347c33303830393634317c333030333439393236347c50505f53797374656d53687574646f776e2e786d6c0d0a31363033327c33303830393634317c333030333439393236347c50505f557362496e666f2e786d6c0d0a34363235367c33303830393634317c333030333439393236347c5f6b6579626f6172642e786d6c0d0a353439367c33303239383237307c313530343632323038307c616d7869636f6e332e7474660d0a3632323339367c33303038373339377c3734333233313438387c617269616c2d626f6c646174632e7474660d0a313033363538347c33303732373639347c313638353835343731397c617269616c2e7474660d0a3938303735367c33303732373639347c313638363031303938317c617269616c62642e7474660d0a32333237353831327c32393532373833337c313338333235303433327c617269616c756e692e7474660d0a313631333636387c33303732373639347c313638363332333530367c63616c69627269622e7474660d0a36333938307c33303031323832377c333136343439343230387c6c746535303837342e7474660d0a3136333434387c33303238313935337c313831353939303931327c726f626f746f2d626f6c642e7474660d0a3836373938387c33303732373639347c313737353038303539347c74611f
                                                (uccess.xml -> PP_CopySuccess)
              
              - Probably ...: List: [Size|PrevSegment|Nextegment|FileName]\0d\0a
              - Probably ...: List: [Size|Created|Modified|FileName]\0d\0a
              -- 689|30809200|1464076800|loading_1.png
              -- 
              -- 689|30 80 92 00|14 64 07 68 00|loading_1.png
              --     689 Bytes (689 Bytes)
              -- Created : Montag, ‎27. ‎April ‎2020, ‏‎10:42:20    
              -- Modified: ‎Montag, ‎27. ‎April ‎2020, ‏‎10:43:29
              -- CRC-32: a9882561

uccess.xml  
4624|30809646|598662784|PP_ExportFailed.xml
3968|30809646|598662784|PP_InfoIncomplete.xml
3840|30809648|1928728192|PP_MicLocked.xml
89504|30809643|3053564672|PP_Overview.xml
34160|30809644|1288597376|PP_RoomSelect.xml
218624|30809650|3608793600|PP_Setup.xml
3744|30809641|3003499264|PP_SystemShutdown.xml
16032|30809641|3003499264|PP_UsbInfo.xml
46256|30809641|3003499264|_keyboard.xml
5496|30298270|1504622080|amxicon3.ttf
622396|30087397|743231488|arial-boldatc.ttf
1036584|30727694|1685854719|arial.ttf
980756|30727694|1686010981|arialbd.ttf
23275812|29527833|1383250432|arialuni.ttf
1613668|30727694|1686323506|calibrib.ttf
63980|30012827|3164494208|lte50874.ttf
163448|30281953|1815990912|roboto-bold.ttf
867988|30727694|1775080594|ta.

-                               : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                  02 | 00 17 | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 4b 61 | 02 04 | 00 04 00 02 | a6

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
63|607293952|button-power-cm.png
6468|30212591|2527576064|button-rectangle-bm.png
2971|30212591|2607576064|button-rectangle-cm.png
3440|30212591|2627576064|button-rectangle-fb-bm.png
3012|30212591|2577576064|button-rectangle-fb-cm.png
10976|30320060|247242240|button-rectlong-bm.png
884|30320060|247242240|button-rectlong-cm.png
2318|30320060|247242240|button-rectlong-fb-bm.png
960|30320060|247242240|button-rectlong-fb-cm.png
1680|30378311|637282944|Button_CB_70x800.png
3918|30439475|1177590400|CamDom-46.png
4209|30439474|3922557696|CamFix-46.png
4818|30411094|2884418176|chat_check_44.png
7822|30411094|1384418176|chat_check_64.png
4864|30411094|3164418176|chat_delete_44.png
7693|30411094|1224418176|chat_delete_64.png
2089|30749463|3125437952|Co)
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
4406272|graphic-panel-left-calendar.png
2269|30402665|1233756160|Green-32x32.png
1513|30191115|325224960|icon-grab.png
1212|30181853|3502320512|icon-keyboard-caps.png
1397|30179837|716389248|icon-keypad-delete.png
1372|30200963|1077293952|icon-power-off.png
2327|30200963|1217293952|icon-power-on.png
2158|30211613|3765591552|icon-zoom-in.png
2239|30211613|3765591552|icon-zoom-out.png
947|30518713|1708989952|Line_White_H.png
951|30518713|2358989952|Line_White_V.png
689|30809200|1464076800|loading_1.png
1071|30809200|1464076800|loading_2.png
680|30809200|1464076800|loading_3.png
1065|30809200|1464076800|loading_4.png
684|30809200|1464076800|loading_5.png
1060|30809200|1464076800|loading_6.png
688|30809200|1464076800|loading_7.png
1058|30809200O
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ral_check_mark_48.png
1154|30412521|3446086784|realvista_general_lock_24.png
4637|30056053|838157312|realvista_general_pause_48.png
2757|30056053|858157312|realvista_general_play_32.png
4698|30056053|858157312|realvista_general_play_48.png
4694|30056044|3512862976|realvista_general_webcam_48.png
3838|30056044|3912862976|realvista_videoproduction_film_camera_16mm_48.png
5094|30056053|1198157312|realvista_webdesign_3d_design_48.png
25293|30056045|577895680|supervista_networking_subnet_128.png
74392|30056045|557895680|supervista_networking_subnet_256.png
20160|30056045|597895680|supervista_networking_video_conference_128.png
56783|30056045|577895680|supervista_networking_video_conference_256.png
3648|30056053|1318157312|USB-48.png
15250|30056044|40R
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

              02 | 00 d3 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 40 28 | 02 04 | 00 04 00 03 | 00 ba 31 32 38 36 32 39 37 36 7c 55 53 42 2e 70 6e 67 0d 0a 32 38
0030   34 33 7c 33 30 33 30 35 31 31 37 7c 32 31 31 33
0040   35 34 36 33 36 38 7c 57 61 70 70 65 6e 2d 4b 74
0050   2d 53 74 2d 47 61 6c 6c 65 6e 2d 68 30 30 34 30
0060   2d 62 30 30 33 33 2e 70 6e 67 0d 0a 32 31 36 38
0070   7c 33 30 33 32 31 30 32 31 7c 34 37 33 36 37 30
0080   37 38 34 7c 57 61 70 70 65 6e 2d 4b 74 2d 53 74
0090   2d 47 61 6c 6c 65 6e 2d 68 30 30 34 30 2d 62 5f
00a0   4c 48 2e 70 6e 67 0d 0a 31 30 38 33 34 7c 33 30
00b0   32 30 34 31 32 36 7c 32 32 39 35 37 33 36 37 30
00c0   34 7c 57 6f 72 6b 50 61 67 65 47 72 6f 75 6e 64
00d0   2e 70 6e 67 0d 0a 60



              [AMX -> Panel] File Transfer    : 02 | 01 1d | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 02 38 | 02 04 | 00 04 01 04 | d0 07 | 41 4d 58 50 61 6e 65 6c 2f 6d 61 6e 69 66 65 73 74 2e 78 6d 61 ... e5 (0xD0, 0x07, AMXPanel/manifest.xma)
              [Panel <- AMX] Ack              : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 38 | 00 01 | 2d
              [Panel <- AMX] FileTransfer (OK): 02 | 00 1f | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 00 b1 | 02 04 | 00 04 01 05 | 00 00 1f fa 00 00 c3 50 | e3
                                                                                                                                                D0 07 | 41 4D 58 50 61 6E 65 6C 2F 6D 61 6E 69 66 65 73 74 2E 78 6D 61 
                                                                                                                                  00 04 01 05 | 00 00 1F FA 00 00 C3 50 | 31

              - FileType: Axcess2 Tokens
              - Function: Get Panel Manifest - No More Data (00 00 C3 50)
              [AMX -> Panel] File Transfer    : 02 | 00 1b | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 02 39 | 02 04 | 00 04 01 06 |             00 00 c3 50 | 51
              [Panel <- AMX] Ack              : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 39 | 00 01 | 2e
                                                                                                                                                            00 00 C3 50
              */

              var lResponse = MsgCmdAck.CreateRequest(m.Source, m.Dest, m.ID);

              Send(lResponse);

              // Result-Code
              // 0x0010 => ??? GetNext, OK
              // 0x000A => Error: Device returned file transfer NAK code: 0x000a
              // 0x0010 => Get Panel Manifest -> Error: Device returned file transfer NAK code: 0x000a
              // 0x0100 = Get Panel Hierarchy

              switch(m.FileType)
              {
                case FileType.Unused:
                {
                  if(m.Function == FileTransferFunction.CreateRemotePanelDirectories)
                  {
                    GetPanelHierarchyCount = 0;

                    // OK
                    lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, FileTransferFunction.Ack, 0x0010);

                    Send(lResponse);
                  }

                  if(m.Function == FileTransferFunction.GetPanelHierarchy)
                  {
                    /*
                    AMXPanel/
                    // --  P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data              AMXPanel/                      | CS
                    // ->  02 | 01 1d | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 4f 8e | 02 04 | 00 00 01 00 | 00 00 41 4d 58 50 61 6e 65 6c 2f ... | de
                                                                                                                                                                           /.amx/AMXPanel
                    // <-  02 | 00 36 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff bd | 02 04 | 00 00 01 01 | 00 00 00 00 00 00 10 00 00 33 8d 83 00 30 7b 66 2f 2e 61 6d 78 2f 41 4d 58 50 61 6e 65 6c 00 | dd
                    
                                                                                                                                                                                    AMXPanel/images
                    // <-  02 | 00 3a | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff be | 02 04 | 00 00 01 02 | 00 00 00 01 00 03 00 01 00 00 20 00 05 03 07 e4 12 09 16 41 4d 58 50 61 6e 65 6c 2f 69 6d 61 67 65 73 00 | 9b
                                                                                                                                                                                    AMXPanel/sounds
                    // <-  02 | 00 3a | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff bf | 02 04 | 00 00 01 02 | 00 00 00 01 00 03 00 02 00 00 10 00 0a 07 07 e3 0d 20 17 41 4d 58 50 61 6e 65 6c 2f 73 6f 75 6e 64 73 00 | ce
                                                                                                                                                                                    AMXPanel/fonts
                    // <-  02 | 00 39 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff c0 | 02 04 | 00 00 01 02 | 00 00 00 01 00 03 00 03 00 00 10 00 05 03 07 e4 12 09 16 41 4d 58 50 61 6e 65 6c 2f 66 6f 6e 74 73 00 | 42
                    */
                    Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, (FileTransferFunction)0x0101, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x33, 0x8d, 0x83, 0x00, 0x30, 0x7b, 0x66, 0x2f, 0x2e, 0x61, 0x6d, 0x78, 0x2f, 0x41, 0x4d, 0x58, 0x50, 0x61, 0x6e, 0x65, 0x6c, 0x00 })); // /.amx / AMXPanel
                    /*
                    Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, (FileTransferFunction)0x0102, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x00, 0x01, 0x00, 0x00, 0x20, 0x00, 0x05, 0x03, 0x07, 0xe4, 0x12, 0x09, 0x16, 0x41, 0x4d, 0x58, 0x50, 0x61, 0x6e, 0x65, 0x6c, 0x2f, 0x69, 0x6d, 0x61, 0x67, 0x65, 0x73, 0x00 })); // AMXPanel/images
                    Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, (FileTransferFunction)0x0102, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x00, 0x02, 0x00, 0x00, 0x10, 0x00, 0x0a, 0x07, 0x07, 0xe3, 0x0d, 0x20, 0x17, 0x41, 0x4d, 0x58, 0x50, 0x61, 0x6e, 0x65, 0x6c, 0x2f, 0x73, 0x6f, 0x75, 0x6e, 0x64, 0x73, 0x00 })); // AMXPanel/sounds
                    Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, (FileTransferFunction)0x0102, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x00, 0x03, 0x00, 0x00, 0x10, 0x00, 0x05, 0x03, 0x07, 0xe4, 0x12, 0x09, 0x16, 0x41, 0x4d, 0x58, 0x50, 0x61, 0x6e, 0x65, 0x6c, 0x2f, 0x66, 0x6f, 0x6e, 0x74, 0x73, 0x00 })); // AMXPanel/fonts
                    */

                    // AMXPanel/images
                    // <- 02 00 3d 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff c1 02 04 | 00 00 01 01 | 00 00 00 00 00 00 10 00 00 33 8d 83 00 30 7b 66 2f 2e 61 6d 78 2f 41 4d 58 50 61 6e 65 6c 2f 69 6d 61 67 65 73 00 8d
                    // <- 02 00 2c 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff c2 02 04 | 00 00 01 02 | 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3f 00 0c

                    Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, (FileTransferFunction)0x0102, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3f, 0x00, })); // End
                  }

                  break;
                }
                case FileType.Axcess2Tokens:
                {
                  // FileData: 00 00 1f fa 00 00 c3 50 e3
                  // AMXPanel/manifest.xma
                  if(m.Function == FileTransferFunction.GetPanelManifest)
                  {
                    // Probably...: Request : 0x0104 -> Create Access Token
                    // Probably...: Response: 0x0105 -> Valid Access Token
                    lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, (FileTransferFunction)0x0105, new byte[] { 0x00, 0x00, 0x1f, 0xfa, 0x00, 0x00, 0xc3, 0x50, });

                    // Error: Device returned file transfer NAK code: 0x000a
                    Send(lResponse);
                  }

                  if(m.Function == FileTransferFunction.GetPanelManifestUnknown) // 0x0106
                  {
                    // Return empty (00 01 | 00 05)
                    lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, (FileTransferFunction)0x0001, new byte[] { 0x00, 0x05 });

                    Send(lResponse);
                  }

                  // ???
                  // File Transfer ...
                  if(m.Function == (FileTransferFunction)0x0006) // 0x0006
                  {
                    // Ack only ...
                    // --  P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD  | N-Data     | CS
                    // ->  02 00 17 02 10 00 06 27 11 00 00 00 06 7d 03 00 00 ff 4f 93 02 04              | 00 04 01 00 | db
                    // <-  02 00 1f 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff c7 02 04              | 00 04 01 01 | 7f ff ff ff 00 00 00 00 | 44
                  }

                  if(m.Function == (FileTransferFunction)0x0100) // 0x0100
                  {
                    // ???
                    // <-  02 00 1f 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff c7 02 04              | 00 04 01 01 | 7f ff ff ff 00 00 00 00 | 44
                    // <-  02 00 1f 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff c7 02 04              | 00 04 01 01 | 7f ff ff ff 00 00 00 00 | 44

                    // Function: -> 0x0100 FileName Request ...
                    // Function: <- 0x0101 FileName Response ...- (Ready)
                    Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, (FileTransferFunction)0x0101, new byte[] { 0x7f, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, }));
                  }
                  
                  if(m.Function == (FileTransferFunction)0x0102) // 0x0102
                  {
                    // ???
                    // Function: -> 0x0102 FileName Info Request ...
                    // Function: <- 0x0103 FileName Info Response ...- (success/Token)                                                            AMXPanel/fonts.xma
                    // ->  02 01 23 02 00 00 06 27 13 00 00 00 06 7D 03 00 00 0F FF 95 02 04              | 00 04 01 02 | 00 00 01 37 00 00 4E 20 41 4D 58 50 61 6E 65 6C 2F 66 6F 6E 74 73 2E 78 6D 61 ... E7
                    // <-  02 00 1d 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff c8 02 04              | 00 04 01 03 | 07 d0 00 00 c3 50 | b3

                    Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, (FileTransferFunction)0x0103, new byte[] { 0x07, 0xd0, 0x00, 0x00, 0xc3, 0x50, }));
                  }

                  // File Data (Raw?) ...
                  if(m.Function == (FileTransferFunction)0x0003) // 0x0003
                  {
                    // ???
                    // Function: -> 0x0003 Request ...
                    // Function: <- 0x0002 Response ... (Next data)
                    // ->  02 01 50 02 10 00 06 27 11 00 00 00 06 7d 03 00 00 ff 4f 95 02 04              | 00 04 00 03 | 01 37 1f ... | 9c
                    // <-  02 00 17 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff c9 02 04              | 00 04 00 02 | c2

                    // Compressed Archiv (GZip / 1F 8B)
                    // 01 37 
                    // 1F 8B 08 00 00 00 00 00 00 0B 8D 94 4D 4F 84 30 10 86 EF 9B EC 7F 68 F6 EE F2 51 57 30 A9 6C 90 8B 26 BB 1E 5C 34 7B 2D 50 B4 49 E9 24 50 08 FE 7B 2B 68 DC 43 4B F7 D2 B4 F3 4E DE A7 93 99 96 EC C7 46 A0 81 B5 1D 07 F9 B0 09 B6 FE 66 9F AC 57 A4 05 50 09 A9 41 AA 03 EF 7E 77 7A E5 82 25 B4 19 79 09 12 6F 95 AA 89 37 85 48 DD 0B F1 42 1B 96 A4 C7 33 7A D6 2A C2 5A FA 0B 92 BE A3 1F 2C 83 5E 7B C4 C4 BB 38 E9 A4 D9 F8 C2 BE E5 54 DC 14 20 2A AA 4A 33 E3 27 03 3D EA 0C 94 E6 99 85 83 31 BE 8A 64 27 58 8D 83 AB 8C 8B CA 71 79 8B 7F 10 C4 77 57 01 7A C9 17 08 6F 52 37 A9 62 E8 78 B2 70 42 7C EF C2 94 54 F0 A2 E5 85 11 93 CD E2 52 29 3B 17 40 28 B6 F3 E3 E8 D6 08 78 62 62 60 8A 97 14 1D 72 14 45 73 C3 33 90 15 93 1D B3 21 43 DF C5 6C A1 00 05 D3 80 19 B1 AF 93 BE 58 96 93 A1 E8 27 34 D4 32 01 F9 24 2E 01 42 97 FF C8 07 8A BA AF 46 17 61 44 9C F9 7B 8A 4E 93 6E 9B 32 73 0D DE FF 8B F7 A6 2F 60 BD FA 06 D0 9F 81 86 22 04 00 00

                    /*
                    <?xml version="1.0"?>
                    <root>
                      <fontList>
                        <font>
                          <file>amxicon3.ttf</file>
                          <fullName>AMX Icon 3</fullName>
                          <usageCount>8</usageCount>
                        </font>
                        <font>
                          <file>arial-boldatc.ttf</file>
                          <fullName>Arial Bold ATC</fullName>
                          <usageCount>333</usageCount>
                        </font>
                        <font>
                          <file>arial.ttf</file>
                          <fullName>Arial</fullName>
                          <usageCount>331</usageCount>
                        </font>
                        <font>
                          <file>arialbd.ttf</file>
                          <fullName>Arial Bold</fullName>
                          <usageCount>1186</usageCount>
                        </font>
                        <font>
                          <file>arialuni.ttf</file>
                          <fullName>Arial Unicode MS</fullName>
                          <usageCount>239</usageCount>
                        </font>
                        <font>
                          <file>calibrib.ttf</file>
                          <fullName>Calibri Bold</fullName>
                          <usageCount>5</usageCount>
                        </font>
                        <font>
                          <file>lte50874.ttf</file>
                          <fullName>Helvetica LT 77 Bold Condensed</fullName>
                          <usageCount>20</usageCount>
                        </font>
                        <font>
                          <file>roboto-bold.ttf</file>
                          <fullName>Roboto Bold</fullName>
                          <usageCount>50</usageCount>
                        </font>
                        <font>
                          <file>tahomabd.ttf</file>
                          <fullName>Tahoma Bold</fullName>
                          <usageCount>2</usageCount>
                        </font>
                        <font>
                          <file>xiva symbol.ttf</file>
                          <fullName>XiVA Symbol</fullName>
                          <usageCount>10</usageCount>
                        </font>
                      </fontList>
                    </root>

    
                    */

                    // fonts.xml
                    // bc 06 cb 95 a4 45 73 29 11 b8 4a 0a 80 b2 06 fc 20 d2 22 92 9e f3 66 ff 99 8c 5b a6 c7 da 65 94 f5 97 7a f7 01 75 7b aa 79 2c d8 97 ab 19 67 3c bd 82 e4 8e 14 82 7d 15 30 99 d3 46 68 93 8e c6 79 ba 94 c9 16 aa 28 ee 91 ec bd df c6 4b 31 20 2a 2a 0d d1 07 e6 1e 85 52 ad df d2 1c dc 4f 72 4c 28 e7 16 c9 09 75 f4 d3 7c b1 6f 24 2c 77 0d c0 35 cb 38 92 ff 21 71 b4 a9 c1 12 98 ee 6f f5 f6 ac 65 4d 8d 70 68 7c 82 76 22 e7 15 d8 fe a4 39 f5 d8 86 db f0 d1 dc ad b2 e8 83 0e c1 b0 19 e5 91 cf a8 14 25 3a fd 9f da d9 7c c7 44 28 18 93 d6 a7 cf 58 a0 c6 12 34 de 90 e1 cf 43 a8 b5 0a 08 48 24 ea 0e f3 b9 5f c9 48 2d 19 45 fe 22 a1 62 1a d0 f8 34 8a c5 81 01 58 43 05 a9 3d 21 36 e3 d8 e4 b7 bf 58 cb 46 80 d0 36 d8 59 a7 ea 03 db b5 b2 23 df 46 9e fe e5 83 e2 8e e1 05 cb b4 26 38 90 a7 e0 c1 72 ce 52 f3 cb 09 be b6 06 4b fc 74 31 46 4d 2e 61 d5 e4 27 99 25 77 fd df 3c 79 18 08 fb 6e 21 e4 03 8b e7 a1 05 78 f5 c6 c2 07 c0 37 e6 44 9c 92 d2 47 32 e8 d2 5c 76 c4 8e 37 a4 36 e9 0c 0c 1a d2 1b 84 db 9e c9 92 52 92 fb f9 e9 e6 46 59 e1 55 39 73 0f 0e 04 94 c5 06 ab 8f 65 4a 4d 75 8a 89 a3 89 da 6d 72 0c 28 db f2 79 2b d5 a9 87 0a b3 99 61 60 c5 ee 5f 96 d8 93 72 00 7b c1 b4 82 43 24 9c dc ec c7 e2 90 8e 8d 21 fd b0 ec de 07 83 6a 9a 65 12 50 5e c9 fc c5 95 8b e6 0c a8 9c c2 60 f7 45 4c e7 67 06 4c b7 1c 17 f1 40 76 45 3b 8c ff 2b 2f 2e a9 aa 97 7b bd 56 c7 bc 66 43 c0 0d 4e 4c 10 bb 3d d9 12 65 d1 b5 68 38 07 4e cb f3 0b d5 17 55 11 41 e0 35 cf 41 ed 19 91 d0 6f 31 8d fd b4 50 9c 2e 95 da 21 65 99 d8 fa c7 d3 8a 0d b8 42 af d6 77 ab dc 2b 70 72 21 e6 1e 23 32 6e c1 7e e0 d9 4a 21 6a 0c bd fd df 26 43 9d 97 8f 44 94 55 1d e2 ae a4 32 83 d9 be 06 a8 f8 95 42 01 8f c9 61 e4 2a a0 79 73 31 7e 1b 4f 30 e4 69 ef d3 1f 8c 8d 89 a5 0b 3d 9e 90 52 12 69 95 56 39 1b d7 0a cc 19 ac a3 bf 4e e9 29 1a 41 2e cb 23 c1 d4 59 5f b7 5e 11 5d 37 a3 9a db 00 f7 a1 2b b6 61 c9 50 8e f8 3e 33 8e b5 9d 27 c4 bc f4 40 29 79 a0 86 64 f7 26 b7 0e 1f 7e b8 c5 47 4c 34 89 98 5d a1 56 14 49 2d 92 1e 37 37 f5 a8 e5 49 0f d8 bc 77 97 4d 71 7c c0 f5 55 46 7f a7 18 92 33 3d c3 31 f3 9f 63 0d 1d 8f bd 50 cf 2a 6f d3 d2 bf a3 0b 40 b5 2c c8 e4 be 04 d7 d6 df 1c 33 85 3e 30 cb d8 a0 82 0e 61 b1 92 10 a8 6e 36 05 84 1b df 8f d7 7d 4a 9c 55 bf 67 a3 92 a7 c6 79 32 d7 b2 03 5d ad ab 7c 49 1e 02 10 28 ad 50 93 2e c8 c2 f8 82 af 4c 42 05 a4 9e 08 9b f8 fa f0 6a 1a 85 52 f6 92 e0 86 c8 fd e0 3f 1d bc 36 45 be 45 43 b5 8b 49 f6 a2 55 5d 97 29 1f 58 24 e7 eb b9 ae 59 d7 cd 79 ea 59 f1 8e 15 dd 39 b2 9d d7 19 6c 9e ef e7 ed 26 43 e8 99 47 bb df cd c3 4f f5 cf 30 1f aa 26 ad 7d ad dc 89 79 9a 17 8e 87 c1 86 63 bc 9b 00 ff f2 6b b9 0a 51 89 b9 22 4e 9b 28 1a 6d 58 21 f2 cf 37 61 0f 66 38 d6 cb 92 ea 2c 0c 76 26 67 fc f1 52 99 e5 69 74 62 ac 3b cb e3 c0 4a 93 c4 fc 58 8d 79 84 67 41 8d 9a 47 96 9e f7 51 21 ff b2 c3 b1 7e 7a dc b8 e8 14 6e b7 ee 9b 2d c7 62 36 17 ba 4d a9 62 c9 8f 7f 2c 1d db cc ca 55 0b 18 11 86 91 e5 66 9a a4 70 2d 03 37 bb 7c 92 92 44 0d f1 23 16 d4 d9 32 13 b6 08 2b 3d 6f c2 fb ae b8 85 35 e0 ec 17 38 30 e7 9e 16 41 d9 5c 1e 3b 69 ec fc 06 dc 8b 05 44 e0 05 a0 12 2a 6e 01 1e 1a 2d 6b 99 a0 af 55 b5 51 a4 b7 c7 a0 75 3b e9 a4 ee 7a 79 d6 2a 39 fd 16 dd 4a d3 07 e7 c0 39 c6 27 c6 12 69 cd e6 10 c5 2c a6 6a



                    Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, (FileTransferFunction)0x0102, new byte[] { }));
                  }


                  break;
                }
                default:
                {
                  lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, FileTransferFunction.Ack, 0x0010);

                  Send(lResponse);

                  break;
                }
              }

              break;
            }
            case MsgCmdBlinkMessage m:
            {
              BlinkMessage?.Invoke(this, new BlinkEventArgs(m));

              break;
            }
            case MsgCmdPingRequest m:
            {
              if(mDevices.ContainsKey(m.Device))
              {
                var lDeviceInfo = mDevices[m.Device];

                if(lDeviceInfo != null)
                {
                  var lResponse = MsgCmdPingResponse.CreateRequest(
                    lDeviceInfo.Device, lDeviceInfo.System, lDeviceInfo.ManufactureId, lDeviceInfo.DeviceId, mClient.LocalIpAddress);

                  Send(lResponse);
                }

                PingEvent?.Invoke(this, new PingEventArgs(m));
              }

              break;
            }
            case MsgCmdDynamicDeviceAddressResponse m:
            {
              CurrentSystem = m.System;

              DynamicDevice = new AmxDevice(m.Device, 1, m.System);

              var lDeviceInfo = new DeviceInfoData(m.Device, m.System, mClient.LocalIpAddress);

              if(!mDevices.ContainsKey(lDeviceInfo.Device))
                mDevices.Add(lDeviceInfo.Device, lDeviceInfo);

              var lRequest = MsgCmdDeviceInfo.CreateRequest(lDeviceInfo);

              Send(lRequest);

              DynamicDeviceCreated?.Invoke(this, new DynamicDeviceCreatedEventArgs(m));

              break;
            }
            case MsgCmdRequestEthernetIp m:
            {
              if(mDevices.ContainsKey(m.Dest.Device))
              {
                var lRequest = MsgCmdGetEthernetIpAddress.CreateRequest(m.Source, m.Dest, mClient.LocalIpAddress);

                Send(lRequest);
              }

              break;
            }
            case MsgCmdRequestDeviceInfo m:
            {
              if(m.Device == DynamicDevice.Device && m.System == DynamicDevice.System)
              {
                var lDeviceInfo = new DeviceInfoData(m.Device, m.System, mClient.LocalIpAddress);

                var lResponse = MsgCmdDeviceInfo.CreateRequest(lDeviceInfo);

                Send(lResponse);
              }

              break;
            }
            case MsgCmdRequestStatus m:
            {
              var lResponse = MsgCmdStatus.CreateRequest(lMsg.Source, lMsg.Dest, lMsg.Dest, StatusType.Normal, 1, "Normal");

              Send(lResponse);

              break;
            }
            case MsgCmdOutputChannelOn m:
            {
              ChannelEvent?.Invoke(this, new ChannelEventArgs(m));

              break;
            }
            case MsgCmdOutputChannelOff m:
            {
              ChannelEvent?.Invoke(this, new ChannelEventArgs(m));

              break;
            }
            case MsgCmdDeviceInfo m:
            {
              if(CurrentSystem > 0)
                DeviceInfo?.Invoke(this, new DeviceInfoEventArgs(m));

              break;
            }
            case MsgCmdPortCountBy m:
            {
              PortCount?.Invoke(this, new PortCountEventArgs(m));

              break;
            }
            case MsgCmdRequestDevicesOnlineEOT m:
            {
              RequestDevicesOnlineEOT?.Invoke(this, EventArgs.Empty);

              break;
            }
            case MsgCmdProbablyProgramInfo m:
            {
              ProgramInfo?.Invoke(this, new ProgramInfoEventArgs(m));

              break;
            }
            case MsgCmdStringMasterDev m:
            {
              StringEvent?.Invoke(this, new StringEventArgs(m));

              break;
            }
            case MsgCmdCommandMasterDev m:
            {
              CommandEvent?.Invoke(this, new CommandEventArgs(m));

              break;
            }
            case MsgCmdLevelValueMasterDev m:
            {
              LevelEvent?.Invoke(this, new LevelEventArgs(m));

              break;
            }
            default:
            {
              MessageReceived?.Invoke(this, new MessageReceivedEventArgs(lMsg));

              break;
            }
          }
        }
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }
    }

    public void SendString(AmxDevice device, string text)
    {
      var lRequest = MsgCmdStringMasterDev.CreateRequest(DynamicDevice, device, text);

      Send(lRequest);
    }

    public void SendCommand(AmxDevice device, string text)
    {
      var lRequest = MsgCmdCommandMasterDev.CreateRequest(DynamicDevice, device, text);

      Send(lRequest);
    }

    public void SetChannel(AmxDevice device, ushort channel, bool enabled)
    {
      if(enabled)
      {
        var lRequest = MsgCmdOutputChannelOn.CreateRequest(DynamicDevice, device, channel);

        Send(lRequest);
      }
      else
      {
        var lRequest = MsgCmdOutputChannelOff.CreateRequest(DynamicDevice, device, channel);

        Send(lRequest);
      }
    }

    public void SendLevel(AmxDevice device, ushort level, ushort value)
    {
      var lRequest = MsgCmdLevelValueMasterDev.CreateRequest(DynamicDevice, device, level, value);

      Send(lRequest);
    }

    public void CreateDeviceInfo(DeviceInfoData deviceInfo)
    {
      CreateDeviceInfo(deviceInfo, 1);
    }

    public void CreateDeviceInfo(DeviceInfoData deviceInfo, ushort portCount)
    {
      if(!mDevices.ContainsKey(deviceInfo.Device))
        mDevices.Add(deviceInfo.Device, deviceInfo);

      var lDeviceRequest = MsgCmdDeviceInfo.CreateRequest(deviceInfo);

      Send(lDeviceRequest);

      if(portCount > 1)
      {
        var lSource = DynamicDevice;

        // lSource = new AmxDevice(deviceInfo.Device, 0, deviceInfo.System);

        var lPortCountRequest = MsgCmdPortCountBy.CreateRequest(lSource, deviceInfo.Device, deviceInfo.System, portCount);

        Send(lPortCountRequest);
      }
    }

    public void RequestDevicesOnline()
    {
      var lRequest = MsgCmdRequestDevicesOnline.CreateRequest(DynamicDevice);

      Send(lRequest);
    }

    public void RequestDeviceStatus(AmxDevice device)
    {
      // System 0 does not works!
      if(device.System == 0)
        device = new AmxDevice(device.Device, device.Port, DynamicDevice.System);

      var lRequest = MsgCmdRequestDeviceStatus.CreateRequest(DynamicDevice, device);

      Send(lRequest);
    }

    public void Send(ICSPMsg request)
    {
      if(mClient?.Connected ?? false)
        mClient?.Send(request);
      else
      {
        Logger.LogDebug(false, "ICSPManager.Send[1]: MessageId=0x{0:X4}, Type={1}", request.ID, request.GetType().Name);
        Logger.LogDebug(false, "ICSPManager.Send[2]: Source={0}, Dest={1}", request.Source, request.Dest);
        Logger.LogError(false, "ICSPManager.Send[3]: Client is offline");
      }
    }

    private List<ICSPMsgData> GetMsgData(byte[] msg)
    {
      var lMsgBytes = msg;
      var lMessages = new List<ICSPMsgData>();
      var lOffset = 0;

      while(lMsgBytes.Length >= 3)
      {
        // +4 => Protocol (1), Length (2), Checksum (1)
        var lSize = ((lMsgBytes[1] << 8) | lMsgBytes[2]) + 4;

        lMsgBytes = new byte[lSize];
        Array.Copy(msg, lOffset, lMsgBytes, 0, lSize);

        var lMsg = ICSPMsgData.FromMessage(lMsgBytes);

        if(lMsg.Command != ICSPMsgData.Empty.Command)
          lMessages.Add(lMsg);

        lOffset += lSize;

        lMsgBytes = new byte[msg.Length - lOffset];
        Array.Copy(msg, lOffset, lMsgBytes, 0, lMsgBytes.Length);
      }

      return lMessages;
    }

    public ushort CurrentSystem { get; private set; }

    public AmxDevice DynamicDevice { get; private set; }

    public bool IsConnected
    {
      get
      {
        return mClient?.Connected ?? false;
      }
    }

    public IPAddress CurrentRemoteIpAddress
    {
      get
      {
        return mClient?.RemoteIpAddress;
      }
    }

    public IPAddress CurrentLocalIpAddress
    {
      get
      {
        return mClient?.LocalIpAddress;
      }
    }

    public string Host { get; private set; }

    public int Port { get; private set; }
  }
}