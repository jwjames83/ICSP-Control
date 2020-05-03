using System.IO;

using ICSP.Logging;

using static ICSP.Constants.ConfigurationManagerCmd;
using static ICSP.Constants.ConnectionManagerCmd;
using static ICSP.Constants.DeviceManagerCmd;
using static ICSP.Constants.DiagnosticManagerCmd;
using static ICSP.Extensions.ArrayExtensions;

namespace ICSP
{
  public class ICSPMsg
  {
    private static ushort mMsgId;

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

    protected ICSPMsg(ICSPMsgData data)
    {
      RawData = data.RawData;

      Protocol = data.Protocol;

      Length = data.Length;

      Flag = data.Flag;

      Dest = data.Dest;

      Source = data.Source;

      Hop = data.Hop;

      ID = data.ID;

      Command = data.Command;

      // Data
      Data = data.Data;

      Checksum = data.Checksum;

      // Checksum Calculated
      var lChecksum = 0;

      for(int i = 0; i < RawData?.Length - 1; i++)
        lChecksum += data.RawData[i];

      ChecksumCalculated = (byte)(lChecksum % 256);
    }

    #endregion

    #region Serialize

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
      Length = 0;
      Flag = flag;

      Dest = dest;
      Source = source;

      Hop = hop;

      if(id > 0)
        ID = id;
      else
        ID = ++mMsgId;

      Command = command;

      Data = data;

      using(var lStream = new MemoryStream())
      {
        lStream.Write(Int16To8Bit(Protocol), 0, 1);

        // Length: Calculate later
        lStream.Write(Int16ToBigEndian(0), 0, 2);

        lStream.Write(Int16ToBigEndian(Flag), 0, 2);

        lStream.Write(Dest.GetBytesSDP(), 0, 6);

        lStream.Write(Source.GetBytesSDP(), 0, 6);

        lStream.Write(Int16To8Bit(Hop), 0, 1);

        lStream.Write(Int16ToBigEndian(ID), 0, 2);

        lStream.Write(Int16ToBigEndian(Command), 0, 2);

        if(Data != null)
          lStream.Write(Data, 0, Data.Length);

        // Checksum: Calculate later
        lStream.Write(Int16To8Bit(0), 0, 1);

        RawData = lStream.ToArray();
      }

      Length = (ushort)(RawData.Length - 4);

      var lLength = Int16ToBigEndian(Length);

      RawData[1] = lLength[0];
      RawData[2] = lLength[1];

      // Checksum
      var lChecksum = 0;

      for(int i = 0; i < RawData.Length - 1; i++)
        lChecksum += RawData[i];

      lChecksum = lChecksum % 256; // 4214 % 256 = 0x76 (118)

      Checksum = (byte)lChecksum;

      // Checksum
      RawData[RawData.Length - 1] = (byte)Checksum;

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
    private byte Protocol { get; set; }

    /// <summary>
    /// Length of data field.<br/>
    /// Indicates the total number of bytes in the data portion of the packet.
    /// </summary>
    private ushort Length { get; set; }

    /// <summary>
    /// Flag (Version, Type)<br/>
    /// Can be one of two types of flags.<br/>
    /// One is a broadcast flag. The broadcast flag will send a broadcast message to all devices on a network.<br/>
    /// A newbie flag is placed when a device is added to the network.<br/>
    /// This will then cause a response from the master indicating that it received the message from the newbie device.
    /// </summary>
    private ushort Flag { get; set; }

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
    private byte Hop { get; set; }

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
    private byte[] Data { get; set; }

    /// <summary>
    /// Checksum (Sum of Bytes % 256)
    /// </summary>
    public byte Checksum { get; private set; }

    /// <summary>
    /// Calculated Checksum (Sum of Bytes % 256)
    /// </summary>
    public byte ChecksumCalculated { get; private set; }

    public bool LogStripline { get; private set; }

    #endregion

    public virtual void WriteLog()
    {
      WriteLog(true);
    }

    public virtual void WriteLog(bool last)
    {
      Logger.LogDebug(false, "-----------------------------------------------------------");

      var lName = nameof(ICSPMsg);

      Logger.LogDebug(false, "{0} Type     : {1}", lName, GetType().Name);
      Logger.LogDebug(false, "{0} Protocol : {1}", lName, Protocol);
      Logger.LogDebug(false, "{0} Length   : {1}", lName, Length);
      Logger.LogDebug(false, "{0} Flag     : {1}", lName, Flag);
      Logger.LogDebug(false, "{0} Dest     : {1}", lName, Dest);
      Logger.LogDebug(false, "{0} Source   : {1}", lName, Source);
      Logger.LogDebug(false, "{0} Hop      : {1}", lName, Hop);
      Logger.LogDebug(false, "{0} MessageId: 0x{1:X4}", lName, ID);
      Logger.LogDebug(false, "{0} Command  : 0x{1:X4} ({2})", lName, Command, GetFrindlyName(Command));

      if(Checksum != ChecksumCalculated)
        Logger.LogDebug(false, "{0} Checksum : 0x{1:X4} (Invalid => Calculated: 0x{2:X2})", lName, Checksum, ChecksumCalculated);
      else
        Logger.LogDebug(false, "{0} Checksum : 0x{1:X4} (Valid)", lName, Checksum);

      WriteLogExtended();

      if(last)
        Logger.LogDebug(false, "-----------------------------------------------------------");
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
      }

      return "Unknown";
    }
  }
}
