using System.Linq;

using ICSP.Constants;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message requests, from the destination device, the number of levels supported by the specified device/port. 
  /// The initial assumption that the master makes is that each device/port in the system has eight levels.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestLevelCount)]
  public class MsgCmdRequestLevelCount : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestLevelCount;

    private MsgCmdRequestLevelCount()
    {
    }

    public MsgCmdRequestLevelCount(ICSPMsgData msg) : base(msg)
    {
    }

    public static ICSPMsg CreateRequest(AmxDevice device)
    {
      var lDest = new AmxDevice(0, 0, device.System);

      var lRequest = new MsgCmdRequestLevelCount();

      lRequest.Device = device;

      var lData = device.GetBytesDPS().ToArray();

      return lRequest.Serialize(lDest, device, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {1:l}", GetType().Name, Device);
    }
  }
}
