using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message requests, from the destination device, the number of ports supported by the device.
  /// The initial assumption that the master makes is that each device in the system has one port.
  /// If the device does not respond, the master assumes that it has one port.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestPortCount)]
  public class MsgCmdRequestPortCount : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestPortCount;

    private MsgCmdRequestPortCount()
    {
    }

    public MsgCmdRequestPortCount(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestPortCount(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort device, ushort system)
    {
      var lDest = new AmxDevice(0, 0, source.System);

      var lRequest = new MsgCmdRequestPortCount
      {
        Device = device,
        System = system
      };

      var lData = ArrayExtensions.Int16ToBigEndian(device)
        .Concat(ArrayExtensions.Int16ToBigEndian(system)).ToArray();

      return lRequest.Serialize(lDest, source, MsgCmd, lData);
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
