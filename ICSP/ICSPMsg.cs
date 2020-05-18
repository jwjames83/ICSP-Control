using System;
using System.IO;

using ICSP.Logging;

using static ICSP.Constants.ConfigurationManagerCmd;
using static ICSP.Constants.ConnectionManagerCmd;
using static ICSP.Constants.DeviceManagerCmd;
using static ICSP.Constants.DiagnosticManagerCmd;
using static ICSP.Extensions.ArrayExtensions;

namespace ICSP
{
  public abstract class ICSPMsg
  {
    // Minimum 23 Bytes
    // ---------------------------------------------------------------------------------------------
    // P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
    // ---------------------------------------------------------------------------------------------
    // 02 | 00 1B | 02 10 | 00 06 27 11 00 00 | 00 06 7D 03 00 00 | FF | 4B 60 | 02 04 | ...    | C1

    public const int PacketLengthMin = 23;

    private static ushort MsgId;

    public const int DefaultHop = 0xFF;

    public const int DefaultFlag = 0x0200;

    public const int FlagFileTransfer = 0x0208; // Not Documented: Found by Wireshark

    /// <summary>
    /// If set then messag is a broadcast message
    /// </summary>
    public const int FlagBroadcast = 0x01;

    /// <summary>
    /// If set then receiver should reply to message.
    /// A new Master or non-configured device connecting to de system uses this.
    /// </summary>
    public const int FlagNewbee = 0x02;

    #region Constructors

    protected ICSPMsg()
    {
      Protocol = 0x02;

      Flag = DefaultFlag;

      Hop = 0xFF;
    }

    public ICSPMsg(byte[] bytes)
    {
      RawData = bytes;

      Protocol = bytes[0];

      DataLength = bytes.GetBigEndianInt16(1);

      Flag = bytes.GetBigEndianInt16(3);

      Dest = AmxDevice.FromSDP(bytes.Range(5, 6));

      Source = AmxDevice.FromSDP(bytes.Range(11, 6));

      Hop = bytes[17];

      ID = bytes.GetBigEndianInt16(18);

      Command = bytes.GetBigEndianInt16(20);

      // Data
      Data = bytes.Range(22, bytes.Length - 22 - 1);

      Checksum = bytes[DataLength + 3];
    }

    #endregion

    #region Serialize

    public abstract ICSPMsg FromData(byte[] bytes);

    protected ICSPMsg Serialize(AmxDevice dest, AmxDevice source, ushort command, byte[] data)
    {
      return Serialize(DefaultFlag, dest, source, DefaultHop, 0, command, data);
    }

    protected ICSPMsg Serialize(AmxDevice dest, AmxDevice source, ushort id, ushort command, byte[] data)
    {
      return Serialize(DefaultFlag, dest, source, DefaultHop, id, command, data);
    }

    protected ICSPMsg Serialize(ushort flag, AmxDevice dest, AmxDevice source, ushort id, ushort command, byte[] data)
    {
      return Serialize(flag, dest, source, DefaultHop, id, command, data);
    }

    protected ICSPMsg Serialize(AmxDevice dest, AmxDevice source, byte hop, ushort id, ushort command, byte[] data)
    {
      return Serialize(DefaultFlag, dest, source, hop, id, command, data);
    }

    protected ICSPMsg Serialize(ushort flag, AmxDevice dest, AmxDevice source, byte hop, ushort id, ushort command, byte[] data)
    {
      Protocol = 0x02;
      
      DataLength = (ushort)(PacketLengthMin  + (data?.Length ?? 0) - 4);
      
      Flag = flag;

      Dest = dest;

      Source = source;

      Hop = hop;

      if(id > 0)
        ID = id;
      else
        ID = ++MsgId;

      Command = command;

      Data = data;

      RawData = new byte[DataLength + 4];

      RawData[00] = Protocol;

      RawData[01] = (byte)(DataLength >> 8);
      RawData[02] = (byte)(DataLength);

      RawData[03] = (byte)(Flag >> 8);
      RawData[04] = (byte)(Flag);

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

      RawData[17] = Hop;

      RawData[18] = (byte)(ID >> 8);
      RawData[19] = (byte)(ID);

      RawData[20] = (byte)(Command >> 8);
      RawData[21] = (byte)(Command);

      if(Data != null)
        Array.Copy(Data, 0, RawData, 22, Data.Length);

      byte lCs = 0;

      unchecked // Let overflow occur without exceptions
      {
        foreach(byte b in RawData)
          lCs += b;
      }

      // Checksum
      RawData[RawData.Length - 1] = Checksum = lCs;

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
    public byte Protocol { get; set; }

    /// <summary>
    /// Length of data field.<br/>
    /// Indicates the total number of bytes in the data portion of the packet.
    /// </summary>
    public ushort DataLength { get; set; }

    /// <summary>
    /// Flag (Version, Type)<br/>
    /// Can be one of two types of flags.<br/>
    /// One is a broadcast flag. The broadcast flag will send a broadcast message to all devices on a network.<br/>
    /// A newbie flag is placed when a device is added to the network.<br/>
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
    public AmxDevice Dest { get; private set; }

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
    public AmxDevice Source { get; private set; }

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
    public ushort ID { get; protected set; }

    /// <summary>
    /// Message command field and message data represent the actual message being sent in the packet.<br/>
    /// Each packet is decoded by reading the message field and performing the appropriate functions.<br/>
    /// Some commands are designed for communication between a device manager located in the master and <br/>
    /// other ones are intended for communication with the connection manager located in the master.
    /// </summary>
    public ushort Command { get; private set; }

    /// <summary>
    /// Message Data
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Checksum (Sum of Bytes % 256)
    /// </summary>
    public byte Checksum { get; private set; }

    /*
    /// <summary>
    /// Calculated Checksum (Sum of Bytes % 256)
    /// </summary>
    public byte ChecksumCalculated { get; private set; }
    */

    public bool LogStripline { get; private set; }

    #endregion

    public virtual void WriteLogVerbose()
    {
      Logger.LogVerbose(false, "----------------------------------------------------------------");

      var lName = nameof(ICSPMsg);

      Logger.LogVerbose(false, "{0:l} Type     : {1:l}", lName, GetType().Name);
      Logger.LogVerbose(false, "{0:l} Protocol : {1}", lName, Protocol);
      Logger.LogVerbose(false, "{0:l} Length   : {1}", lName, DataLength);
      Logger.LogVerbose(false, "{0:l} Flag     : {1}", lName, Flag);
      Logger.LogVerbose(false, "{0:l} Dest     : {1:l}", lName, Dest);
      Logger.LogVerbose(false, "{0:l} Source   : {1:l}", lName, Source);
      Logger.LogVerbose(false, "{0:l} Hop      : {1}", lName, Hop);
      Logger.LogVerbose(false, "{0:l} MessageId: 0x{1:X4}", lName, ID);
      Logger.LogVerbose(false, "{0:l} Command  : 0x{1:X4} ({2:l})", lName, Command, GetFrindlyName(Command));
      Logger.LogVerbose(false, "{0:l} Checksum : 0x{1:X2}", lName, Checksum);

      WriteLogExtended();
    }

    protected virtual void WriteLogExtended()
    {
    }

    public static string GetFrindlyName(int command)
    {
      switch(command)
      {
        // ====================================================================
        // Device Manager Messages (Table A)
        // ====================================================================

        case Ack: return "Ack";
        case Nak: return "Nak";

        case InputChannelOnStatus: return "Input Channel ON Status";
        case InputChannelOffStatus: return "Input Channel OFF Status";

        case OutputChannelOn: return "Output Channel ON";
        case OutputChannelOnStatus: return "Output Channel ON Status";

        case OutputChannelOff: return "Output Channel OFF";
        case OutputChannelOffStatus: return "Output Channel OFF Status";

        case InputOutputChannelOn: return "Input OutputC hannel ON";
        case InputOutputChannelOff: return "Inpu tOutput Channel OFF";

        case FeedbackChannelOn: return "Feedback Channel ON";
        case FeedbackChannelOff: return "Feedback Channel OFF";

        case LevelValueMasterDev: return "Level Value Master->Dev";
        case LevelValueDevMaster: return "Level Value Dev->Master";

        case StringMasterDev: return "String Master->Dev";
        case StringDevMaster: return "String Dev->Master";

        case CommandMasterDev: return "Command Master->Dev";
        case CommandDevMaster: return "Command Dev-Master";

        case RequestLevelValue: return "Request Level Value";
        case RequestOutputChannelStatus: return "Request Output Channel Status";

        case RequestPortCount: return "Request Port Count";
        case PortCountBy: return "Port Count by";

        case RequestOutputChannelCount: return "Request Output Channel Count";
        case OutputChannelCount: return "Output Channel Count";

        case RequestLevelCount: return "Request Level Count";
        case LevelCount: return "Level Count";

        case RequestStringSize: return "Request String Size";
        case StringSize: return "String Size";

        case RequestCommandSize: return "Request Command Size";
        case CommandSize: return "Command Size";

        case RequestLevelSize: return "Request Level Size";
        case LevelSize: return "Level Size";

        case RequestStatus: return "Request Status";
        case Status: return "Status";

        case RequestDeviceInfo: return "Request Device Info";
        case DeviceInfo: return "Device Info";
        case DeviceInfoEOT: return "Device Info EOT";

        case RequestMasterStatus: return "Request Master Status";
        case MasterStatus: return "Master Status";

        // ====================================================================
        // Connection Manager Messages (Table B)
        // ====================================================================

        case PingRequest: return "Ping Request";
        case PingResponse: return "Ping Response";

        case BlinkMessage: return "Blink Message";
        case BlinkRequest: return "Blink Request";

        case DynamicDeviceAddressResponse: return "Dynamic Device Address Response";
        case DynamicDeviceAddressRequest: return "Dynamic Device Address Request";

        case PassThroughRequests: return "Pass Through Requests";
        case NotificationRequest: return "Notification Request";

        // ====================================================================
        // Configuration Manager Messages
        // ====================================================================

        case SetDeviceNumber: return "SetDevice Number";

        case SetIdentifyModeAddress: return "Set Identify Mode/Address";

        case SetSerialNumber: return "Set SerialNumber";

        case FileTransfer: return "File Transfer";

        case RequestIpAddressList: return "Request IP Address List";
        case IpAddressList: return "IP Address List";
        case AddIpAddress: return "Add IP Address";
        case DeleteIpAddress: return "Delete IP Address";

        case SetDnsIpAddresses: return "Set DNS IP Addresses";
        case RequestDnsIpAddresses: return "Request DNS IP Addresses";
        case GetDnsIpAddresses: return "Get DNS IP Addresses";

        case SetEthernetIPAddress: return "Set Ethernet IPAddress";
        case RequestEthernetIpAddress: return "Request Ethernet IP Address";
        case GetEthernetIpAddress: return "Get Ethernet IP Address";

        case SetDateTime: return "Set Time & Date";
        case RequestDateTime: return "Request Time & Date";
        case GetDateTime: return "Get Time & Date";

        case IdentifyModeAddressResponse: return "Identify Mode/Address Response";

        case Restart: return "Restart";

        case CompletionCode: return "Completion Code";

        // ====================================================================
        // Diagnostic Manager Messages
        // ====================================================================

        //  Diagnostic Manager Messages
        case InternalDiagnosticString: return "Internal Diagnostic String";
        case RequestDiagnosticInformation: return "Request Diagnostic Information";

        case RequestDevicesOnline: return "Request Devices Online";
        case RequestDevicesOnlineEOT: return "Request Devices Online EOT";

        case RequestDeviceStatus: return "Request Device Status";
        case RequestDeviceStatusEOT: return "Request Device Status EOT";

        case RequestAsynchronousNotificationList: return "Request Asynchronous NotificationL ist";
        case AsynchronousNotificationList: return "Asynchronous Notification List";

        case AddModifyAsynchronousNotificationList: return "Add Modify Asynchronous Notification List";
        case DeleteAsynchronousNotificationList: return "Delete Asynchronous Notification List";

        case RequestDiscoveryInfo: return "Request ProgramInfo";
        case DiscoveryInfo: return "ProgramInfo";
        case DiscoveryInfoEOT: return "ProgramInfo EOT";

        default:
          break;
      }

      return "Unknown";
    }
  }
}
