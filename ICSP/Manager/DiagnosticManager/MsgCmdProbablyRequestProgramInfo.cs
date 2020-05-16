using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DiagnosticManager
{
  /// <summary>
  /// Unknown: (Probably Request ProgramInfo)
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.ProbablyRequestProgramInfo)]
  public class MsgCmdProbablyRequestProgramInfo : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.ProbablyRequestProgramInfo;

    private MsgCmdProbablyRequestProgramInfo()
    {
    }

    public MsgCmdProbablyRequestProgramInfo(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 2)
        Unknown = Data.GetBigEndianInt16(0);
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdProbablyRequestProgramInfo(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort unknown)
    {
      var lDest = new AmxDevice(0, 1, source.System);

      var lRequest = new MsgCmdProbablyRequestProgramInfo
      {
        Unknown = unknown
      };

      var lData = ArrayExtensions.Int16ToBigEndian(unknown);

      return lRequest.Serialize(lDest, source, MsgCmd, lData);
    }
    
    public ushort Unknown { get; set; }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Unknown: {0}", GetType().Name, Unknown);
    }
  }
}