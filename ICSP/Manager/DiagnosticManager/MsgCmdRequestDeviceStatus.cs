using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DiagnosticManager
{
  /// <summary>
  /// This message is used by the IDE to request the status of the specified device.
  /// The master responds with Output ON messages for each output channel that is on, 
  /// Feedback ON messages for each feedback channel that is on, etc.
  /// See the Diagnostic Manager specification for more information.
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.RequestDeviceStatus)]
  public class MsgCmdRequestDeviceStatus : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.RequestDeviceStatus;

    private MsgCmdRequestDeviceStatus()
    {
    }

    public MsgCmdRequestDeviceStatus(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device)
    {
      var lRequest = new MsgCmdRequestDeviceStatus();

      lRequest.Device = device;
      
      var lData = device.GetBytesDPS().ToArray();

      return lRequest.Serialize(AmxDevice.Empty, source, MsgCmd, lData);
    }
    
    public AmxDevice Device { get; set; }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} Device: {0}", GetType().Name, Device);
    }
  }
}