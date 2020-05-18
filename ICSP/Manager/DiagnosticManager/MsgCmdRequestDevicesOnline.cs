
using ICSP.Constants;

namespace ICSP.Manager.DiagnosticManager
{
  /// <summary>
  /// This message is used by the IDE to request a list of online devices for the receiving NetLinx master.
  /// The master will respond with Device Info message(s) for each device currently online.
  /// In addition, it will generate a Port Count message for each device.
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.RequestDevicesOnline)]
  public class MsgCmdRequestDevicesOnline : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.RequestDevicesOnline;

    private MsgCmdRequestDevicesOnline()
    {
    }

    public MsgCmdRequestDevicesOnline(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestDevicesOnline(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source)
    {
      var lDest = new AmxDevice(0, 0, source.System);

      var lRequest = new MsgCmdRequestDevicesOnline();
      
      return lRequest.Serialize(lDest, source, MsgCmd, null);
    }
    
    protected override void WriteLogExtended()
    {
    }
  }
}