using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConnectionManager
{
  [MsgCmd(ConnectionManagerCmd.ChallengeAckMD5)]
  public class MsgCmdChallengeAckMD5 : ICSPMsg
  {
    public const int MsgCmd = ConnectionManagerCmd.ChallengeAckMD5;

    protected MsgCmdChallengeAckMD5()
    {
    }

    public MsgCmdChallengeAckMD5(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Status = (AuthenticationState)Data.GetBigEndianInt16(0);

        ExtendedData = Data.GetBigEndianInt16(2);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdChallengeAckMD5(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, AuthenticationState status, ushort extendedData)
    {
      var lRequest = new MsgCmdChallengeAckMD5
      {
        Status = status,
        ExtendedData = extendedData,
      };

      var lData = ArrayExtensions.Int16ToBigEndian((ushort)lRequest.Status)
        .Concat(ArrayExtensions.Int16ToBigEndian(lRequest.ExtendedData)).ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }

    public AuthenticationState Status { get; private set; }

    public ushort ExtendedData { get; private set; }

    public bool Authenticated => (Status & AuthenticationState.Authenticated) > 0;

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Status      : 0x{1:X4} ({2})", GetType().Name, (ushort)Status, Status);
      Logger.LogDebug(false, "{0:l} ExtendedData: 0x{1:X4}", GetType().Name, ExtendedData);
    }
  }
}
