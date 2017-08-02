using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.ConnectionManager
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

    public MsgCmdDynamicDeviceAddressResponse(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = msg.Data.GetBigEndianInt16(0);

        System = msg.Data.GetBigEndianInt16(2);
      }
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
