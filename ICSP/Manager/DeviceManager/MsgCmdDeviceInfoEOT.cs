using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message indicates that all data (Device Info messages) have been sent as a result of a Request Device Info message.
  /// This message indicates to the requesting device that all Device Info information has been provided.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.DeviceInfoEOT)]
  public class MsgCmdDeviceInfoEOT : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.DeviceInfoEOT;

    private MsgCmdDeviceInfoEOT()
    {
    }

    public MsgCmdDeviceInfoEOT(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = msg.Data.GetBigEndianInt16(0);

        System = msg.Data.GetBigEndianInt16(2);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort device, ushort system)
    {
      var lRequest = new MsgCmdDeviceInfoEOT();

      lRequest.Device = device;
      lRequest.System = system;

      var lData = ArrayExtensions.Int16ToBigEndian(device)
        .Concat(ArrayExtensions.Int16ToBigEndian(system)).ToArray();

      return lRequest.Serialize(AmxDevice.Empty, source, MsgCmd, lData);
    }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort Device { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort System { get; private set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} Device: {1:00000}", GetType().Name, Device);
      Logger.LogDebug(false, "{0} System: {1}", GetType().Name, System);
    }
  }
}
