using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using ICSharpCode.SharpZipLib.GZip;

using ICSP.Extensions;
using ICSP.Logging;
using ICSP.Manager.ConfigurationManager;
using ICSP.Manager.DeviceManager;

namespace ICSP
{
  public class FileManager
  {
    private readonly ICSPManager mManager;

    public readonly byte[] AccessToken = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }; // 0x00, 0x00, 0xC3, 0x50

    private const ushort MagicGzipByte = 0x1F8B;

    private string mCurrentFileName;

    private List<byte> mRawData;

    public FileManager(ICSPManager manager)
    {
      mManager = manager ?? throw new ArgumentNullException(nameof(manager));

      BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    }

    public void ProcessMessage(MsgCmdFileTransfer m)
    {
      // Confirm
      var lResponse = MsgCmdAck.CreateRequest(m.Source, m.Dest, m.ID);

      mManager.Send(lResponse);

      int lOffset;
      /*
      -                       : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
      ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      [M->D] File Transfer    : 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff b5 | 02 04 | 00 00 00 01 | 00 10 | bb
      ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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

      [M->D] File Transfer    : 02 | 01 1d | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 02 38 | 02 04 | 00 04 01 04 | d0 07 | 41 4d 58 50 61 6e 65 6c 2f 6d 61 6e 69 66 65 73 74 2e 78 6d 61 ... e5 (0xD0, 0x07, AMXPanel/manifest.xma)
      [D->M] Ack              : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 38 | 00 01 | 2d
      [D->M] FileTransfer (OK): 02 | 00 1f | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 00 b1 | 02 04 | 00 04 01 05 | 00 00 1f fa 00 00 c3 50 | e3
                                                                                                                                        D0 07 | 41 4D 58 50 61 6E 65 6C 2F 6D 61 6E 69 66 65 73 74 2E 78 6D 61 
                                                                                                                          00 04 01 05 | 00 00 1F FA 00 00 C3 50 | 31

      - FileType: Axcess2 Tokens
      - Function: Get Panel Manifest - No More Data (00 00 C3 50)
      [M->D] File Transfer    : 02 | 00 1b | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 02 39 | 02 04 | 00 04 01 06 |             00 00 c3 50 | 51
      [D->M] Ack              : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 39 | 00 01 | 2e
                                                                                                                                                    00 00 C3 50
      */

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
            /*
            - FileType: Unused
            - Function: Create Remote Panel Directories
            -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
            --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            [M->D] File Transfer: 02 | 01 1b | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 02 32 | 02 04 | 00 00 01 05 | 41 4d 58 50 61 6e 65 6c ... 09 (AMXPanel)
            [D->M] Ack          : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 32 | 00 01 | 27
            [D->M] FileTransfer : 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 00 ab | 02 04 | 00 00 00 01 | 00 10 | b2
            -------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            [M->D] File Transfer: 02 | 01 1b | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 02 33 | 02 04 | 00 00 01 05 | 41 4d 58 50 61 6e 65 6c 2f 69 6d 61 67 65 73  ... af (AMXPanel/images)
            [D->M] Ack          : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 33 | 00 01 | 28
            [D->M] FileTransfer : 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 00 ac | 02 04 | 00 00 00 01 | 00 10 | b3
            -------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            ...
            */

            lOffset = 0;
            var lDirectory = AmxUtils.GetNullStr(m.FileData, ref lOffset);

            CreateResourceDirectory(lDirectory);

            // OK
            lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, 0x0001, new byte[] { 0x00, 0x10 });

            mManager.Send(lResponse);
          }

          if(m.Function == FileTransferFunction.GetPanelHierarchy) // 0x0100
          {
            lOffset = 2;

            var lPath = AmxUtils.GetNullStr(m.FileData, ref lOffset);

            Logger.LogDebug(false, "FileManager[GetPanelHierarchy]: Path={0}", lPath);

            ProcessDirectoryInfo(m, lPath);
          }

          break;
        }
        case FileType.Axcess2Tokens:
        {
          if(m.Function == FileTransferFunction.GetPanelManifest) // 0x0104
          {
            /*
            - FileType: Axcess2 Tokens
            - Function: Get Panel Manifest
            -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
            --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            [M->D] File Transfer: 02 | 01 1d | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 4b 5f | 02 04 | 00 04 01 04 | d0 07 | 41 4d 58 50 61 6e 65 6c 2f 6d 61 6e 69 66 65 73 74 2e 78 6d 61 ... 55  (0xD0, 0x07, AMXPanel/manifest.xma)
            [D->M] Ack          : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 4b 5f | 00 01 | 9d
            [D->M] FileTransfer : 02 | 00 1F | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | 00 0D | 02 04 | 00 04 01 05 | 00 00 1F FA FF FF FF FF | 1A
            -------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            - If Manifest not exists: (Emtpy)
            [D->M] FileTransfer : 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff bc | 02 04 | 00 04 00 01 | 00 05 | bb

            */

            lOffset = 2;
            var lManifest = AmxUtils.GetNullStr(m.FileData, ref lOffset);

            Logger.LogDebug(false, "FileManager[CreateAccessToken]: Manifest={0}, Token={1}", lManifest, BitConverter.ToString(AccessToken).Replace("-", " "));

            // Return empty (00 01 | 00 05)
            // -> GetPanelHierarchy 0x0100
            lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0001, new byte[] { 0x00, 0x05 });

            /*
            // Probably...: Request : 0x0104 -> Create Access Token
            // Probably...: Response: 0x0105 -> Valid Access Token
            // var lBytes = new byte[] { 0x00, 0x00, 0x1f, 0xfa, }.Concat(AccessToken).ToArray();
            var lBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, }.Concat(AccessToken).ToArray();

            // Function: -> 0x0104 Create Access Token
            // Function: <- 0x0105 Access Token
            lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0105, lBytes);
            */

            mManager.Send(lResponse);
          }

          if(m.Function == (FileTransferFunction)0x0106) // 0x0106
          {
            /*
            - FileType: Axcess2 Tokens
            - Function: Get Panel Manifest
            -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
            --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            [M->D] File Transfer: 02 | 00 1b | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 4b 60 | 02 04 | 00 04 01 06 | 00 00 c3 50 | c1
            [D->M] Ack          : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 4b 60 | 00 01 | 9e
            [D->M] FileTransfer : 
            -------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            - If Manifest not exists: (Emtpy)
            [D->M] FileTransfer : 02 | 00 19 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff bc | 02 04 | 00 04 00 01 | 00 05 | bb
            -------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            */

            var lToken = m.FileData.Range(0, 4);

            Logger.LogDebug(false, "FileManager[Token]: Token={0}", BitConverter.ToString(lToken).Replace("-", " "));

            // Return empty (00 01 | 00 05)
            lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0001, new byte[] { 0x00, 0x05 });

            mManager.Send(lResponse);
          }

          // File Transfer Start ... (176)
          if(m.Function == (FileTransferFunction)0x0006) // 0x0006
          {
            /*
            Ack only ...
            -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data              | CS
            [M->D] File Transfer: 02 | 00 19 | 02 00 | 00 06 27 13 00 00 | 00 06 7D 03 00 00 | 0F | FF 97 | 02 04 | 00 04 00 06 | 00 B0 | 48 (176)
            [D->M] Ack          : 02 | 00 13 | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | FF 97 | 00 01 | 73
            */

            Logger.LogDebug(false, "FileManager[0x0006]: Count={0}", ArrayExtensions.GetBigEndianInt16(m.FileData, 0));
          }

          // File Transfer Start Transfer  ...
          if(m.Function == (FileTransferFunction)0x0100) // 0x0100
          {
            /*
            -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data      | CS
            [M->D] File Transfer: 02 | 00 17 | 02 00 | 00 06 27 13 00 00 | 00 06 7D 03 00 00 | 0F | FF 98 | 02 04 | 00 04 01 00 | 92
            [D->M] Ack          : 02 | 00 13 | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | FF 98 | 00 01 | 74
            [D->M] FileTransfer : 02 | 00 1F | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | 00 1A | 02 04 | 00 04 01 01 | 7F FF FF FF 00 00 00 00 | 8A
            */

            Logger.LogDebug(false, "FileManager[0x0100]: ... (Start Transfer)");

            // Function: -> 0x0100 FileName Request ...
            // Function: <- 0x0101 FileName Response ...- (Ready)
            mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0101, new byte[] { 0x7f, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, }));
          }

          // File Transfer File Info ...
          if(m.Function == (FileTransferFunction)0x0102) // 0x0102
          {
            /*
            -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data                                                                                        | CS
            [M->D] File Transfer: 02 | 01 23 | 02 00 | 00 06 27 13 00 00 | 00 06 7D 03 00 00 | 0F | FF 99 | 02 04 | 00 04 01 02 | 00 00 01 37 00 00 4E 20 41 4D 58 50 61 6E 65 6C 2F 66 6F 6E 74 73 2E 78 6D 61 ... EB
            [D->M] Ack          : 02 | 00 13 | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | FF 99 | 00 01 | 75
            [D->M] FileTransfer : 02 | 00 1D | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | 00 1B | 02 04 | 00 04 01 03 | 07 D0 00 00 C3 50 | F9
            */

            // 00 00 01 37 00 00 4E 20 -> Timestamp/Hash ....

            lOffset = 8;
            mCurrentFileName = AmxUtils.GetNullStr(m.FileData, ref lOffset);

            Logger.LogDebug(false, "FileManager[0x0102]: FileName={0}", mCurrentFileName);

            mRawData = new List<byte>();

            // Function: -> 0x0102 FileName Info Request ... (AMXPanel/fonts.xma)
            // Function: <- 0x0103 FileName Info Response ...- (success/Token)
            mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0103, new byte[] { 0x07, 0xd0, 0x00, 0x00, 0xc3, 0x50, }));
          }

          // File Data (Raw?) ...
          if(m.Function == (FileTransferFunction)0x0003) // 0x0003
          {
            Logger.LogDebug(false, "FileManager[0x0003]: FileName={0}, RawData ...", mCurrentFileName);

            // ???
            // Function: -> 0x0003 Request ...
            // Function: <- 0x0002 Response ... (Next data)
            // ->  02 01 50 02 10 00 06 27 11 00 00 00 06 7d 03 00 00 ff 4f 95 02 04              | 00 04 00 03 | 01 37 1f ... | 9c
            // <-  02 00 17 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff c9 02 04              | 00 04 00 02 | c2

            // Length: 07 D0 (2000)
            // 07 D0 1F 8B 08 00 00 00 00 00 00 0B ED 9C 6D 6F DB 38 12 80 3F 6F 80 FC 07 9E BE 5C 17 48 E4 C4 69 BA 6D 91 64 91 D7 6E 80 A4 E7 73 D2 A6 38 DC 17 5A A2 6D 5E 24 52 25 29 BF F4 D7 EF 0C 29 59 B2 E2 DA B1 E2 05 7A 5B 6D 81 56 26 67 86 D4 F0 21 39 A4 C8 3D FA 7D 12 47 64 C4 94 E6 52 1C 7B FB FE 9E 47 98 08 64 C8 C5 E0 D8 4B 4D 7F F7 AD F7 FB C9 F6 D6 D1 3F 76 77 B7 B7 CE 65 32 55 7C 30 34 A4 BD B7 D7 DE 6D EF ED 1F EC 90 D3 DB 2F E4 E6 E6 DC 27 A7 51 44 6C AE 26 8A 69 A6 46 2C F4 B7 B7 AE 85 61 22 64 21 E9 4B 45 52 CD 88 14 D1 94 8C B9 19 5A CD 90 69 3E 10 C4 48 19 69 42 45 48 12 25 C3 34 30 1A 54 1F A8 12 50 8F F7 E4 7E C8 35 09 64 9C A4 86 29 94 18 28 1A 13 48 83 47 C3 02 03 D6 7B 53 10 C8 6B 17 D1 B1 B5 C5 A1 6C 25 A8 81 77 A3 11 31 8A C1 23 43 CB 9F 04 4D CD 50 2A FE 0D 54 A9 31 2C 4E A0 D6 46 42 C5 D1 17 0C 7C 30 E0 82 41 61 66 C8 F2 02 77 08 BC 01 15 53 92 48 85 26 89 EC 13 6E 6C AA 62 49 AA 12 A9 D9 F6 16 0A 04 12 5F DA 90 98 4E D1 15 69 64 A0 2A 44 A3 6D 46 02 3E E2 91 AD 5E A0 78 CC B1 66 09 83 BF B1 6A 3B 36 7D CC C1 93 3D 5B AE 66 01 BC 73 B8 BD 05 75 C3 AA F4 53 C8 62 13 6B 1D 8A C7 24 78 59 7F 77 17 DB 88 26 89 7E 7F 67 64 F0 D8 95 32 26 D0 B2 42 BF 9F 68 7E EC 0D 8D 49 DE B7 5A E3 F1 D8 1F 1F F8 52 0D 5A D0 80 FB AD 2F B7 37 77 C1 90 C5 74 97 0B 6D A8 08 98 B7 BD 45 2A FF 39 33 68 7B CE 0E 8D 27 3E 34 49 EB C3 E1 29 64 2D D2 D3 FC BD B6 D6 6F 64 60 DB 60 89 3E 71 FF F8 13 1D 2E 30 55 E6 D3 7F EB 9D A0 C4 11 CA 9F 38 59 7C 26 82 C6 EC D8 3B 0D 82 F4 01 DA 79 08 6D F7 EF 94 07 8F 1E 49 68 F0 48 07 EC A3 CD 87 22 7D 0A 32 63 27 E3 83 BF 95 E4 A1 47 E0 05 AF 2F 16 E9 9F E4 F5 39 BA 16 7D 79 72 C1 75 12 D1 A9 26 99 05 E4 9A 05 54 1B 9D 21 07 BF 63 FB BA FE 51 CB 6A CC F4 1F B8 08 25 90 A9 13 60 F6 8A 4F 58 78 EC F5 69 A4 59 51 06 BE 8B CD EE A2 09 7D 52 F6 45 39 07 90 51 01 40 70 EC ED F9 6F 3C A2 30 ED 81 87 66 08 3E CA 7F FF C1 B0 33 1C 7B EF 3C D2 2A 17 D0 FA 4E 09 65 FB 37 40 26 BC 11 E0 99 59 3D DC 6F 7B F8 33 37 7A B0 B7 E7 15 56 8F 5A EE DD 4A BE 8A C1 E5 73 D6 EF 87 69 DC B3 C9 27 DC 66 FE B7 E4 6B 9B E9 27 62 70 D4 2A C9 95 B4 9D FD EF A9 BB 5C A7 5F 96 2C EA 37 57 1F 70 41 92 2C 60 A7 F3 99 74 2E AE C8 67 CE C6 D0 B0 1D 25 AB F0 4C FC 21 8B A2 1C 1A 3F 09 FB 23 90 4D 50 30 E3 A7 D0 AF 82 83 39 7D 1E 31 32 B2 D9 0D 1D AB E8 E8 7C 06 97 39 5F 42 53 D4 22 64 DE C4 66 28 39 53 72 0C B3 DB A2 71 25 C3 A2 97 4B 64 4C E4 1A 15 20 1E 58 8F 64 A2 3F 35 0C 1D 0A 93 2B 83 99 7A 56 C2 2F BF 14 89 99 D7 3F 75 AF 3D 3B FB 95 DC 0D 33 C8 D8 35 A9 66 E6 82 1A FA 49 71 08 5F F0 E9 7E 9A 80 D4 9D 51 10 3F 78 64 44 A3 14 7E 7A 76 74 3E F6 AE 05 37 1C A6 DD 4F DD 9B 2C AF CB BE A6 5C 15 4E 9E F7 42 B5 2E 5D 06 93 A5 32 04 98 58 52 27 E5 A4 BA A9 C0 20 C6 CA 96 AA 76 06 01 0F A3 62 56 37 A3 52 96 D7 2F B7 4F 05 12 44 38 86 19 FF 84 E9 25 82 00 26 9C 12 09 E1 82 FF A4 E2 D6 40 D9 CD 4F DD FA BC 7E 97 D1 5A A7 BF 65 AA 9B E9 67 E7 34 0A D2 88 1A B9 AC AB 05 33 A1 F6 AC BB 95 14 2B 3D AE 19 72 97 37 7D E1 B9 3A AD 5F 68 6F 0C 00 58 39 D0 15 CD 9F 89 14 6D EF 12 2A 2D 7F 0F 81 72 9E 67 3B 55 98 87 71 10 94 0B 70 6C 5F 41 C4 CC 68 30 C4 B0 7A 2A 53 45 3E 48 39 80 D9 1A 82 45 99 82 44 33 5F AF 86 C7 BA B7 26 3A 56 77 43 E0 C0 F2 8B C2 12 72 19 38 33 91 1C 9C 3C A1 02 4E 9E 5E 00 43 49 C4 B5 5D 7D E5 46 1C 3C 0D 34 75 A0 C9 5C 58 0B 9A 4C 77 43 D0 A4 4A 31 11 4C 09 98 85 F5 A6 59 18 E1 49 E0 87 FA 41 26 1A 14 92 39 45 4F 8D 34 53 D0 7A 40 64 1E 9C 39 B0 16 19 55 23 9B 41 E4 42 C9 A4 27 27 0B B8 08 5D CE 93 DD 84 5C A3 02 C1 DD 90 2A 66 17 83 9A 98 A1 92 E9 60 68 B7 72 50 FC 4C 4E 48 10 C9 34 24 B8 85 C6 03 D6 0C 22 AB 98 C9 BC 5C 87 94 4C 75 33 7C 5C C6 94 47 4B 26 1D E6 F2 33 36 9C 74 95 0C 98 06 5B 5D 16 30 3E 62 C4 CA BB AD D2 CE BF 3A 07 3B E4 FA F6 B4 63 77 99 2E 27 C1 90 8A 41 33 D1 3C 9B 11 EB ED 3A 84 58 C5 CD F0 71 05 1D 7E C9 EE 01 2C 1D 71 48 A8 EE 1E 94 B5 2A B4 9C 06 30 3C E8 6C 20 D1 10 78 B3 90 48 41 3E DD 9D 91 50 01 41 0D 16 AB B0 28 39 B7 0E 1C 25 F5 CD 20 F2 81 46 11 53 D3 25 83 C8 C0 49 1C 14 93 4C AE 53 81 03 B7 BD 88 AD 67 06 C8 AB 5E 9C EC 10 A8 E0 0E 31 BC DF DF 21 FF 4B E0 71 C0 FB BF 36 98 AC C0 24 73 71 1D 44 32 D5 FA 78 E0 A7 37 E2 9E 0A 4E 6E A6 22 58 14 87 D0 98 3E 52 3F 0D 02 FF 0B 48 9C CB 28 A2 3D FF CC 4F F9 8C 16 AB 59 41 E5 12 3F 28 85 DA 7D EC 92 B8 F7 0D AB 9B 5B 1E 28 A9 65 DF 10 54 21 D9 07 28 F7 CD 0E E3 92 1D A2 D3 C4 7E 0A 13 03 E2 BE 1F 19 12 C3 78 44 07 1C 19 1B F1 90 49 5C 23 F5 19 06 63 28 65 3F 6E 0D B9 61 3D 49 15 7E 62 24 AF B2 0D 2C 5C 56 55 0A E4 A2 AF A8 36 2A 0D 4C AA 58 C3 E8 2A 46 BF D0 D1 F4 9E F6 EA 30 9A A9 BE 60 08 B3 1F 20 2B 63 D9 2D 57 38 F8 DC B2 90 53 5C 10 19 25 B1 2B 2C 1A DB 52 23 85 8C 79 60 D7 E5 20 A6 FD 20 E2 D0 20 33 6A 9D AD 45 8B 73 90 76 4B EF AC B8 D3 34 84 06 BD 9B 6A C3 E2 86 99 15 CC 38 9F D5 41 C6 69 6E 66 D2 EB 44 54 DC D2 47 18 76 6E 65 0F A6 AA 0A 21 38 24 C4 98 ED 03 0C 11 77 DF 92 B5 9F 80 96 4D 2E 22 EA 49 C0 A2 EF 7C 7D 2B 06 17 27 15 CA 20 8D F1 2B 7A F3 35 EE 59 A4 CC DA C8 35 51 1D 64 2A 26 36 C4 0E 1E 35 11 C6 31 B1 1E 3F 65 CD E2 FB 2D 4E 7F 1D C9 81 8C 55 20 95 44 1B 9A D6 A4 A9 EC FB 17 10 F5 D4 CC 66 A8 B2 27 3F 2E F8 A2 10 6B 98 0E 86 B0 B0 CA C3 F1 90 07 F6 78 11 C6 EC F9 A6 4F 91 54 5D AF 91 42 1C A2 29 6A F2 10 0A FC 9D 46 86 27 11 1E E7 11 83 14 6B D8 40 B4 02 A2 BC 91 EA 90 93 EB 6E 0A 17 69 D8 A2 F3 22 C0 0B 86 B7 8F 10 D6 E0 29 30 88 78 7D 03 05 97 E6 AC 42 F5 7B E7 8C 34 1E E8 22 7A 1A F7 F2 23 72 2E E5 2B 6A 36 94 AC A6 04 DC 54 F3 FC 48 AE BB 89 C5 5B 99 97 3B 9E 64 FB C4 73 B8 48 35 F0 75 96 85 0F 29 9D 61 02 1A 17 56 A3 BA C8 97 D7 1D 92 6A 5C 4A E1 EA 0C 8F 06 DC 41 CA A8 ED EF D9 C3 91 32 90 51 83 C8 0A 44 F2 E6 A8 83 48 AE BB E1 B5 D3 DD E3 34 61 0B 46 13 8D E9 BE A2 B0 B6 2E C6 10 27 5C 21 E3 BA BA 22 B7 43 C7 D3 45 79 43 C7 2A 3A D0 BB B5 D0 40 C5 CD 4C 30 F7 6C 62 D6 5F 21 19 D0 9A 5F 21 3D 48 15 AE 8C 6B AD 50 13 D1 AE 47 C9 AC 85 EA 47 B3 15 13 75 C9 C9 F2 4B F4 7C FE 78 4E CE B3 FD 94 85 9F 1D 46 22 A8 EC B7 94 54 AA 31 6C 14 C9 B1 AE EC 05 E2 EE 20 0C 2A 02 5C 8E 8F 94 28 16 63 5C 13 32

            // Length: 02 E0 (736)
            // 02 E0 FD 68 64 E2 62 5D AE 49 8F E1 40 A4 F1 F3 27 8E 45 94 60 39 77 78 55 A0 21 6C 15 61 E0 2A D7 22 75 D8 9A 29 AF 49 D5 C2 E3 99 8B CE 44 42 9C 21 01 DC 90 45 CB 4E 60 9E B7 5F F7 B8 C9 CF 39 9E A3 0E 89 65 98 E2 A9 68 94 D0 BE 37 DF 88 4E FF B3 CD 3B 99 BF 18 50 CE 3B 71 86 8F 5A E5 B4 A5 E2 87 6F 9E 2F FC E6 F5 F3 65 DF 3E 5F 74 0D AB ED 79 D1 B2 E0 9C BB 5A 73 FE AA 8A 94 CE 84 2E 6B CA BE 84 CE 72 95 46 D1 D2 03 AB D9 41 59 D7 92 A7 D1 18 97 2A 8A 7D 85 72 8D BD EA 63 6F AA E8 40 31 26 48 9A 84 D4 94 4F A7 2E 28 95 27 A7 61 08 EB 79 BD 8C 9F 7D DF FE C9 8B 75 23 07 81 80 97 E6 BA CB 4F C5 2E 28 37 A1 5A 8F 61 C6 7B CE C1 E1 D3 14 86 3D 61 B2 09 96 CC 54 97 17 00 EB FB 39 E3 78 23 6A 80 93 72 66 FD F0 1D 8C 0E F3 6F 84 3A 44 40 DF 9E 89 AD F5 4A 8A F5 2F B3 4B 5C CB 5E EB B7 BC D4 0E 28 30 85 63 B2 76 E5 E7 57 C0 EA F5 46 F2 C8 A6 C7 5E 97 8E C9 AC 16 27 7B CF 85 DD 29 E3 4D 33 D2 C5 E9 A4 30 B1 BF 9E 89 6E F7 B2 A4 DC 5E B7 FC 79 F5 D7 EB A9 FF 01 53 39 5E F5 28 0C 1C AE 67 E0 3F 11 EF 95 B4 DF AC A7 7D 6F 6F C1 15 EA BF AD 59 78 F7 A6 FC EE FB DF 2D 7D 33 E3 8D 0E 68 54 25 B5 DA 47 66 1D 04 83 57 DB 3B 32 2D 58 F2 BA 0D DC 17 90 DA E7 36 6A 71 83 D5 DA A8 4A C1 76 8D DC 85 7F D6 46 F4 9B 94 F1 52 34 37 E3 E0 54 33 BC 82 17 9D A7 4A E3 69 FA E7 8E EA 56 09 E6 67 BC B5 99 48 7B 99 92 BC D2 CC 3A 0B 47 21 BC 99 90 A7 43 70 C7 C5 88 6B DE 8B D8 AF DE DF F1 FA 44 79 69 06 7F DB 9F 2E 2F 0B B8 AE 50 61 26 5D 4A 23 CC 55 FA 2A 92 D4 40 ED F8 37 76 2B 47 50 5F 16 0E D8 1D FC 3A F6 20 5C EC 51 85 CF 04 A2 C5 77 A5 A0 FB 2C 35 06 F8 2E 3B D4 25 11 56 DC AB BC 61 7D 98 63 60 2A 62 0A 26 49 8F 68 08 F3 ED 0D DE 76 A5 5F 38 D5 F9 4B 52 49 02 75 BD A5 13 F4 9B 54 CC C5 86 65 C1 72 87 77 E9 6B D4 A6 5D AB 36 4C A4 2F AF C7 39 13 F6 70 EE AC FC C3 E7 96 DF 65 26 55 E2 E5 35 E8 E2 40 FC 32 57 5C 4E B8 D9 74 45 6A 11 E2 C0 5D AB 2A 79 42 D1 85 4A BD 62 55 47 B1 4B BE BF B2 9F FC 48 64 B6 7F 06 32 5F 8E C3 47 29 E2 86 88 86 88 0B 19 3C 62 CB FD 45 50 DC CB E4 65 8E D8 54 9B D4 A7 22 F7 D0 CB EB 70 26 21 25 FE 3F 02 C3 82 DC 90 B1 8C 0C EB A2 9F 10 0D 68 BE 1F 76 1E F9 31 C8 00 0F FD 7C 53 89 E3 B8 21 63 29 19 CE 49 7F 57 38 E6 52 50 EC A8 35 FF BF 53 82 A4 3F 01 58 47 D9 1D 2D 4B 00 00

            var lLenght = m.FileData.GetBigEndianInt16(0);

            mRawData.AddRange(m.FileData.Range(2, lLenght));

            // var lRawData = new byte[lLenght];

            // Function: -> 0x0003 Request ...
            // Function: <- 0x0002 Response ... (Next data)
            mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0002, new byte[] { }));
          }

          // File Data Raw End
          if(m.Function == (FileTransferFunction)0x0004) // 0x0004
          {
            // Function: -> 0x0004 Request ...
            // Function: <- 0x0005 Response ... (Next data)
            // -> 02 00 17 02 10 00 06 27 11 00 00 00 06 7d 03 00 00 ff 4f 96 02 04 | 00 04 00 04 | e1
            // <- 02 00 17 02 00 00 06 7d 03 00 00 00 06 27 11 00 00 0f ff ca 02 04 | 00 04 00 05 | c6

            // Array.Copy(m.FileData, 2, lRawData, 0, lLenght);

            // Compressed Archiv (GZip / 1F 8B)
            // 01 37 1F 8B 08 00 00 00 00 00 00 0B 8D 94 4D 4F 84 30 10 86 EF 9B EC 7F 68 F6 EE F2 51 57 30 A9 6C 90 8B 26 BB 1E 5C 34 7B 2D 50 B4 49 E9 24 50 08 FE 7B 2B 68 DC 43 4B F7 D2 B4 F3 4E DE A7 93 99 96 EC C7 46 A0 81 B5 1D 07 F9 B0 09 B6 FE 66 9F AC 57 A4 05 50 09 A9 41 AA 03 EF 7E 77 7A E5 82 25 B4 19 79 09 12 6F 95 AA 89 37 85 48 DD 0B F1 42 1B 96 A4 C7 33 7A D6 2A C2 5A FA 0B 92 BE A3 1F 2C 83 5E 7B C4 C4 BB 38 E9 A4 D9 F8 C2 BE E5 54 DC 14 20 2A AA 4A 33 E3 27 03 3D EA 0C 94 E6 99 85 83 31 BE 8A 64 27 58 8D 83 AB 8C 8B CA 71 79 8B 7F 10 C4 77 57 01 7A C9 17 08 6F 52 37 A9 62 E8 78 B2 70 42 7C EF C2 94 54 F0 A2 E5 85 11 93 CD E2 52 29 3B 17 40 28 B6 F3 E3 E8 D6 08 78 62 62 60 8A 97 14 1D 72 14 45 73 C3 33 90 15 93 1D B3 21 43 DF C5 6C A1 00 05 D3 80 19 B1 AF 93 BE 58 96 93 A1 E8 27 34 D4 32 01 F9 24 2E 01 42 97 FF C8 07 8A BA AF 46 17 61 44 9C F9 7B 8A 4E 93 6E 9B 32 73 0D DE FF 8B F7 A6 2F 60 BD FA 06 D0 9F 81 86 22 04 00 00

            // Compressed Archiv (GZip / 1F 8B)
            var lExt = Path.GetExtension(mCurrentFileName);

            if(lExt == ".xma" || lExt == ".xml")
            {
              var lRawData = mRawData.ToArray();

              // Search GZip MagicByte (1F 8B)
              var lMagicByte = lRawData.GetBigEndianInt16(0);

              //       | FS    |             | FileName
              // 00 00 | 07 A2 | 00 00 4E 20 | 41 4D 58 50 61 6E 65 6C 2F 70 72 6A 2E 78 6D 61 -- AMXPanel/prj.xma

              // AMXPanel/prj.xma
              // 07 A2 -> 1954
              // 07 A2 1F 8B 08 00 00 00 00 00 00 0B C5 5A 5B 6F DB 36 14 7E 1F D0 FF A0 E6 69 03 D6 48 F2 25 37 B0 2A 52 27 E9 B2 C6 8D 11 BB 69 D1 17 83 96 68 5B 8D 24 6A 14 E5 C4 FF 7E 87 A4 A8 1B E5 62 03 56 0D 28 10 9E EF 7C A4 3E 1E 1E 1E D2 52 D1 BB 97 38 B2 76 84 65 21 4D DE 1E B9 C7 CE D1 3B EF D5 2F 88 51 CA 3D 54 E0 B7 C9 9A 7A 68 4D 59 8C F9 A3 82 BC C1 39 B2 9B 08 DA 30 9C 6E 43 3F 2B 29 03 64 B7 31 B4 19 E3 34 2D 4D F7 0C 28 0D 04 AD C3 88 94 86 DD B0 02 92 85 9B A4 72 B6 ED 86 DC 94 D1 EF C4 E7 A5 C1 C1 10 B4 84 26 04 D9 35 00 A5 38 CB 9E 29 0B 2C 92 F8 6C 9F 72 12 40 20 8E 60 3C ED 10 94 84 44 8B 7D 4A BC AF 37 6F 5C C7 71 AE 84 57 63 52 F3 03 D9 85 52 C7 A3 33 56 BA 4B 04 E5 19 61 33 46 05 E8 05 64 8D F3 88 23 BB 0E C2 D4 70 44 D8 6D E0 5D 3E CE AD F9 3E E3 24 26 D6 E5 07 31 C7 C2 81 BE D3 D5 27 1C 13 EF 23 4E A9 35 BF B7 1E C3 80 50 12 26 30 EB 04 6F 63 98 94 66 A0 0C FA 64 F7 2C 20 0C 66 51 37 D2 9C F9 5B 9C 11 ED 6B D9 30 C0 84 C6 31 49 60 ED ED BA A1 22 2D 75 5C CD 74 E0 95 2C 9F 11 2C E2 78 85 39 F1 16 39 B1 EE 7D 6E B9 8E E5 BA 17 8E F8 67 0D 1C F7 14 D9 0D 1A 62 45 68 54 A7 6D 6E 7D A2 3B 6B E0 5A CE F9 C5 F0 FC 62 7C 2A 3A 41 82 35 68 28 C2 19 9F E3 1D 91 D6 94 26 D6 14 EF 2D 67 64 B9 27 17 A3 E1 C5 60 00 9D 06 0E B2 1B 34 B9 34 3F 0C DB EF 8F CE E9 EF 41 7A CC D3 62 D9 54 08 7D 9A EC EE E8 C6 83 01 75 13 B0 88 B2 C9 96 86 3E 10 EC 86 95 A5 C4 0F D7 FB 19 65 7C 42 73 08 19 F4 33 30 CD 9A 6C 71 D2 66 55 98 4C CE 2A 75 B3 3C 4D 61 84 1B 50 76 17 66 E0 8E 71 2A 0C 0F FE 1E BF C4 18 D9 1A 50 FA 64 33 C5 D1 D2 71 5C E5 AF 60 D8 C3 89 1C C9 13 8D 4C B9 4B 0C F1 2D 64 9D 6A DA B5 36 79 E1 22 56 D1 FB 9C 73 9A 14 FE 2E 10 F6 B1 6C 7C 18 5F C2 8E 56 A3 6B 0C 45 74 53 74 AD 5A C6 DC E4 9E 9A 13 9E A7 D0 2E E3 26 AA 44 5A 8B A2 F0 8B A8 CA F0 95 06 C2 41 C0 48 96 E9 3E 2E 78 1B 10 82 54 4F 60 7C 4D 38 87 0C 6B 40 28 22 3B DD 3E 19 82 D0 CA 04 35 CF 84 7D 4E 67 78 43 BC D9 87 E5 14 87 89 10 55 81 25 83 A6 A0 7E 36 5B 3A EE 72 CE 31 E3 15 4D 7A D0 9A 90 60 85 FD A7 F7 51 98 3C 3D 88 24 15 89 67 80 28 13 9D F3 74 CE 59 98 6C 44 B0 9A F6 33 7E 22 35 6F D3 CC 22 42 6A 3D EB 16 0C 93 04 AB 7D 7D D8 BA 9D 6D 73 1E D0 E7 A4 F2 B7 80 30 88 88 9A B0 5D 35 45 6B 11 C6 84 E6 72 51 EA A6 48 1F 95 24 D9 47 B2 57 99 53 B7 33 A8 0C 24 F9 12 06 7C EB B9 83 33 B1 A4 35 A4 70 FF 41 C2 CD 96 7B 67 4E E9 2E 90 C2 FF 40 D6 B0 CE 5B EF A4 F4 6B 44 13 28 17 41 AD DC CA 2E BC 57 04 FE 86 A9 3A 0E EC 0E 2C 85 59 2E 18 AC 8F 08 81 23 6A 7F CD 46 7E CE 32 CA 64 A9 50 2D B4 62 42 5D 02 A9 27 E6 84 EC 9A 8D 22 D1 9C 93 04 88 77 22 C1 64 F2 BA 90 6D 5D B8 C1 9E D0 40 4E A3 13 AF B3 27 2A B3 3B 46 AF 7B 3A 7A 74 3C A1 EE 41 31 15 31 E9 7C C8 21 57 57 1F FD 98 43 2E B4 C2 1C 4A CC BE 11 23 03 6B B0 F4 90 06 86 42 26 D8 97 D3 AF C3 B3 EB 38 54 39 DA 82 2A CE 68 3C 36 48 1A AB 8F F4 40 FC 5D 6B 24 09 35 46 32 48 1A 2B 58 9F E1 16 E0 56 04 65 D6 7C 83 A6 6F 20 0E 5C 0C DB AB 15 79 13 6C F2 74 68 4C 10 E5 49 F8 57 4E 6E AF BC 91 F5 C6 12 C7 EA 9F 79 62 B9 A7 96 3B BA 18 9F 5D 0C CF C4 59 0C C5 B0 A4 89 32 3F 11 A7 39 09 BC 0F A3 CF 3C 8C 42 BE 97 85 5E A3 68 95 87 51 F0 29 8F 57 70 A9 10 3B BA 6E 8B EE 53 1A 84 EB 10 98 8B D9 95 BC 4A 8C 65 FF 12 AE 0F 00 A0 E7 3A CD 41 04 A6 38 50 61 79 9E 09 7B 83 0B 4A 05 21 0C 37 BC 1D 94 A8 88 40 42 88 30 35 01 38 4A 19 CC 89 CC 53 A8 BE 32 7F EB 76 71 CE C0 4E CF 66 EA 40 AE CE 9B 3A 88 76 34 4C C5 4D 09 EA A8 5E 8E 36 54 DC 16 F5 C9 06 DD C5 79 67 71 B8 3D BE 3D 12 E6 91 42 AF 13 CE A0 26 26 E2 0A 52 9C 32 77 D4 7F 22 01 B2 25 26 49 B0 06 03 55 82 C4 6A C8 FB 63 83 0B 07 6F A4 AE 32 90 48 D9 23 8E C2 C0 7B 03 A2 74 5B F5 2D 1E 75 E0 A9 AD E7 0D BB 9F F7 AF 9E 64 EB 69 1B 01 C8 F2 D5 A1 18 CC 96 EA 32 3C 2F 4E A1 96 AE B1 63 44 A2 DD A3 AE 71 C3 28 C4 7F 9A 6D DE D3 17 F1 DB 44 58 DD C2 53 71 56 CB CB BD 2B CE 6F 6D FC 38 74 B3 E5 04 EE 52 8C 46 4B 58 08 1C 19 5A 87 A6 D6 46 07 53 EA 4F 11 E9 8C 96 0F 79 92 C0 D1 65 28 1C 99 0A 2B B6 29 EF 0B 65 4F EB 88 3E FF 1C 99 E3 E5 7B 38 61 F3 D4 50 39 EE 50 A9 C9 7D 8B 1C 2C C5 35 DD 90 78 D2 21 51 51 7B 16 38 81 BF 0C DF 84 2F 86 C4 D3 8E 6C D4 E4 3E 32 71 F9 44 F6 2B 8A 59 60 28 3B 6B 29 2B 99 3D 6D 90 69 E8 77 56 DD B1 73 6E C6 AC 24 F7 5A 68 44 26 DD 26 3E 8D 53 38 CA 48 5B A7 EB 9A 3A 9B 3D 7A 15 EB 0C 97 33 46 52 CC 4C A1 1D E5 BB 62 F7 BD 97 4F 96 D7 89 B1 E6 6E 47 D5 56 CC 9E E5 5D BF C8 DF CA 18 9E 67 8A EC A8 37 75 7E DF 87 60 A2 DE AD 1D 10 7B DA 79 0E 36 FA F4 2C 38 DD 5F 33 46 D9 C0 90 7A D6 25 55 B3 FF 1F 91 AE 21 F2 FC 07 22 DD DE 45 CE 73 DF 87 1F BA 6D 95 03 A7 5B 65 41 EF 55 E6 65 CE 29 23 2B 4A B9 A1 B2 A3 72 56 6C 53 A4 BC D1 FF 9C 50 46 04 27 97 D9 93 A1 B0 A3 64 6A 6E 5F 17 C8 F2 CD 56 4B 5A 57 A9 2C B8 3D 17 CB FB 1D 61 BB 90 3C 1B 12 3B 6E B8 9A DB EB EA CA 9B 96 A1 6E 7C E8 4E D6 C7 CA BE D6 3B AE 25 AA 7D B6 14 BC 7E 0B 8B C8 F0 2B E3 9E 3D 38 3D B0 19 AE 68 4F 7B E1 73 B6 EA FA 01 30 E8 38 38 0A 6A 7F 71 7B 5D 6D B1 A6 BA 61 7B A3 96 CC 3E B7 E9 EB 62 7B B5 B4 B5 F7 80 A2 F5 B9 39 BB 75 75 5C B1 7A 57 F6 40 69 3C 27 11 DC 94 0C 79 1D 3B A1 62 F7 B4 19 F4 DB 8C 07 12 53 F3 57 C9 68 7C F8 FD 87 EA F1 33 64 D6 5E 3B 31 92 D1 9C F9 F5 57 4F 61 AC 5E 3C 69 57 31 97 8F B2 E8 2E 67 8B 6F E5 24 18 E5 D4 A7 91 F7 C7 62 31 53 DF 8F A5 89 B6 14 86 76 9D E3 E1 C9 B1 3B 84 3F E3 0B F7 5C BC F3 94 B8 9A E7 4E 7C 72 3C 8E BF A7 9B 77 BE 1C 58 0C F9 76 4A 57 E0 B4 A0 BE EB 29 B3 E2 9B 85 F8 72 A5 DB A2 A5 A5 75 8B AC FF B6 FF CF 45 CE 17 97 5F 2E AD FB 88 93 C4 92 0F FA 67 4A ED 7A A8 E1 01 CF 37 04 F3 9C 69 53 19 65 03 12 62 25 BF 08 2D 68 EE 6F E7 7C 1F 11 F1 45 4C FB 50 9E C1 22 A9 EF 70 C3 D3 B1 F8 72 5E DA 25 AF 73 D4 4C 7C E4 99 E2 EC E9 56 2C F3 A1 31 C5 25 BF 7B 48 BB 25 3C 55 2F 8E 1B 46 B1 18 37 98 AD 32 7F 4B 62 6C 4D E8 0A 47 DC FA 75 4A 03 C2 E8 57 6B 26 5E FD 5A D7 77 DF 06 8E EB FE A6 D7 6A 6D 7C A7 55 A1 2D 86 05 8D F2 2A AA 8D B2 5D B5 94 0C 5B FE 57 8D 57 BF FC 0D 45 6A 7E 74 CA 21 00 00

              // ???
              // bc 06 cb 95 a4 45 73 29 11 b8 4a 0a 80 b2 06 fc

              #warning TODO: <password encrypted="1">
              /*
              Proj.xma
              <protection>none</protection>

              <password encrypted="1">
              </password>
              */

              //       | FS    |             | FileName
              // 00 00 | 01 30 | 00 00 4E 20 | 41 4D 58 50 61 6E 65 6C 2F 21 4D 73 67 42 6F 78 2E 78 6D 6C --> AMXPanel/!MsgBox.xml

              // AMXPanel/!MsgBox.xml
              // 01 30 -> 304
              // BC 06 CB 95 A4 45 73 29 11 B8 4A 0A 80 B2 06 FC 82 DA 4A 8D 8A 40 10 D8 55 25 ED EA 8B 26 C2 54 B4 AC 3C 62 CA AF 73 4A 9A E5 0E 88 D2 EB 10 D1 C3 93 DC E7 32 36 22 A3 7E 0C 15 5E FB E1 98 1C 56 AC FF 45 E3 5E A3 5A 88 14 06 DF BD 2C 78 42 03 E2 E8 A3 23 9C 5F 6A FA CA EB B7 B9 67 FB 0D 20 BF 20 A8 82 FB D1 5B 1A 91 42 A8 11 3B 5E 73 01 A1 D4 C8 7A A7 8E 0E 21 7B D0 DD BA 9B E6 59 0E 71 55 D7 03 D3 F3 B8 6A 75 F8 CC 8D D8 DC E4 5E 27 A5 E8 14 F6 2D 23 8F 78 B0 B6 3C FB 4D 62 84 D2 20 72 96 BD 8D DD 2B B2 35 FC 26 8E 9F C3 95 B6 94 76 2D 6D 72 89 5D E0 7A 6C D9 1E 96 D7 14 E7 67 C2 37 3A A6 CE 53 4E A0 D9 42 ED 8F F7 60 30 C6 1B F3 84 52 A9 7E 82 DB 0D 20 B9 73 C1 01 6A 75 24 2F 36 20 21 44 5B 69 84 FC BE 25 3D DD D0 3C 56 EF 47 3A B1 73 42 06 2D 17 89 87 C0 74 2A 79 8C C8 43 DC 37 44 EC C6 CD D5 BF C2 E0 85 82 2B 03 9C C9 01 4C 42 4C 54 DF 3C A2 04 37 83 3C 12 22 C2 75 F6 3B 8E 43 F2 DA AC FB 22 FB

              // .xma -> Archive (Gzip)
              if(lMagicByte == MagicGzipByte)
              {
                // Unzip
                var lMemStream = new MemoryStream();

                try
                {
                  GZip.Decompress(new MemoryStream(lRawData), lMemStream, false);

                  lRawData = lMemStream.ToArray();
                }
                catch(Exception ex)
                {
                  Logger.LogError(false, "FileManager[0x0003]: FileName={0}, GZipData-Decompress => {1}", mCurrentFileName, ex.Message);

                  Logger.LogError(false, "FileManager[0x0003]: FileName={0}, RawData={1}", mCurrentFileName, BitConverter.ToString(lRawData).Replace("-", " "));
                }
              }

              var lXml = Encoding.Default.GetString(lRawData);

              var lXmlDoc = new XmlDocument();

              try
              {
                lXmlDoc.LoadXml(lXml);

                lXmlDoc.Save(mCurrentFileName);
              }
              catch(Exception ex)
              {
                Logger.LogError(false, "FileManager[0x0003]: FileName={0}, GZipData-LoadXml => {1}", mCurrentFileName, ex.Message);

                Logger.LogError(false, "FileManager[0x0003]: FileName={0}, RawData={1}", mCurrentFileName, BitConverter.ToString(lRawData).Replace("-", " "));

                File.WriteAllText(mCurrentFileName, lXml);
              }
            }
            else
            {
              // SaveFile
              File.WriteAllBytes(mCurrentFileName, mRawData.ToArray());
            }

            mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0005, new byte[] { }));
          }

          break;
        }
        default:
        {
          lResponse = MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, 0x0001, new byte[] { });

          mManager.Send(lResponse);

          break;
        }
      }
    }

    private void CreateResourceDirectory(string directory)
    {
      Logger.LogDebug(false, "FileManager.CreateResourceDirectory: Directory={0}", directory);

      try
      {
        var lPath = Path.Combine(BaseDirectory, directory);

        if(!Directory.Exists(lPath))
          Directory.CreateDirectory(lPath);
      }
      catch(Exception ex)
      {
        Logger.LogError(false, "FileManager.CreateResourceDirectory: Message={0}", ex.Message);
      }
    }

    private void ProcessDirectoryInfo(MsgCmdFileTransfer m, string path)
    {
      /*
      - FileType: Unused
      - Function: Get Panel Hierarchy
      -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
      ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      [M->D] File Transfer: 02 | 01 1d | 02 10 | 00 06 27 11 00 00 | 00 06 7d 03 00 00 | ff | 4f 8e | 02 04 | 00 00 01 00 | 00 00 41 4d 58 50 61 6e 65 6c 2f ... de (AMXPanel/)
      [D->M] Ack          : 02 | 00 13 | 02 08 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | 02 32 | 00 01 | 27
      [D->M] FileTransfer : 02 | 00 36 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff bd | 02 04 | 00 00 01 01 | 00 00 00 00 00 00 10 00 00 33 8d 83 00 30 7b 66 2f 2e 61 6d 78 2f 41 4d 58 50 61 6e 65 6c 00 | dd (FullPath)
      [D->M] FileTransfer : 02 | 00 3a | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff be | 02 04 | 00 00 01 02 | 00 00 00 01 | 00 03 | 00 01 | 00 00 20 00 05 03 07 e4 12 09 16 | 41 4d 58 50 61 6e 65 6c 2f 69 6d 61 67 65 73 00 | 9b      (AMXPanel/images)
      [D->M] FileTransfer : 02 | 00 3a | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff bf | 02 04 | 00 00 01 02 | 00 00 00 01 | 00 03 | 00 02 | 00 00 10 00 0a 07 07 e3 0d 20 17 | 41 4d 58 50 61 6e 65 6c 2f 73 6f 75 6e 64 73 00 | ce | dd (AMXPanel/sounds)
      [D->M] FileTransfer : 02 | 00 39 | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff c0 | 02 04 | 00 00 01 02 | 00 00 00 01 | 00 03 | 00 03 | 00 00 10 00 05 03 07 e4 12 09 16 | 41 4d 58 50 61 6e 65 6c 2f 66 6f 6e 74 73 00 | 42         (AMXPanel/fonts)
      ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      ...
      [D->M] FileTransfer : 02 | 00 2c | 02 00 | 00 06 7d 03 00 00 | 00 06 27 11 00 00 | 0f | ff c2 | 02 04 | 00 00 01 02 | 00 00 00 00 | 00 00 | 00 00 | 00 00 00 00 00 00 00 00 00 00 00 | 3f 00 0c (? -> 0x3F- > no more entries)
      ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      ...
      */

      try
      {
        var lBaseDirectory = new DirectoryInfo(Path.Combine(BaseDirectory, path));

        Logger.LogDebug(false, "FileManager.ProcessDirectoryInfo[0x0101]: FullPath={0}", lBaseDirectory.FullName);

        // Directory Info ...
        var lBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x33, 0x8d, 0x83, 0x00, 0x30, 0x7b, 0x66 };

        lBytes = lBytes.Concat(Encoding.Default.GetBytes(lBaseDirectory.FullName + "\0")).ToArray();

        mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, 0x0101, lBytes));

        var lDirectories = lBaseDirectory.GetDirectories();

        if(lDirectories.Length > 0)
        {
          var lSequence = 0;

          foreach(var lDirectory in lDirectories)
          {
            byte[] lData;

            lSequence++;

            var lDirectoryName = string.Format("{0}{1}", path, lDirectory.Name);

            using(var lStream = new MemoryStream())
            {
              // Unknown (4 Bytes)
              lStream.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 0, 4);

              // Count
              lStream.Write(AmxUtils.Int16ToBigEndian(lDirectories.Length), 0, 2);
              // 00 00 00 01 | 00 03 | 00 01 | 00 00 20 00 05 03 07 e4 12 09 16

              // Sequence
              lStream.Write(AmxUtils.Int16ToBigEndian(lSequence), 0, 2);

              // Unknown (11 Bytes)
              lStream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, }, 0, 11);

              // Directory Name (Null-Terminated)
              lBytes = Encoding.Default.GetBytes(lDirectoryName + "\0");
              lStream.Write(lBytes, 0, lBytes.Length);

              lData = lStream.ToArray();
            }

            Logger.LogDebug(false, "FileManager.ProcessDirectoryInfo[0x0102]: Directory={0}", lDirectoryName);

            mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, 0x0102, lData));
          }
        }
        else
        {
          lBytes = new byte[] { 00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

          // ? (0x3F)
          lBytes = lBytes.Concat(Encoding.Default.GetBytes("?\0")).ToArray();

          Logger.LogDebug(false, "FileManager.ProcessDirectoryInfo[0x0102]: ?");

          mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Unused, 0x0102, lBytes)); // End
        }
      }
      catch(Exception ex)
      {
        Logger.LogError(false, "FileManager.ProcessDirectoryInfo: Message={0}", ex.Message);
      }
    }

    public string BaseDirectory { get; set; }
  }
}