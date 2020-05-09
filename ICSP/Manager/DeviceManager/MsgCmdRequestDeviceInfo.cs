using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message is used by the IDE to request the status of the specified device.
  /// The master responds with Output ON messages for each output channel that is on, 
  /// Feedback ON messages for each feedback channel that is on, etc.
  /// See the Diagnostic Manager specification for more information.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestDeviceInfo)]
  public class MsgCmdRequestDeviceInfo : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestDeviceInfo;

    private MsgCmdRequestDeviceInfo()
    {
    }

    public MsgCmdRequestDeviceInfo(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = msg.Data.GetBigEndianInt16(0);

        System = msg.Data.GetBigEndianInt16(2);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort device, ushort system)
    {
      var lRequest = new MsgCmdRequestDeviceInfo();

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
      Logger.LogDebug(false, "{0:l} Device: {1:00000}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} System: {1}", GetType().Name, System);
    }
  }
}
