using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message is the response from a master to the Request Level Count message above.
  /// It is sent by a device/port upon reporting if the device has more than eight levels
  /// </summary>
  [MsgCmd(DeviceManagerCmd.LevelCount)]
  public class MsgCmdLevelCount : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.LevelCount;

    private MsgCmdLevelCount()
    {
    }

    public MsgCmdLevelCount(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));

        Count = msg.Data.GetBigEndianInt16(6);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice device, ushort count)
    {
      var lDest = new AmxDevice(0, 0, device.System);

      var lRequest = new MsgCmdLevelCount();

      lRequest.Device = device;
      lRequest.Count = count;

      var lData = device.GetBytesDPS().Concat(ArrayExtensions.Int16ToBigEndian(count)).ToArray();

      return lRequest.Serialize(lDest, device, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    public ushort Count { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {1:l}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} Count : {1}", GetType().Name, Count);
    }
  }
}
