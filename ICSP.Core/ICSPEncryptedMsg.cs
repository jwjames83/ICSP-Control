using System;

using ICSP.Core.Cryptography;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core
{
  public class ICSPEncryptedMsg
  {
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

    // Minimum 14 Bytes
    public const int PacketLengthMin = 14;

    private readonly RC4 mCryptoProvider;

    /// <summary>
    /// The first field is a protocol field, and in one embodiment, one byte size.<br/>
    /// Protocol field identifies the format of the data section of the packet with some protocol.
    /// </summary>
    public const byte ProtocolValue = 0x04;

    #region Constructors

    public ICSPEncryptedMsg(RC4 cryptoProvider, byte[] bytes)
    {
      mCryptoProvider = cryptoProvider ?? throw new ArgumentNullException(nameof(cryptoProvider));

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
    }

    public ICSPEncryptedMsg(RC4 cryptoProvider, ICSPMsg msg, int mode)
    {
      mCryptoProvider = cryptoProvider ?? throw new ArgumentNullException(nameof(cryptoProvider));

      DataLength = (ushort)(PacketLengthMin + (msg?.RawData?.Length ?? 0) - 4);

      if(mode == 4)
        DataLength += 4;

      Source = msg.Source;

      if(mode == 4)
        EncryptionType = 2;
      else
        EncryptionType = 1;

      if(mode == 4)
        CustomDataLength = 8;
      else
        CustomDataLength = 4;

      Salt = new byte[4];

      new Random().NextBytes(Salt);

      Dest = msg.Dest;

      RawData = new byte[DataLength + 4];

      RawData[00] = Protocol;

      RawData[01] = (byte)(DataLength >> 8);
      RawData[02] = (byte)(DataLength);

      // Protocol 4 (Encrypted) -> Encryption Type 1
      // ---------------------------------------------------------------------------------------------
      // P  | Len   | S-Sys | S-Dev | ET | Ln | Salt        | Encrypted Data | CS
      // ---------------------------------------------------------------------------------------------
      // 04 | 00 21 | 00 01 | 00 00 | 01 | 04 | 52 2a 17 f5 | ...            | 83

      var lDsp = Source.GetBytesSDP();

      RawData[03] = lDsp[0]; // Source System
      RawData[04] = lDsp[1];
      RawData[05] = lDsp[2]; // Source Device
      RawData[06] = lDsp[3];

      RawData[07] = EncryptionType;

      RawData[08] = CustomDataLength;

      RawData[09] = Salt[0];
      RawData[10] = Salt[1];
      RawData[11] = Salt[2];
      RawData[12] = Salt[3];

      if(mode == 4)
      {
        lDsp = Dest.GetBytesSDP();

        RawData[13] = lDsp[0];
        RawData[14] = lDsp[1];
        RawData[15] = lDsp[2];
        RawData[16] = lDsp[3];
      }

      var lOffset = mode == 4 ? 17 : 13;

      // Encrypt
      if(msg?.RawData != null)
      {
        var lEncryptedData = new byte[msg.RawData.Length];

        Array.Copy(msg.RawData, 0, lEncryptedData, 0, msg.RawData.Length);

        mCryptoProvider.TransformBlock(lEncryptedData, Salt);

        Array.Copy(lEncryptedData, 0, RawData, lOffset, lEncryptedData.Length);
      }

      byte lCs = 0;

      unchecked // Let overflow occur without exceptions
      {
        foreach(byte b in RawData)
          lCs += b;
      }

      // Checksum
      RawData[RawData.Length - 1] = Checksum = lCs;
    }

    #endregion

    public byte[] GetDecryptedData()
    {
      var lDecryptedData = new byte[EncryptedData.Length];

      Array.Copy(EncryptedData, 0, lDecryptedData, 0, EncryptedData.Length);

      mCryptoProvider.TransformBlock(lDecryptedData, Salt);

      return lDecryptedData;
    }

    #region Serialize

    //protected ICSPEncryptedMsg Serialize(AmxDevice dest, AmxDevice source, ushort command, byte[] data)
    //{
    //  return Serialize(DefaultFlag, dest, source, DefaultHop, 0, command, data);
    //}

    //protected ICSPEncryptedMsg Serialize(AmxDevice dest, AmxDevice source, ushort id, ushort command, byte[] data)
    //{
    //  return Serialize(DefaultFlag, dest, source, DefaultHop, id, command, data);
    //}

    //protected ICSPEncryptedMsg Serialize(ICSPMsgFlag flag, AmxDevice dest, AmxDevice source, ushort id, ushort command, byte[] data)
    //{
    //  return Serialize(flag, dest, source, DefaultHop, id, command, data);
    //}

    //protected ICSPEncryptedMsg Serialize(AmxDevice dest, AmxDevice source, byte hop, ushort id, ushort command, byte[] data)
    //{
    //  return Serialize(DefaultFlag, dest, source, hop, id, command, data);
    //}

    //protected ICSPEncryptedMsg Serialize(ICSPMsgFlag flag, AmxDevice dest, AmxDevice source, byte hop, ushort id, ushort command, byte[] data)
    //{
    //  /*
    //  CustomDataLength = (ushort)(PacketLengthMin + (data?.Length ?? 0) - 4);

    //  Flag = flag;

    //  Dest = dest;

    //  Source = source;

    //  EncryptionType = hop;

    //  if(id > 0)
    //    ID = id;
    //  else
    //    ID = ++MsgId;

    //  Command = command;

    //  EncryptedData = data;

    //  RawData = new byte[CustomDataLength + 4];

    //  RawData[00] = Protocol;

    //  RawData[01] = (byte)(CustomDataLength >> 8);
    //  RawData[02] = (byte)(CustomDataLength);

    //  RawData[03] = (byte)((ushort)Flag >> 8);
    //  RawData[04] = (byte)((ushort)Flag);

    //  var lDsp = Dest.GetBytesSDP();

    //  RawData[05] = lDsp[0];
    //  RawData[06] = lDsp[1];
    //  RawData[07] = lDsp[2];
    //  RawData[08] = lDsp[3];
    //  RawData[09] = lDsp[4];
    //  RawData[10] = lDsp[5];

    //  lDsp = Source.GetBytesSDP();

    //  RawData[11] = lDsp[0];
    //  RawData[12] = lDsp[1];
    //  RawData[13] = lDsp[2];
    //  RawData[14] = lDsp[3];
    //  RawData[15] = lDsp[4];
    //  RawData[16] = lDsp[5];

    //  RawData[17] = EncryptionType;

    //  RawData[18] = (byte)(ID >> 8);
    //  RawData[19] = (byte)(ID);

    //  RawData[20] = (byte)(Command >> 8);
    //  RawData[21] = (byte)(Command);

    //  if(EncryptedData != null)
    //    Array.Copy(EncryptedData, 0, RawData, 22, EncryptedData.Length);

    //  byte lCs = 0;

    //  unchecked // Let overflow occur without exceptions
    //  {
    //    foreach(byte b in RawData)
    //      lCs += b;
    //  }

    //  // Checksum
    //  RawData[RawData.Length - 1] = Checksum = lCs;
    //  */

    //  return this;
    //}

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

    public byte EncryptionType { get; set; }

    public byte CustomDataLength { get; set; }

    public byte[] CustomData { get; set; }

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
    }
  }
}
