using System;

using ICSP.Core.Cryptography;
using ICSP.Core.Logging;

using static ICSP.Core.Extensions.ArrayExtensions;

namespace ICSP.Core
{
  public class ICSPEncryptedMsg
  {
    // Minimum [] Bytes
    // Protocol 2 (Default)
    // ---------------------------------------------------------------------------------------------
    // P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
    // ---------------------------------------------------------------------------------------------
    // 02 | 00 1B | 02 10 | 00 06 27 11 00 00 | 00 06 7D 03 00 00 | FF | 4B 60 | 02 04 | ...    | C1

    // Protocol 4 (Encrypted) -> Encryption Type 1
    // ---------------------------------------------------------------------------------------------
    // P  | Len   | S-Sys | S-Dev | ET | Ln | Salt        | Encrypted Data | CS
    // ---------------------------------------------------------------------------------------------
    // 04 | 00 21 | 00 01 | 00 00 | 01 | 04 | 52 2a 17 f5 | ...            | 83

    // Protocol 4 (Encrypted) -> Encryption Type 2 (RC4)
    // ---------------------------------------------------------------------------------------------
    // P  | Len   | S-Sys | S-Dev | ET | Ln | Salt        | D-Sys | D-Dev | Encrypted Data | CS
    // ---------------------------------------------------------------------------------------------
    // 04 | 00 21 | 00 01 | 00 00 | 02 | 08 | 5B CF 08 88 | 00 01 | 7D 01 | ...            | D6

    //    ---------------------------------------------------------------------------------------------
    //    P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
    //    ---------------------------------------------------------------------------------------------
    // 1: 02 | 00 5b | 02 12 | 00 00 00 00 00 01 | 00 01 7d 01 00 00 | ff | 01 b7 | 00 97 | 7d 01 00 01 00 20 00 00 00 01 01 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 19 76 34 2e 34 2e 31 2e 31 36 32 36 00 4e 53 58 20 41 70 70 6c 69 63 61 74 69 6f 6e 00 41 4d 58 20 4c 4c 43 00 02 04 ac 12 23 71 | 2e
    // E: 5e | 5b 34 | 8d 51 | 13 52 22 10 34 fd | 56 08 92 4c 60 13 | 18 | 5d 27 | 04 8c | 4d 6a e3 93 31 3d 4f 8a de 03 d0 8e 10 6b 07 04 41 a9 87 f0 2a bb 54 6e 66 79 ca 49 23 24 3c bf b0 1d 6e 5b a7 0a 76 45 d2 29 fc 37 26 c6 d0 05 a4 ee 9c 3a 8b 27 3e 6c 83 5e 25 62 7c 53 22 d1 cd c9 6d 87 7b 07 87 bc | 7f

    public const int PacketLengthMin = 10;

    // ======================================================================
    // Debug stuff ... / Log
    // ======================================================================

    public static byte[] DebugChallenge = new byte[4];
    public static byte[] DebugHash = new byte[16];
    public static byte[] DebugPrivateKey = new byte[256];
    public static byte[] DebugSaltValue = new byte[4];
    public static byte[] DebugSaltKey = new byte[256];
    public static byte[] DebugDecrypted1 = new byte[256];
    public static byte[] DebugDecrypted2 = new byte[256];

    /// <summary>
    /// The first field is a protocol field, and in one embodiment, one byte size.<br/>
    /// Protocol field identifies the format of the data section of the packet with some protocol.
    /// </summary>
    public const byte ProtocolValue = 0x04;

    public const int DefaultHop = 0xFF;

    public const ICSPMsgFlag DefaultFlag = ICSPMsgFlag.Version_02;

    #region Constructors

    protected ICSPEncryptedMsg()
    {
    }

    public ICSPEncryptedMsg(byte[] bytes)
    {
      RawData = bytes;

      if(bytes[0] != ProtocolValue)
        throw new Exception($"Invalid protocol at first byte, Expect={ProtocolValue}, Value={bytes[0]}");

      DataLength = bytes.GetBigEndianInt16(1);

      Source = AmxDevice.FromSD(bytes.Range(3, 4));

      // Protocol 4 (Encrypted) -> Encryption Type 1
      // ---------------------------------------------------------------------------------------------
      // P  | Len   | S-Sys | S-Dev | ET | Ln | Salt        | Encrypted Data | CS
      // ---------------------------------------------------------------------------------------------
      // 04 | 00 21 | 00 01 | 00 00 | 01 | 04 | 52 2a 17 f5 | ...            | 83

      // Protocol 4 (Encrypted) -> Encryption Type 2 (RC4)
      // ---------------------------------------------------------------------------------------------
      // P  | Len   | S-Sys | S-Dev | ET | Ln | Salt        | D-Sys | D-Dev | Encrypted Data | CS
      // ---------------------------------------------------------------------------------------------
      // 04 | 00 21 | 00 01 | 00 00 | 02 | 08 | 5B CF 08 88 | 00 01 | 7D 01 | ...            | D6
      // 04 | 00 2D | 00 01 | 00 00 | 02 | 08 | 4C 99 59 B6 | 00 01 | 7D 01 | 04 6D 1F CC 6B DE 61 E1 A9 52 2B DC 93 16 96 8F F8 F6 81 F0 41 AA B8 61 E0 77 1E 01 E7 DF 42 | 47
      // 04 | 00 2D | 00 01 | 00 00 | 02 | 08 | 69 F1 5C 8B | 00 01 | 7D 01 | C1 36 0E FE F9 3C B1 B0 E4 27 3B C9 2B 7D F7 58 43 00 03 A5 2B 30 20 5B 5C FD 84 FD 40 A4 91 | AB

      EncryptionType = bytes[7];

      var cStr = BitConverter.ToString(bytes).Replace("-", " ");

      Console.WriteLine(cStr);

      CustomDataLength = bytes[8];

      if(CustomDataLength > 0)
      {
        CustomData = bytes.Range(9, CustomDataLength);

        // Encryption salt ...
        if(CustomDataLength >= 4)
          Salt = bytes.Range(9, 4);

        // Destination ...
        if(CustomDataLength >= 8)
          Dest = AmxDevice.FromSD(bytes.Range(13, 4));
      }

      var lEncryptedLength = DataLength + 3 - (9 + CustomDataLength);

      // Data
      EncryptedData = bytes.Range(9 + CustomDataLength, lEncryptedLength);

      Checksum = bytes[DataLength + 3];

      // ======================================================================
      // Debug stuff ... / Log
      // ======================================================================

      // Inititalize DebugSaltKey from original PrivateKey
      Array.Copy(DebugPrivateKey, 0, DebugSaltKey, 0, DebugPrivateKey.Length);

      DebugSaltValue = new byte[8];

      if(Salt == null)
        Salt = new byte[0];

      if(Salt.Length > 0)
      {
        DebugSaltValue = new byte[Salt.Length];

        Array.Copy(Salt, 0, DebugSaltValue, 0, Salt.Length);
        Array.Copy(Salt, 0, DebugSaltKey, 0, Salt.Length);
      }
    }

    #endregion

    #region Serialize

    public static ICSPEncryptedMsg FromData(byte[] bytes)
    {
      return new ICSPEncryptedMsg(bytes);
    }

    protected ICSPEncryptedMsg Serialize(AmxDevice dest, AmxDevice source, ushort command, byte[] data)
    {
      return Serialize(DefaultFlag, dest, source, DefaultHop, 0, command, data);
    }

    protected ICSPEncryptedMsg Serialize(AmxDevice dest, AmxDevice source, ushort id, ushort command, byte[] data)
    {
      return Serialize(DefaultFlag, dest, source, DefaultHop, id, command, data);
    }

    protected ICSPEncryptedMsg Serialize(ICSPMsgFlag flag, AmxDevice dest, AmxDevice source, ushort id, ushort command, byte[] data)
    {
      return Serialize(flag, dest, source, DefaultHop, id, command, data);
    }

    protected ICSPEncryptedMsg Serialize(AmxDevice dest, AmxDevice source, byte hop, ushort id, ushort command, byte[] data)
    {
      return Serialize(DefaultFlag, dest, source, hop, id, command, data);
    }

    protected ICSPEncryptedMsg Serialize(ICSPMsgFlag flag, AmxDevice dest, AmxDevice source, byte hop, ushort id, ushort command, byte[] data)
    {
      /*
      CustomDataLength = (ushort)(PacketLengthMin + (data?.Length ?? 0) - 4);

      Flag = flag;

      Dest = dest;

      Source = source;

      EncryptionType = hop;

      if(id > 0)
        ID = id;
      else
        ID = ++MsgId;

      Command = command;

      EncryptedData = data;

      RawData = new byte[CustomDataLength + 4];

      RawData[00] = Protocol;

      RawData[01] = (byte)(CustomDataLength >> 8);
      RawData[02] = (byte)(CustomDataLength);

      RawData[03] = (byte)((ushort)Flag >> 8);
      RawData[04] = (byte)((ushort)Flag);

      var lDsp = Dest.GetBytesSDP();

      RawData[05] = lDsp[0];
      RawData[06] = lDsp[1];
      RawData[07] = lDsp[2];
      RawData[08] = lDsp[3];
      RawData[09] = lDsp[4];
      RawData[10] = lDsp[5];

      lDsp = Source.GetBytesSDP();

      RawData[11] = lDsp[0];
      RawData[12] = lDsp[1];
      RawData[13] = lDsp[2];
      RawData[14] = lDsp[3];
      RawData[15] = lDsp[4];
      RawData[16] = lDsp[5];

      RawData[17] = EncryptionType;

      RawData[18] = (byte)(ID >> 8);
      RawData[19] = (byte)(ID);

      RawData[20] = (byte)(Command >> 8);
      RawData[21] = (byte)(Command);

      if(EncryptedData != null)
        Array.Copy(EncryptedData, 0, RawData, 22, EncryptedData.Length);

      byte lCs = 0;

      unchecked // Let overflow occur without exceptions
      {
        foreach(byte b in RawData)
          lCs += b;
      }

      // Checksum
      RawData[RawData.Length - 1] = Checksum = lCs;
      */

      return this;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Raw data bytes of packet
    /// </summary>
    public byte[] RawData { get; private set; }

    /// <summary>
    /// The first field is a protocol field, and in one embodiment, one byte size.<br/>
    /// Protocol field identifies the format of the data section of the packet with some protocol.
    /// </summary>
    public byte Protocol => ProtocolValue;

    /// <summary>
    /// Length of data field.<br/>
    /// Indicates the total number of bytes in the data portion of the packet.
    /// </summary>
    public ushort DataLength { get; set; }

    /// <summary>
    /// [4 Bytes: System:Device]<br/>
    /// <br/>
    /// Source system field is the number of a system where the message originates.<br/>
    /// <br/>
    /// Source device field lists the device that the message originated from.<br/>
    /// If the device number is 0, this indicates that the master of that control <br/>
    /// area network is the enunciator of the communication.<br/>
    /// <br/>
    /// Source port field lists the port where the message originated.<br/>
    /// <br/>
    /// An important aspect of addressing is the sequencing of messages.<br/>
    /// There are certain messages and circumstances under which messages must be delivered in the order intended.<br/>
    /// This requires that each device be guaranteed the correct order for delivery.<br/>
    /// However, while messages destined for a certain device must be delivered in the order intended,<br/>
    /// out of order messages are possible when destined for different devices.
    /// </summary>
    public AmxDevice Source { get; private set; }

    /// <summary>
    /// Allowed hop count field indicates how many hops can occur before the message is purged from the system.<br/>
    /// Each time a message passes through a master, the allowed hop count field is decremented by one and checked to see if it reaches Zero.<br/>
    /// Once the count reaches Zero, the master generates an error message indicating that the message has not reached the sender with an air.
    /// </summary>
    public byte EncryptionType { get; set; }

    /// <summary>
    /// Length of data field.<br/>
    /// Indicates the total number of bytes in the data portion of the packet.
    /// </summary>
    public byte CustomDataLength { get; set; }

    /// <summary>
    /// Raw data bytes of packet
    /// </summary>
    public byte[] CustomData { get; set; }

    /// <summary>
    /// Raw data bytes of packet
    /// </summary>
    public byte[] Salt { get; set; }

    /// <summary>
    /// [4 Bytes: System:Device]<br/>
    /// <br/>
    /// Destination system field allows for the addressing of the message to reach a specific system.<br/>
    /// A system is, in one embodiment, a complete control area network with a single master.<br/>
    /// Thus, message can be directed to one ofmany different control area networks.<br/>
    /// In one embodiment control system field is two bytes in size.<br/>
    /// <br/>
    /// Destination devicefield lists the  number the device that the message is being sent.<br/>
    /// The device range can be anywhere between 0 and 65,535.<br/>
    /// <br/>
    /// Destination port field lists the specific port of the device that the message is destined for.<br/>
    /// In one embodiment the protocol supports up to 65,535 ports on the device.
    /// </summary>
    public AmxDevice Dest { get; private set; }

    /// <summary>
    /// Encrypted Data
    /// </summary>
    public byte[] EncryptedData { get; set; }

    /// <summary>
    /// Checksum (Sum of Bytes % 256)
    /// </summary>
    public byte Checksum { get; private set; }

    #endregion

    public virtual void WriteLogVerbose()
    {
      Logger.LogVerbose(false, "----------------------------------------------------------------");

      var lName = nameof(ICSPMsg);

      Logger.LogVerbose(false, "{0:l} Type          : {1:l}", lName, GetType().Name);
      Logger.LogVerbose(false, "{0:l} Protocol      : {1}", lName, Protocol);
      Logger.LogVerbose(false, "{0:l} CustomDataLen : {1}", lName, CustomDataLength);
      Logger.LogVerbose(false, "{0:l} Source        : {1:l}", lName, Source);
      Logger.LogVerbose(false, "{0:l} EncryptionType: 0x{1:X2}", lName, EncryptionType);

      if(Salt?.Length > 0)
        Logger.LogVerbose(false, "{0:l} Salt          : {1:l}", lName, BitConverter.ToString(Salt).Replace("-", ""));

      if(Dest.Device > 0)
        Logger.LogVerbose(false, "{0:l} Dest          : {1:l}", lName, Dest);

      Logger.LogVerbose(false, "{0:l} Checksum      : 0x{1:X2}", lName, Checksum);

      Logger.LogVerbose(false, "--------------------------------------------------------------------------------------------");

      Logger.LogVerbose(false, "{0:l} [PrivateKey]  : 0x: {1:l}", lName, BitConverter.ToString(DebugPrivateKey).Replace("-", ""));
      Logger.LogVerbose(false, "{0:l} [SaltKey]     : 0x: {1:l}", lName, BitConverter.ToString(DebugSaltKey).Replace("-", ""));

      var lCryptoProvider = RC4.Create(DebugHash);

      DebugDecrypted1 = new byte[EncryptedData.Length];
      Array.Copy(EncryptedData, 0, DebugDecrypted1, 0, EncryptedData.Length);

      DebugDecrypted2 = new byte[EncryptedData.Length];
      Array.Copy(EncryptedData, 0, DebugDecrypted2, 0, EncryptedData.Length);

      lCryptoProvider = RC4.Create(DebugHash);
      lCryptoProvider.TransformBlock(DebugDecrypted1, DebugSaltKey);

      Logger.LogVerbose(false, "{0:l} EncryptedData : 0x: {1:l}", lName, BitConverter.ToString(EncryptedData).Replace("-", ""));

      Logger.LogVerbose(false, "{0:l} [Decrypted1]  : 0x: {1:l}", lName, BitConverter.ToString(DebugDecrypted1).Replace("-", ""));

      // Try / verify another algorithm
      var lDecryptedData = RC4Algorithm.ApplyDirect(DebugDecrypted2, DebugSaltKey);
      
      Logger.LogVerbose(false, "{0:l} [Decrypted2]  : 0x: {1:l}", lName, BitConverter.ToString(lDecryptedData).Replace("-", ""));

      Logger.LogVerbose(false, "--------------------------------------------------------------------------------------------");
    }
  }
}
