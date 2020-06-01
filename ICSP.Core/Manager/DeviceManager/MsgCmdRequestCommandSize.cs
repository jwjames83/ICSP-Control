using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
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

    public MsgCmdRequestCommandSize(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
        Device = AmxDevice.FromDPS(Data.Range(0, 6));
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestCommandSize(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdRequestCommandSize
      {
        Device = dest
      };

      var lData = dest.GetBytesDPS().ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {1:l}", GetType().Name, Device);
    }
  }
}