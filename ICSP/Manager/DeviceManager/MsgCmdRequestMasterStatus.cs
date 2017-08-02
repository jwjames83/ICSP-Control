using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.ConfigurationManager
{
  /// <summary>
  /// This message requests that the master respond with its status.
  /// Generally, Master Status message will be unsolicited, this message is included for completeness.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestMasterStatus)]
  public class MsgCmdRequestMasterStatus : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestMasterStatus;

    private MsgCmdRequestMasterStatus()
    {
    }

    public MsgCmdRequestMasterStatus(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        System = msg.Data.GetBigEndianInt16(0);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort system)
    {
      var lRequest = new MsgCmdRequestMasterStatus();

      lRequest.System = system;

      return lRequest.Serialize(AmxDevice.Empty, source, MsgCmd, AmxUtils.Int16ToBigEndian(lRequest.System));
    }
    
    public ushort System { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} System: {0}", GetType().Name, System);
    }
  }
}
