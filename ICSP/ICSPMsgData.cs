using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP
{
  public struct ICSPMsgData
  {
    public static readonly ICSPMsgData Empty;
    
    public static ICSPMsgData FromMessage(byte[] msg)
    {
      var lMsg = new ICSPMsgData();

      lMsg.RawData = msg;

      if(msg.Length >= 3)
      {
        lMsg.Protocol = msg[0];

        lMsg.Length = msg.GetBigEndianInt16(1);

        // Protocol (1), Length (2), Checksum (1)
        if(msg.Length < lMsg.Length + 4)
        {
          Logger.LogWarn("{0} => Data to Short: {1} Bytes", nameof(ICSPMsgData),  msg.Length);

          return ICSPMsgData.Empty;
        }

        lMsg.Flag = msg.GetBigEndianInt16(3);

        lMsg.Dest = AmxDevice.FromSDP(msg.Range(5, 6));
        lMsg.Source = AmxDevice.FromSDP(msg.Range(11, 6));

        lMsg.Hop = msg[17];

        lMsg.ID = msg.GetBigEndianInt16(18);

        lMsg.Command = msg.GetBigEndianInt16(20);

        // Data
        lMsg.Data = msg.Range(22, msg.Length - 22 - 1);

        lMsg.Checksum = msg[lMsg.Length + 3];
      }

      return lMsg;
    }

    /// <summary>
    /// Raw data bytes of packet
    /// </summary>
    public byte[] RawData { get; set; }

    /// <summary>
    /// The first field is a protocol field, and in one embodiment, one byte size.<br/>
    /// Protocol field identifies the format of the data section of the packet with some protocol.
    /// </summary>
    public byte Protocol { get; set; }

    /// <summary>
    /// Length of data field.<br/>
    /// Indicates the total number of bytes in the data portion of the packet.
    /// </summary>
    public ushort Length { get; set; }

    /// <summary>
    /// Flag (Version, Type)<br/>
    /// Can be one of two types of flags. <br/>
    /// One is a broadcast flag. The broadcast flag will send a broadcast message to all devices on a network.<br/>
    /// A newbie flag is placed in flag field 674 when a device is added to the network.<br/>
    /// This will then cause a response from the master indicating that it received the message from the newbie device.
    /// </summary>
    public ushort Flag { get; set; }

    /// <summary>
    /// [6 Bytes: System:Device:Port]<br/>
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
    public AmxDevice Dest { get; set; }

    /// <summary>
    /// [6 Bytes: System:Device:Port]<br/>
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
    public AmxDevice Source { get; set; }

    /// <summary>
    /// Allowed hop count field indicates how many hops can occur before the message is purged from the system.<br/>
    /// Each time a message passes through a master, the allowed hop count field is decremented by one and checked to see if it reaches Zero.<br/>
    /// Once the count reaches Zero, the master generates an error message indicating that the message has not reached the sender with an air.
    /// </summary>
    public byte Hop { get; set; }

    /// <summary>
    /// Message I.D. field contains the unique identification number for a message.<br/>
    /// This message I.D. is used by low level communication algorithms to correlate in the original message with its acknowledge and response.
    /// </summary>
    public ushort ID { get; set; }

    /// <summary>
    /// Message command field and message data represent the actual message being sent in the packet.<br/>
    /// Each packet is decoded by reading the message field and performing the appropriate functions.<br/>
    /// Some commands are designed for communication between a device manager located in the master and <br/>
    /// other ones are intended for communication with the connection manager located in the master.
    /// </summary>
    public ushort Command { get; set; }
    
    /// <summary>
    /// Message Data
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Checksum (Sum of Bytes % 256)
    /// </summary>
    public byte Checksum { get; set; }
  }
}
