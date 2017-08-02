using System.Linq;
using System.Text;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message is the response to the Request Master Status message above,
  /// but more commonly will be sent by the master unsolicited.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.MasterStatus)]
  public class MsgCmdMasterStatus : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.MasterStatus;

    private MsgCmdMasterStatus()
    {
    }

    public MsgCmdMasterStatus(ICSPMsgData msg) : base(msg)
    {
      var lOffset = 0;

      if(msg.Data.Length > 0)
      {
        System = msg.Data.GetBigEndianInt16(0);

        Status = (StatusType)msg.Data.GetBigEndianInt16(2);

        lOffset = 4;

        StatusString = AmxUtils.GetNullStr(msg.Data, ref lOffset);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort system, StatusType status, string statusString)
    {
      var lStatusString = statusString ?? string.Empty;

      var lBytes = Encoding.Default.GetBytes(lStatusString + '\0');

      var lDest = new AmxDevice(0, 0, source.System);

      var lRequest = new MsgCmdMasterStatus();

      lRequest.System = system;
      lRequest.Status = status;
      lRequest.StatusString = statusString;

      var lData = ArrayExtensions.Int16ToBigEndian(system)
        .Concat(ArrayExtensions.Int16ToBigEndian((ushort)status))
        .Concat(lBytes).ToArray();

      return lRequest.Serialize(lDest, source, MsgCmd, lData);
    }

    public ushort System { get; set; }

    public StatusType Status { get; set; }

    /// <summary>
    /// Containing a text description of the status. For example, "Master Reset"
    /// </summary>
    public string StatusString { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} System      : {1:00000}", GetType().Name, System);
      Logger.LogDebug(false, "{0} Status      : {1} ({2})", GetType().Name, (ushort)Status, Status);
      Logger.LogDebug(false, "{0} StatusString: {1}", GetType().Name, StatusString);
    }
  }
}
