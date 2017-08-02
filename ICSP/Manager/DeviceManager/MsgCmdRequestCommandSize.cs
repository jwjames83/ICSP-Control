using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message requests the number of elements per command and the string types supported by the device/port.
  /// The initial assumption that the master makes is that each device/port in the system 
  /// supports 64 elements/command and only supports 8-bit character arrays.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestCommandSize)]
  public class MsgCmdRequestCommandSize : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestCommandSize;

    private MsgCmdRequestCommandSize()
    {
    }

    public MsgCmdRequestCommandSize(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device)
    {
      var lRequest = new MsgCmdRequestCommandSize();

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