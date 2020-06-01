using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
{
  /// <summary>
  /// This message requests, from the destination device, the number of levels supported by the specified device/port. 
  /// The initial assumption that the master makes is that each device/port in the system has eight levels.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestLevelCount)]
  public class MsgCmdRequestLevelCount : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestLevelCount;

    private MsgCmdRequestLevelCount()
    {
    }

    public MsgCmdRequestLevelCount(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestLevelCount(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdRequestLevelCount
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
