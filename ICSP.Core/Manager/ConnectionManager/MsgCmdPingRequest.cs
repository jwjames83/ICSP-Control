using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConnectionManager
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

    public MsgCmdPingRequest(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = Data.GetBigEndianInt16(0);

        System = Data.GetBigEndianInt16(2);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdPingRequest(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort device, ushort system)
    {
      var lRequest = new MsgCmdPingRequest
      {
        Device = device,
        System = system
      };

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

    public override void WriteLogVerbose()
    {
      Logger.LogDebug(false, "{0:l}: Dest={1:00000}:{2}", GetType().Name, Dest.Device, Dest.System);
    }
  }
}
