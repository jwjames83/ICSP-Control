using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message requests, from the destination device, the status of the device or port.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestStatus)]
  public class MsgCmdRequestStatus : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestStatus;

    private MsgCmdRequestStatus()
    {
    }

    public MsgCmdRequestStatus(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device)
    {
      var lRequest = new MsgCmdRequestStatus();

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
