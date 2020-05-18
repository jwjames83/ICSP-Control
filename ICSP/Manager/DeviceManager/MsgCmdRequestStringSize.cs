using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message requests the number of elements per string and the string types supported by the device/port.
  /// The initial assumption that the master makes is that each device/port in the system 
  /// supports 64 elements/string and only supports 8-bit character strings.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestStringSize)]
  public class MsgCmdRequestStringSize : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestStringSize;

    private MsgCmdRequestStringSize()
    {
    }

    public MsgCmdRequestStringSize(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
        Device = AmxDevice.FromDPS(Data.Range(0, 6));
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestStringSize(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device)
    {
      var lRequest = new MsgCmdRequestStringSize
      {
        Device = device
      };

      var lData = device.GetBytesDPS().ToArray();

      return lRequest.Serialize(device, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {1:l}", GetType().Name, Device);
    }
  }
}
