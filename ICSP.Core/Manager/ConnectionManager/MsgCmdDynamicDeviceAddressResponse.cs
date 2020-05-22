using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConnectionManager
{
  /// <summary>
  /// This message is the response to the Request Dynamic Device Address message.
  /// It returns a device number for “temporary” use by the requesting device.
  /// 
  /// The device can use this device number for as long as it has communication with the master.
  /// The returned device number may the one suggested by the device in the Request Dynamic Device Address message or may be different.
  /// </summary>
  [MsgCmd(ConnectionManagerCmd.DynamicDeviceAddressResponse)]
  public class MsgCmdDynamicDeviceAddressResponse : ICSPMsg
  {
    public const int MsgCmd = ConnectionManagerCmd.DynamicDeviceAddressResponse;

    private MsgCmdDynamicDeviceAddressResponse()
    {
    }

    public MsgCmdDynamicDeviceAddressResponse(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = Data.GetBigEndianInt16(0);

        System = Data.GetBigEndianInt16(2);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdDynamicDeviceAddressResponse(bytes);
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
