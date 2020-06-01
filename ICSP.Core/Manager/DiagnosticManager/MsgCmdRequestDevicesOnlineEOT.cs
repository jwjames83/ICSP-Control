
using ICSP.Core.Constants;

namespace ICSP.Core.Manager.DiagnosticManager
{
  /// <summary>
  /// This message is used by the IDE to request a list of online devices for the receiving NetLinx master.
  /// The master will respond with Device Info message(s) for each device currently online.
  /// In addition, it will generate a Port Count message for each device.
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.RequestDevicesOnlineEOT)]
  public class MsgCmdRequestDevicesOnlineEOT : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.RequestDevicesOnlineEOT;

    private MsgCmdRequestDevicesOnlineEOT()
    {
    }

    public MsgCmdRequestDevicesOnlineEOT(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestDevicesOnlineEOT(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdRequestDevicesOnlineEOT();
      
      return lRequest.Serialize(dest, source, MsgCmd, null);
    }
    
    protected override void WriteLogExtended()
    {
    }
  }
}