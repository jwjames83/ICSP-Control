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
    private const ushort MagicByteGZip          /**/ = 0x1F8B; // GZip
    private const ushort MagicByteGZipEncrypted /**/ = 0xBC06; // GZip Encrypted

    private readonly ICSPManager mManager;

    public readonly byte[] AccessToken = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }; // 0x00, 0x00, 0xC3, 0x50

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
*/

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

          // File Transfer Initialize ... (176)
          if(m.Function == (FileTransferFunction)0x0006) // 0x0006
          {
            // Received Files: 13 (00 0A)
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

            //       | FS    |             | FileName
            // 00 00 | 07 A2 | 00 00 4E 20 | 41 4D 58 50 61 6E 65 6C 2F 70 72 6A 2E 78 6D 61 -- AMXPanel/prj.xma

            var lFileSize = m.FileData.GetBigEndianInt16(2);

            lOffset = 8;
            mCurrentFileName = AmxUtils.GetNullStr(m.FileData, ref lOffset);

            Logger.LogDebug(false, "---------------------------------------------------------------------------");
            Logger.LogDebug(false, "FileManager[0x0102]: FileSize={0}", lFileSize);
            Logger.LogDebug(false, "FileManager[0x0102]: FileName={0}", mCurrentFileName);
            Logger.LogDebug(false, "---------------------------------------------------------------------------");

            mRawData = new List<byte>();

            // Function: -> 0x0102 FileName Info Request ...   (AMXPanel/fonts.xma)
            // Function: <- 0x0103 FileName Info Response ...- (Success/Token)
            mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0103, new byte[] { 0x07, 0xd0, 0x00, 0x00, 0xc3, 0x50, }));
          }

          // File Data
          if(m.Function == (FileTransferFunction)0x0003) // 0x0003
          {
            /*
            -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data                           | CS
            [M->D] File Transfer: 02 | 04 79 | 02 00 | 00 06 27 13 00 00 | 00 06 7D 03 00 00 | 0F | FF AE | 02 04 | 00 04 00 03 | 04 60 1F 8B 08 ... | E6
            [D->M] Ack          : 02 | 00 13 | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | FF AE | 00 01 | 8A
            [D->M] FileTransfer : 02 | 00 17 | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | 00 30 | 02 04 | 00 04 00 02 | 1C
            */

            // Size  | Raw
            // 07 D0 | 1F 8B 08 00 00 00 00 00 00 0B ED ...

            var lLenght = m.FileData.GetBigEndianInt16(0);

            Logger.LogDebug(false, "FileManager[0x0003]: FileName={0}, Size={1}, RawData  ...", mCurrentFileName, lLenght);

            mRawData.AddRange(m.FileData.Range(2, lLenght));

            // Function: -> 0x0003 RawData Request ...
            // Function: <- 0x0002 RawData Response ... (Next data)
            mManager.Send(MsgCmdFileTransfer.CreateRequest(m.Source, m.Dest, FileType.Axcess2Tokens, 0x0002, new byte[] { }));
          }

          // File Data Raw End
          if(m.Function == (FileTransferFunction)0x0004) // 0x0004
          {
            /*
            -                   : P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data      | CS
            [M->D] File Transfer: 02 | 00 17 | 02 00 | 00 06 27 13 00 00 | 00 06 7D 03 00 00 | 0F | FF AF | 02 04 | 00 04 00 04 | AC
            [D->M] Ack          : 02 | 00 13 | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | FF AF | 00 01 | 8B
            [D->M] FileTransfer : 02 | 00 17 | 02 00 | 00 06 7D 03 00 00 | 00 06 27 13 00 00 | FF | 00 31 | 02 04 | 00 04 00 05 | 20
            */
            
            WriteDataToFile();

            // Function: -> 0x0004 RawData End Request ...
            // Function: <- 0x0005 RawData End Response ... (Next file)
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

    private void WriteDataToFile()
    {
      try
      {
        Logger.LogDebug(false, "FileManager.WriteDataToFile: FileName={0}", mCurrentFileName);

        var lExt = Path.GetExtension(mCurrentFileName);

        if(lExt == ".xma" || lExt == ".xml")
        {
          var lRawData = mRawData.ToArray();

          if(lRawData.Length < 2)
          {
            Logger.LogDebug(false, "FileManager.WriteDataToFile: Invalid Raw Data, Size={0}", lRawData.Length);
            return;
          }

          // Get first 2 bytes
          var lMagicByte = lRawData.GetBigEndianInt16(0);

          if(lMagicByte == MagicByteGZipEncrypted)
          {
            /*
            Proj.xma ->
            <protection>none</protection>
            <password encrypted="1">
            </password>
            
            ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            Original:
            <?xml version="1.0"?>
            <root><page type="page"><pageID>1</pageID><name>Page</name><width>1024</width><height>600</height><sr number="1"><bs></bs><cb></cb><cf>#ccccccFF</cf><ct>#000000ff</ct><ec>#000000FF</ec><ff>arial.ttf</ff><fs>10</fs></sr><eventShow/><eventHide/><gestureAny/><gestureUp/><gestureDown/><gestureRight/><gestureLeft/><gestureDblTap/><gesture2FUp/><gesture2FDn/><gesture2FRt/><gesture2FLt/></page></root>
            
            Compressed (GZip): (00 FF)
            1F 8B 08 00 00 00 00 00 00 0B 55 91 DF 6A 83 30 14 87 EF 0B 7D 07 B1 F7 4D 2C 63 57 A7 29 03 91 15 7A 51 BA F5 01 A2 3D D1 80 26 92 A4 75 7D FB 9D 58 87 2E 17 C9 F7 FD C8 C9 5F 38 FC 74 6D F2 40 E7 B5 35 FB 34 DB F2 F4 20 D6 2B 70 D6 06 01 BD AC 31 09 CF 1E F7 69 C4 F4 95 1C 73 91 01 9B 08 8C EC 50 9C 49 80 8D 08 83 BE 85 46 64 7C F7 06 EC C5 D0 A0 AE 9B 20 DE 39 07 36 31 78 97 98 7B 57 A2 A3 5D 69 E1 D2 0B 60 B1 AB 4A 82 D8 55 4A 6C AA B1 15 05 25 8A 92 20 36 7C 6C 4A 51 42 8B 60 F5 97 C4 39 64 A0 94 90 4E CB 76 1B 02 CD 21 03 E5 E9 30 84 71 03 EF A8 E6 81 26 7C 35 76 60 13 7F EA 1B 12 D7 E8 C3 DD E1 87 79 CE 72 ED 67 CE ED 60 66 BB C4 5B CC 7A 42 B5 B0 BC 6C BF E5 A2 74 57 5C FF 59 6E 96 76 09 4B 3B 45 1B 5F 97 86 F1 1B D6 AB 5F 59 FA 71 B7 A6 01 00 00
            
            Encrypted: (01 B0 )
            BC 06 CB 95 A4 45 73 29 11 B8 4A 0A 80 B2 06 FC 82 DA 4A 8D 8A 40 10 D8 55 25 ED EA 8B 26 C2 54 A9 1A 9B D4 EF 6E 7E 96 DC 8E 63 08 97 D6 E2 8A B8 7F 9E ED 54 C7 01 DA D3 68 65 F9 51 B6 65 C3 DC 31 F2 CB CC A8 79 4C 14 BC 75 D3 8D 50 EE 4E BC 40 77 22 51 53 73 3A A4 66 D2 03 EA 12 65 ED 8A 61 96 CA C4 5A 50 74 3F DC 80 2C 7F 0D A5 9B 5A C6 D4 74 B9 D1 E4 51 C5 EA 77 BF 1B C1 B3 BE FF FC C3 6E 90 88 0F E2 7B 9E 31 74 7B 66 F1 DC 92 E6 54 55 C9 30 F1 CE 48 01 88 2B 88 9F C9 83 F4 9E 8B 1F F7 10 62 29 35 98 D6 09 3E 0A 7C 3E 6E 18 FA A8 7A E6 0E 19 D1 3D 42 18 89 09 A4 01 4E F4 0B 41 74 4E E4 51 DD E6 F7 58 65 08 A8 26 35 8A D4 68 79 F3 AE DA 84 13 C1 6D 5F DF 84 46 C9 68 B8 D4 A7 80 D4 80 88 88 77 10 4D 72 64 9F 89 8B 67 76 DE 32 90 DC 5C 14 5D AA 7F A8 AE E3 9D 9C A4 37 B5 0F FD D7 58 2C 6A 93 8B 07 3C F8 BF 9C D9 B6 DA 9A 29 99 7E AD FD 72 53 CF D7 E1 9B 77 E3 EF B8 40 A0 D2 D4 82 CC 13 C8 96 9F EA 48 97 A5 71 51 CC 20 7E B6 FA 6A 9F 7A 7F CF A4 0C AE 00 A1 54 1A EE C0 C8 99 13 60 8B 46 AF 12 B7 8C 5C 1E 13 2F E7 94 E4 A4 06 3F FE 14 FD C0 7F BB 5C CF 7A 44 F1 10 11 71 13 77 9A 05 52 B8 67 00 47 5B 07 57 F6 97 BF B6 59 F1 69 68 C8 DC 19 AD AB 47 8D 7A DD A7 53 EC A0 F7 59 30 4B 84 29 FE DF 6B 5B 6F 2D 94 C7 EE 5E C4 BD D4 3F 2E 23 15 37 6D 48 FB 40 78 90 0D 22 FC 7B 51 97 18
            ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            
            ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            Original:
            <?xml version="1.0"?>
            <root><page type="subpage" popupType="popup"><pageID>502</pageID><name>Empty</name><width>15</width><height>15</height><sr number="1"><bs></bs><cb>#ff0000FF</cb><cf>#0000dfff</cf><ct>#ffffffff</ct><ec>#000000FF</ec><ff>18cents.ttf</ff><fs>10</fs></sr></page></root>

            // 00 D2
            Compressed (GZip):
            1F 8B 08 00 00 00 00 00 00 0B 2D 90 CB 0E 82 30 10 45 F7 24 FE 03 29 7B 0A 26 26 2E C6 71 A3 26 EE FD 01 81 0E 90 08 34 ED E0 E3 EF 9D B6 CC A2 3D F7 E6 CE F4 01 E7 EF F4 CA DF C6 F9 71 99 4F AA 2E 2B 75 C6 5D 06 6E 59 18 C1 3E 7B 93 F3 CF 9A 93 F2 6B 13 94 CA ED 62 57 FB 88 5E 44 95 62 F7 0B 1E AA 3D E8 8D 61 7E 4E 06 AF 93 E5 1F E8 C8 F0 19 3B 1E B0 3E 80 4E 04 83 19 FB 81 A3 B3 21 78 97 CF EB D4 18 27 57 91 C1 8D 47 D0 61 69 1B 2C 88 2A A9 DB 0D B4 28 68 09 8B A0 3B 22 12 87 C4 E1 90 49 25 8E 4C 33 6D CA A4 2E 51 40 84 F5 B1 35 33 FB 92 59 52 A2 81 3C D6 95 60 38 CB 3B 4C 4F 90 2D 7E C1 2E FB 03 05 03 B7 07 22 01 00 00
  
            Encrypted:
            bc 06 cb 95 a4 45 73 29 11 b8 4a 0a 80 b2 06 fc 82 da 4a 8d 8a 40 10 d8 55 25 ed ea 8b 26 c2 54 b4 ac 3c 62 ca af 73 4a 9a e5 0e 88 d2 eb 10 d1 c3 93 dc e7 32 36 22 a3 7e 0c 15 5e fb e1 98 1c 97 5a 1d 02 32 20 83 72 a3 de ab 3d 98 bd 8a 0b 8e 14 3b 9a de dc 66 16 70 4c 1d 2a c1 17 77 16 0f f4 d0 ae 7d a9 3e b2 12 97 20 16 94 f5 61 bf 61 50 a9 74 99 b0 19 9b 21 cc dc 13 85 16 55 09 de 3e cb 56 59 a9 58 77 66 50 46 1d e9 ca 9b 81 fe bf db 2f 8c 73 cc cc 76 46 a2 b3 bf ab 38 ab 0b 19 d9 5e 3c 93 7c b7 8b 89 ec e7 79 bd 94 49 7f c3 f9 f4 5d b7 e2 e8 4c c3 f9 b1 3e 5f 9b a4 37 91 f2 3b 49 5f 2e ff 98 d6 da 3d 14 ac 85 b5 97 4e 6b 32 a3 d1 8c 04 e1 fa cd 6a 4d aa a6 90 d4 c8 19 5c 2d f6 ab 53 4c 10 72 4c d5 73 a4 3b 10 10 b5 ba 8c 0c 27 4c c2 69 54 24 d3 74 80 5e 91 d4 29 d6 61 42 06 00 fd 2e c3 17 87 4e 27 3e 9e a6 b9 ae 37 c4 a7 44 20 db ef ca 79 9d 71 37 48 df b2 d6 06 28 3a ea c4 5d d1 88 5e dc a1 ca
            ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            */

            // <?xml version="1.0"?>\r\n<root><page type="
            // 3c 3f 78 6d 6c 20 76 65 72 73 69 6f 6e 3d 22 31 2e 30 22 3f 3e 0d 0a 3c 72 6f 6f 74 3e 3c 70 61 67 65 20 74 79 70 65 3d 22

            // BC 06 CB 95 A4 45 73 29 11 B8 4A 0A 80 B2 06 FC 82 DA 4A 8D 8A 40 10 D8 55 25 ED EA 8B 26 C2 54 | A9 1A 9B D4 EF 6E 7E 96 DC 8E 63 08 97 D6 E2 8A B8 7F 9E ED 54 C7 01 DA D3 68 65 F9 51 B6 65 C3 DC 31 F2 CB CC A8 79 4C 14 BC 75 D3 8D 50 EE 4E BC 40 77 22 51 53 73 3A A4 66 D2 03 EA 12 65 ED 8A 61 96 CA C4 5A 50 74 3F DC 80 2C 7F 0D A5 9B 5A C6 D4 74 B9 D1 E4 51 C5 EA 77 BF 1B C1 B3 BE FF FC C3 6E 90 88 0F E2 7B 9E 31 74 7B 66 F1 DC 92 E6 54 55 C9 30 F1 CE 48 01 88 2B 88 9F C9 83 F4 9E 8B 1F F7 10 62 29 35 98 D6 09 3E 0A 7C 3E 6E 18 FA A8 7A E6 0E 19 D1 3D 42 18 89 09 A4 01 4E F4 0B 41 74 4E E4 51 DD E6 F7 58 65 08 A8 26 35 8A D4 68 79 F3 AE DA 84 13 C1 6D 5F DF 84 46 C9 68 B8 D4 A7 80 D4 80 88 88 77 10 4D 72 64 9F 89 8B 67 76 DE 32 90 DC 5C 14 5D AA 7F A8 AE E3 9D 9C A4 37 B5 0F FD D7 58 2C 6A 93 8B 07 3C F8 BF 9C D9 B6 DA 9A 29 99 7E AD FD 72 53 CF D7 E1 9B 77 E3 EF B8 40 A0 D2 D4 82 CC 13 C8 96 9F EA 48 97 A5 71 51 CC 20 7E B6 FA 6A 9F 7A 7F CF A4 0C AE 00 A1 54 1A EE C0 C8 99 13 60 8B 46 AF 12 B7 8C 5C 1E 13 2F E7 94 E4 A4 06 3F FE 14 FD C0 7F BB 5C CF 7A 44 F1 10 11 71 13 77 9A 05 52 B8 67 00 47 5B 07 57 F6 97 BF B6 59 F1 69 68 C8 DC 19 AD AB 47 8D 7A DD A7 53 EC A0 F7 59 30 4B 84 29 FE DF 6B 5B 6F 2D 94 C7 EE 5E C4 BD D4 3F 2E 23 15 37 6D 48 FB 40 78 90 0D 22 FC 7B 51 97 18
            // bc 06 cb 95 a4 45 73 29 11 b8 4a 0a 80 b2 06 fc 82 da 4a 8d 8a 40 10 d8 55 25 ed ea 8b 26 c2 54 | b4 ac 3c 62 ca af 73 4a 9a e5 0e 88 d2 eb 10 d1 c3 93 dc e7 32 36 22 a3 7e 0c 15 5e fb e1 98 1c 97 5a 1d 02 32 20 83 72 a3 de ab 3d 98 bd 8a 0b 8e 14 3b 9a de dc 66 16 70 4c 1d 2a c1 17 77 16 0f f4 d0 ae 7d a9 3e b2 12 97 20 16 94 f5 61 bf 61 50 a9 74 99 b0 19 9b 21 cc dc 13 85 16 55 09 de 3e cb 56 59 a9 58 77 66 50 46 1d e9 ca 9b 81 fe bf db 2f 8c 73 cc cc 76 46 a2 b3 bf ab 38 ab 0b 19 d9 5e 3c 93 7c b7 8b 89 ec e7 79 bd 94 49 7f c3 f9 f4 5d b7 e2 e8 4c c3 f9 b1 3e 5f 9b a4 37 91 f2 3b 49 5f 2e ff 98 d6 da 3d 14 ac 85 b5 97 4e 6b 32 a3 d1 8c 04 e1 fa cd 6a 4d aa a6 90 d4 c8 19 5c 2d f6 ab 53 4c 10 72 4c d5 73 a4 3b 10 10 b5 ba 8c 0c 27 4c c2 69 54 24 d3 74 80 5e 91 d4 29 d6 61 42 06 00 fd 2e c3 17 87 4e 27 3e 9e a6 b9 ae 37 c4 a7 44 20 db ef ca 79 9d 71 37 48 df b2 d6 06 28 3a ea c4 5d d1 88 5e dc a1 ca

            // Emtpy Sub Pages:
            // bc 06 cb 95 a4 45 73 29 11 b8 4a 0a 80 b2 06 fc 82 da 4a 8d 8a 40 10 d8 55 25 ed ea 8b 26 c2 54 b4 ac 3c 62 ca af 73 4a 9a e5 0e 88 d2 eb 10 d1 c3 93 dc e7 32 36 22 a3 7e 0c 15 5e fb e1 98 1c | 97 5a 1d 02 32 20 83 72 a3 de ab 3d 98 bd 8a 0b 8e 14 3b 9a de dc 66 16 70 4c 1d 2a c1 17 77 16 0f f4 d0 ae 7d a9 3e b2 12 97 20 16 94 f5 61 bf 61 50 a9 74 99 b0 19 9b 21 cc dc 13 85 16 55 09 de 3e cb 56 59 a9 58 77 66 50 46 1d e9 ca 9b 81 fe bf db 2f 8c 73 cc cc 76 46 a2 b3 bf ab 38 ab 0b 19 d9 5e 3c 93 7c b7 8b 89 ec e7 79 bd 94 49 7f c3 f9 f4 5d b7 e2 e8 4c c3 f9 b1 3e 5f 9b a4 37 91 f2 3b 49 5f 2e ff 98 d6 da 3d 14 ac 85 b5 97 4e 6b 32 a3 d1 8c 04 e1 fa cd 6a 4d aa a6 90 d4 c8 19 5c 2d f6 ab 53 4c 10 72 4c d5 73 a4 3b 10 10 b5 ba 8c 0c 27 4c c2 69 54 24 d3 74 80 5e 91 d4 29 d6 61 42 06 00 fd 2e c3 17 87 4e 27 3e 9e a6 b9 ae 37 c4 a7 44 20 db ef ca 79 9d 71 37 48 df b2 d6 06 28 3a ea c4 5d d1 88 5e dc a1 ca
            // bc 06 cb 95 a4 45 73 29 11 b8 4a 0a 80 b2 06 fc 82 da 4a 8d 8a 40 10 d8 55 25 ed ea 8b 26 c2 54 b4 ac 3c 62 ca af 73 4a 9a e5 0e 88 d2 eb 10 d1 c3 93 dc e7 32 36 22 a3 7e 0c 15 5e fb e1 98 1c | 56 ac ff 45 e3 5e a3 5a 88 14 06 df bd 2c 78 42 03 e2 e8 a3 23 9c 5f 6a fa ca eb b7 b9 67 fb 0d  
            // bc 06 cb 95 a4 45 73 29 11 b8 4a 0a 80 b2 06 fc 82 da 4a 8d 8a 40 10 d8 55 25 ed ea 8b 26 c2 54 b4 ac 3c 62 ca af 73 4a 9a e5 0e 88 d2 eb 10 d1 c3 93 dc e7 32 36 22 a3 7e 0c 15 5e fb e1 98 1c | 3f a1 5c 65 6d 91 85 04 0e de c1 f9 5b d9 73 cd a6 3c 4a 7f 07 07 8c 3d 8d f2 8c f7 9d c1 91 f2  
            // bc 06 cb 95 a4 45 73 29 11 b8 4a 0a 80 b2 06 fc 82 da 4a 8d 8a 40 10 d8 55 25 ed ea 8b 26 c2 54 b4 ac 3c 62 ca af 73 4a 9a e5 0e 88 d2 eb 10 d1 c3 93 dc e7 32 36 22 a3 7e 0c 15 5e fb e1 98 1c | df 18 26 da 17 63 80 84 0a 95 94 82 90 94 27 93 b0 b1 d7 6b df f0 62 46 6c ff 38 e9 9d aa e9 ba  

            // Write Raw ...
            File.WriteAllBytes(mCurrentFileName, mRawData.ToArray());

            return;
          }

          // .xma -> Archive (GZip)
          if(lMagicByte == MagicByteGZip)
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
              Logger.LogError(false, "FileManager.WriteDataToFile: FileName={0}, GZip.Decompress => {1}", mCurrentFileName, ex.Message);
              Logger.LogError(false, "FileManager.WriteDataToFile: FileName={0}, RawData={1}", mCurrentFileName, BitConverter.ToString(lRawData).Replace("-", " "));
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
            Logger.LogError(false, "FileManager.WriteDataToFile: FileName={0}, XmlDocument.LoadXml => {1}", mCurrentFileName, ex.Message);
            Logger.LogError(false, "FileManager.WriteDataToFile: FileName={0}, RawData={1}", mCurrentFileName, BitConverter.ToString(lRawData).Replace("-", " "));

            File.WriteAllBytes(mCurrentFileName, mRawData.ToArray());
          }
        }
        else
        {
          // Write Raw ...
          File.WriteAllBytes(mCurrentFileName, mRawData.ToArray());
        }
      }
      catch(Exception ex)
      {
        Logger.LogError(false, "FileManager.WriteDataToFile: Message={0}", ex.Message);
      }
    }

    public string BaseDirectory { get; set; }
  }
}