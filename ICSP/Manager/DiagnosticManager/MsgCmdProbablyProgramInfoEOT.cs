
using ICSP.Constants;

namespace ICSP.Manager.DiagnosticManager
{
  /// <summary>
  /// Unknown: (Probably ProgramInfo -> EOT)
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.ProbablyProgramInfoEOT)]
  public class MsgCmdProbablyProgramInfoEOT : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.ProbablyProgramInfoEOT;

    private MsgCmdProbablyProgramInfoEOT()
    {
    }

    public MsgCmdProbablyProgramInfoEOT(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdProbablyProgramInfoEOT(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source)
    {
      var lDest = new AmxDevice(0, 1, source.System);

      var lRequest = new MsgCmdProbablyProgramInfoEOT();
      
      return lRequest.Serialize(lDest, source, MsgCmd, null);
    }
    
    protected override void WriteLogExtended()
    {
    }
  }
}