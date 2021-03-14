using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConnectionManager
{
  [MsgCmd(ConnectionManagerCmd.ChallengeResponseMD5)] // MD5-Challenge
  public class MsgCmdChallengeResponseMD5 : ICSPMsg
  {
    public const int MsgCmd = ConnectionManagerCmd.ChallengeResponseMD5;

    protected MsgCmdChallengeResponseMD5()
    {
    }

    public MsgCmdChallengeResponseMD5(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Challenge = Data.Range(0, 4);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdChallengeResponseMD5(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, byte[] challenge, EncryptionType encryptionType, NetworkCredential credentials)
    {
      // ---------------------------------------------------------------------------------------------------------------------------------
      // Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data      | CS
      // ---------------------------------------------------------------------------------------------------------------------------------
      // 02 08 | 00 01 7d 01 00 00 | 00 01 00 00 00 01 | 0f | 00 15 | 07 01 | 16 b0 5a 69 | 59
      // 02 00 | 00 00 00 00 00 00 | 00 01 7d 01 00 00 | ff | 00 16 | 07 02 | 00 01 | 65 2b a6 82 78 ae ce 5f 78 34 8d 17 ca 1c 4d a3 | f8 (MD5: [16 b0 5a 69, Base64(username), Base64(password)])

      using var lHashAlgorithm = HashAlgorithm.Create("MD5");

      if(lHashAlgorithm == null)
        throw new Exception("ICSP: Authentication Challenge received but not MD5 available.");

      var lRequest = new MsgCmdChallengeResponseMD5
      {
        Challenge = challenge,

        EncryptionType = encryptionType,

        Hash = lHashAlgorithm.ComputeHash(
          challenge
          .Concat(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials?.UserName ?? ICSPManager.DefaultUsername))))
          .Concat(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials?.Password ?? ICSPManager.DefaultPassword)))).ToArray()),
      };

      var lData = ArrayExtensions.Int16ToBigEndian((ushort)lRequest.EncryptionType).Concat(lRequest.Hash).ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }

    public byte[] Challenge { get; private set; }

    /// <summary>
    /// Assumption:
    /// 0: None => This value does not work, if the option [Encrypt ICSP connection] is enabled on the controller
    /// 1: RC4
    /// </summary>
    public EncryptionType EncryptionType { get; private set; }

    public byte[] Hash { get; private set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Challenge: 0x: {1:l}", GetType().Name, BitConverter.ToString(Challenge).Replace("-", " "));
      Logger.LogDebug(false, "{0:l} Hash     : 0x: {1:l}", GetType().Name, BitConverter.ToString(Hash).Replace("-", " "));
    }
  }
}
