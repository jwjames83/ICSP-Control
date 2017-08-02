using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.ConnectionManager
{
  /// <summary>
  /// This message is sent by a master to request determine the presence of the specified device in the system.
  /// </summary>
  [MsgCmd(ConnectionManagerCmd.PingRequest)]
  public class MsgCmdPingRequest : ICSPMsg
  {
    public const int MsgCmd = ConnectionManagerCmd.PingRequest;

    private MsgCmdPingRequest()
    {
    }

    public MsgCmdPingRequest(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = msg.Data.GetBigEndianInt16(0);

        System = msg.Data.GetBigEndianInt16(2);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort device, ushort system)
    {
      var lRequest = new MsgCmdPingRequest();

      lRequest.Device = device;
      lRequest.System = system;

      var lData = ArrayExtensions.Int16ToBigEndian(device)
        .Concat(ArrayExtensions.Int16ToBigEndian(system)).ToArray();

      return lRequest.Serialize(AmxDevice.Empty, source, MsgCmd, lData);
    }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort Device { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort System { get; private set; }

    public override void WriteLog(bool last)
    {
      Logger.LogDebug(false, "{0}: Dest={1:00000}:{2}", GetType().Name, Dest.Device, Dest.System);
    }
  }
}
