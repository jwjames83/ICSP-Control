using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConfigurationManager
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

    public MsgCmdRequestMasterStatus(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        System = Data.GetBigEndianInt16(0);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestMasterStatus(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, ushort system)
    {
      var lRequest = new MsgCmdRequestMasterStatus
      {
        System = system
      };

      return lRequest.Serialize(dest, source, MsgCmd, AmxUtils.Int16ToBigEndian(lRequest.System));
    }
    
    public ushort System { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} System: {0}", GetType().Name, System);
    }
  }
}
