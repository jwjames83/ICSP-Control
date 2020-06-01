using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
{
  /// <summary>
  /// This message requests from the destination device, the number of output channel supported by the specified device/port.
  /// The initial assumption that the master makes is that each device/port in the system has 256 channels.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestOutputChannelCount)]
  public class MsgCmdRequestOutputChannelCount : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestOutputChannelCount;

    private MsgCmdRequestOutputChannelCount()
    {
    }

    public MsgCmdRequestOutputChannelCount(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
        Device = AmxDevice.FromDPS(Data.Range(0, 6));
    }
    
    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestOutputChannelCount(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdRequestOutputChannelCount
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
