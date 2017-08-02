using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message requests from the destination device, the number of output channel supported by the specified device/port.
  /// The initial assumption that the master makes is that each device/port in the system has 256 channels.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestOutputChannelCount)]
  public class MsgCmdRequestOutputChannelCount : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestOutputChannelCount;

    private MsgCmdRequestOutputChannelCount()
    {
    }

    public MsgCmdRequestOutputChannelCount(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device)
    {
      var lRequest = new MsgCmdRequestOutputChannelCount();

      lRequest.Device = device;

      var lData = device.GetBytesDPS().ToArray();

      return lRequest.Serialize(device, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} Device: {1}", GetType().Name, Device);
    }
  }
}
