using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DiagnosticManager
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

    public MsgCmdRequestDeviceStatusEOT(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
        Device = AmxDevice.FromDPS(Data.Range(0, 6));
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestDeviceStatusEOT(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdRequestDeviceStatusEOT
      {
        Device = source
      };

      var lData = source.GetBytesDPS().ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }
    
    public AmxDevice Device { get; set; }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {1:l}", GetType().Name, Device);
    }
  }
}