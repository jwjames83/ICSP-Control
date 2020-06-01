using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
{
  /// <summary>
  /// This message requests, from the destination device, the status of the device or port.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestStatus)]
  public class MsgCmdRequestStatus : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestStatus;

    private MsgCmdRequestStatus()
    {
    }

    public MsgCmdRequestStatus(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
        Device = AmxDevice.FromDPS(Data.Range(0, 6));
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestStatus(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdRequestStatus
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
