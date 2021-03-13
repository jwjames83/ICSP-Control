using System;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConnectionManager
{
  [MsgCmd(ConnectionManagerCmd.ExtChallengeRequest)] // MD5-Challenge
  public class MsgCmdExtChallengeRequest : ICSPMsg
  {
    public const int MsgCmd = ConnectionManagerCmd.ExtChallengeRequest;

    protected MsgCmdExtChallengeRequest()
    {
    }

    public MsgCmdExtChallengeRequest(byte[] buffer) : base(buffer)
    {
      // Data: (298 Bytes)
      // 00 00 01 26 30 82 01 22 30 0D 06 09 2A 86 48 86 F7 0D 01 01 01 05 00 03 82 01 0F 00 30 82 01 0A 02 82 01 01 00 F2 28 3A 0E 6A 79 1B DC 22 86 6D F1 B1 62 11 F2 18 9C 5A 57 F6 57 3E 8A D2 C5 B4 2D 67 C1 20 B7 FD EF 15 57 49 CA EB E4 A3 E2 4F 38 7A 1C 08 40 61 31 DA 11 F3 68 A3 05 78 04 B2 66 53 0F 52 C2 DC 89 D5 21 15 FE 0A 60 A0 BF 62 31 80 46 96 AD 57 FD D4 09 42 E8 BE 1C 4A 2E 73 06 26 B4 B2 C7 23 8B 54 9A 8F 04 5D 0C A9 E4 2D BC DF 1D 0F AD 0E CF B4 54 94 8A 7B A4 D8 45 93 2D C2 EE 6E 02 71 BB D3 C7 00 DD 17 99 58 AD 6F 4B F6 7B BD CC 80 DA 0F 00 54 5D 83 E3 93 03 60 53 EB 73 99 34 37 54 C0 D4 5E 44 28 08 CA 69 FB 09 A4 79 23 65 AD 6B F6 7A A0 3A 2B 79 B0 64 29 49 C4 DF 49 7D F5 DD 21 67 05 19 85 7F 59 D1 25 88 31 E8 42 6F 3D 49 B9 61 B5 D0 F7 97 4E 91 AA FF B3 ED 1C 71 7B 5F 2D 0B D6 3E 3D 7D DC 1F 6E FE 4B 88 91 B4 54 6A 7E D5 5B 7D 30 3F 3C 0D BA 7A 2F 51 F1 D3 02 03 01 00 01
      // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // 00 00    | 01 26 -> Len: 294
      // 30 82    | 01 22 -> Len: 290
      // 30 0D 06 09 2A 86 48 86 F7 0D 01 01 01 05 00 03 82 | 01 0F -> Len: 271
      // 00 30 82 | 01 0A -> Len: 266
      // 02 82 01 | 01 00 -> Len: 256
      // F2 28 3A 0E 6A 79 1B DC 22 86 6D F1 B1 62 11 F2 18 9C 5A 57 F6 57 3E 8A D2 C5 B4 2D 67 C1 20 B7 FD EF 15 57 49 CA EB E4 A3 E2 4F 38 7A 1C 08 40 61 31 DA 11 F3 68 A3 05 78 04 B2 66 53 0F 52 C2 DC 89 D5 21 15 FE 0A 60 A0 BF 62 31 80 46 96 AD 57 FD D4 09 42 E8 BE 1C 4A 2E 73 06 26 B4 B2 C7 23 8B 54 9A 8F 04 5D 0C A9 E4 2D BC DF 1D 0F AD 0E CF B4 54 94 8A 7B A4 D8 45 93 2D C2 EE 6E 02 71 BB D3 C7 00 DD 17 99 58 AD 6F 4B F6 7B BD CC 80 DA 0F 00 54 5D 83 E3 93 03 60 53 EB 73 99 34 37 54 C0 D4 5E 44 28 08 CA 69 FB 09 A4 79 23 65 AD 6B F6 7A A0 3A 2B 79 B0 64 29 49 C4 DF 49 7D F5 DD 21 67 05 19 85 7F 59 D1 25 88 31 E8 42 6F 3D 49 B9 61 B5 D0 F7 97 4E 91 AA FF B3 ED 1C 71 7B 5F 2D 0B D6 3E 3D 7D DC 1F 6E FE 4B 88 91 B4 54 6A 7E D5 5B 7D 30 3F 3C 0D BA 7A 2F 51 F1 D3
      // 02 03 01 00 01
      // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

      if(Data.Length > 0)
      {
        Challenge = Data.Range(0, 4);

        Data1 = Data.Range(4, 2);
        Data2 = Data.Range(8, 2);
        Data3 = Data.Range(12, 17);
        Data4 = Data.Range(31, 3);
        Data5 = Data.Range(36, 3);
        Data6 = Data.Range(41, 256);
        Data7 = Data.Range(Data.Length - 5, 5);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdExtChallengeRequest(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, byte[] challenge)
    {
      throw new NotImplementedException();
    }

    public byte[] Challenge { get; private set; }

    public byte[] Data1 { get; private set; }
    public byte[] Data2 { get; private set; }
    public byte[] Data3 { get; private set; }
    public byte[] Data4 { get; private set; }
    public byte[] Data5 { get; private set; }
    public byte[] Data6 { get; private set; }
    public byte[] Data7 { get; private set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Challenge: 0x: {1:l}", GetType().Name, BitConverter.ToString(Challenge).Replace("-", " "));
    }
  }
}
