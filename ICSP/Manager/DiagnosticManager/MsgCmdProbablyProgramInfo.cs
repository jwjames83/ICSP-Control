using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DiagnosticManager
{
  /// <summary>
  /// Unknown: (Probably ProgramInfo -> Info)
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.ProbablyProgramInfo)]
  public class MsgCmdProbablyProgramInfo : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.ProbablyProgramInfo;

    private MsgCmdProbablyProgramInfo()
    {
    }

    public MsgCmdProbablyProgramInfo(ICSPMsgData msg) : base(msg)
    {
      var lOffset = 0;

      System = string.Empty;
      ProgramName = string.Empty;
      MainFile = string.Empty;

      /*
      02 00 67 02 00 00 01 7d 01 00 01 00 01 00 00 00  ..g....}........
      01 0f ff d3 01 0c 
                        80 00 00 01 00 00 01 8c 4e 58  ..............NX
      2d 31 32 30 30 20 4d 61 73 74 65 72 20 76 31 2e  -1200 Master v1.
      35 2e 37 38 00 49 43 53 50 2d 54 65 73 74 20 22  5.78.ICSP-Test "
      4d 61 69 6e 2e 61 78 73 22 00 18 1c ac 10 10 65  Main.axs"......e
      05 27 00 60 9f 9c 95 fa 00 00 00 00 00 00 00 00  .'.`............
      00 00 ff ff ac 10 10 65 00 00 1b                 .......e...
      */

      if(msg.Data.Length > 8)
      {
        // 80 00 00 01 00 00 01 8c Unknown
        // NX-1200 Master v1.5.78 (Null Terminated)
        // ICSP-Test "Main.axs"   (Null Terminated)
        // 18 1c ac 10 ... Unkwnown

        lOffset = 8;

        // System ?
        System = AmxUtils.GetNullStr(msg.Data, ref lOffset);

        // ProgramInfo ?
        if(lOffset <= msg.Data.Length)
        {
          var lProgramInfo = AmxUtils.GetNullStr(msg.Data, ref lOffset);

          // ICSP - "Test  "Main.axs"
          if(!string.IsNullOrWhiteSpace(lProgramInfo) && lProgramInfo.Contains("\""))
          {
            lProgramInfo = lProgramInfo.TrimEnd('\"');

            // ICSP - "Test  "Main.axs
            var lPos = lProgramInfo.LastIndexOf('"');

            if(lPos >= 0 && lPos + 1 <= lProgramInfo.Length)
            {
              ProgramName = lProgramInfo.Substring(0, lPos);

              MainFile = lProgramInfo.Substring(lPos + 1);
            }
          }

          if(string.IsNullOrWhiteSpace(ProgramName))
            ProgramName = lProgramInfo;
        }
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source)
    {
      var lDest = new AmxDevice(0, 0, source.System);

      var lRequest = new MsgCmdProbablyProgramInfo();

      // lRequest.Device = device;

      var lData = new byte[] { };

      return lRequest.Serialize(lDest, source, MsgCmd, lData);
    }

    public string System { get; private set; }
    
    public string ProgramName { get; private set; }

    public string MainFile { get; private set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} System     : {1}", GetType().Name, System);
      Logger.LogDebug(false, "{0} ProgramName: {1}", GetType().Name, ProgramName);
      Logger.LogDebug(false, "{0} MainFile   : {1}", GetType().Name, MainFile);
    }
  }
}