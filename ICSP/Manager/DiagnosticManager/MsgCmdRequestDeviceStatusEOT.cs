using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DiagnosticManager
{
  /// <summary>
  /// This message indicates that all of the Request Device Status information has been sent.
  /// This message is used by the IDE to determine when all device status information has been received.
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.RequestDeviceStatusEOT)]
  public class MsgCmdRequestDeviceStatusEOT : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.RequestDeviceStatusEOT;

    private MsgCmdRequestDeviceStatusEOT()
    {
    }

    public MsgCmdRequestDeviceStatusEOT(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device)
    {
      var lRequest = new MsgCmdRequestDeviceStatusEOT();

      lRequest.Device = device;
      
      var lData = device.GetBytesDPS().ToArray();

      return lRequest.Serialize(AmxDevice.Empty, source, MsgCmd, lData);
    }
    
    public AmxDevice Device { get; set; }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} Device: {1}", GetType().Name, Device);
    }
  }
}