using System;
using System.Linq;
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
        ChallengeData = Data.Range(0, 4);
      }
    }
    
    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdChallengeResponseMD5(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, byte[] challengeData, string username, string password)
    {
      var lUserBytes = Encoding.UTF8.GetBytes(username);
      var lPassBytes = Encoding.UTF8.GetBytes(password);

      // ---------------------------------------------------------------------------------------------------------------------------------
      // Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data      | CS
      // ---------------------------------------------------------------------------------------------------------------------------------

      // MD5 Fail ...
      // 02 08 | 00 01 7d 01 00 00 | 00 01 00 00 00 01 | 0f | 00 12 | 07 01 | 57 df 2f c6 | f8
      // 02 00 | 00 00 00 00 00 00 | 00 01 7d 01 00 00 | ff | 00 13 | 07 02 | 00 01 | dd 8a 2b 0e 5d e1 3e b0 f1 2e 21 43 ed e3 77 a8 | 02 (Hash: [57 df 2f c6, "", ""])

      // MD5 OK ...
      // 02 08 | 00 01 7d 01 00 00 | 00 01 00 00 00 01 | 0f | 00 15 | 07 01 | 16 b0 5a 69 | 59
      // 02 00 | 00 00 00 00 00 00 | 00 01 7d 01 00 00 | ff | 00 16 | 07 02 | 00 01 | 65 2b a6 82 78 ae ce 5f 78 34 8d 17 ca 1c 4d a3 | f8 (Hash: [16 b0 5a 69, username, password])
      //                                                                         10 | 24 38 9E 3D E4 FC 32 18 F2 11 86 60 DE 57 A7 A6      (Hash: [.., username, password])

      using var lHashAlgorithm = HashAlgorithm.Create("MD5");

      var lRequest = new MsgCmdChallengeResponseMD5
      {
        ChallengeData = challengeData,

        Hash = lHashAlgorithm.ComputeHash(
          challengeData
          .Concat(Encoding.UTF8.GetBytes(username))
          .Concat(Encoding.UTF8.GetBytes(password)).ToArray()),
      };

      var lData = new byte[] { 00, 01 }.Concat(lRequest.Hash).ToArray();
      
      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }
    
    public byte[] ChallengeData { get; private set; }

    public byte[] Hash { get; private set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} ChallengeData: 0x: {1}", GetType().Name, BitConverter.ToString(ChallengeData).Replace("-", string.Empty));
      Logger.LogDebug(false, "{0:l} Hash         : 0x: {1}", GetType().Name, BitConverter.ToString(Hash).Replace("-", string.Empty));
    }
  }
}
