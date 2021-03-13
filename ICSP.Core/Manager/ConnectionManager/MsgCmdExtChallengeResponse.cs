using System;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConnectionManager
{
  [MsgCmd(ConnectionManagerCmd.ExtChallengeResponse)] // MD5-Challenge
  public class MsgCmdExtChallengeResponse : ICSPMsg
  {
    public const int MsgCmd = ConnectionManagerCmd.ExtChallengeResponse;

    protected MsgCmdExtChallengeResponse()
    {
    }

    public MsgCmdExtChallengeResponse(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Challenge = Data.Range(0, 4);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdExtChallengeResponse(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, byte[] challenge)
    {
      throw new NotImplementedException();
    }

    public byte[] Challenge { get; private set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Challenge: 0x: {1:l}", GetType().Name, BitConverter.ToString(Challenge).Replace("-", " "));
    }
  }
}
