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

    public MsgCmdMasterStatus(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        System = Data.GetBigEndianInt16(0);

        Status = (StatusType)Data.GetBigEndianInt16(2);

        StatusString = AmxUtils.GetNullStr(Data, 4);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdMasterStatus(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort system, StatusType status, string statusString)
    {
      var lStatusString = statusString ?? string.Empty;

      var lBytes = Encoding.Default.GetBytes(lStatusString + '\0');

      var lDest = new AmxDevice(0, 0, source.System);

      var lRequest = new MsgCmdMasterStatus
      {
        System = system,
        Status = status,
        StatusString = statusString
      };

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
      Logger.LogDebug(false, "{0:l} System      : {1:00000}", GetType().Name, System);
      Logger.LogDebug(false, "{0:l} Status      : {1} ({2})", GetType().Name, (ushort)Status, Status);
      Logger.LogDebug(false, "{0:l} StatusString: {1:l}", GetType().Name, StatusString);
    }
  }
}
