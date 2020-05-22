using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
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

    public MsgCmdDeviceInfoEOT(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = Data.GetBigEndianInt16(0);

        System = Data.GetBigEndianInt16(2);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdDeviceInfoEOT(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort device, ushort system)
    {
      var lRequest = new MsgCmdDeviceInfoEOT
      {
        Device = device,
        System = system
      };

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
      Logger.LogDebug(false, "{0:l} Device: {1:00000}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} System: {1}", GetType().Name, System);
    }
  }
}
