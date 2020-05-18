namespace ICSP.Constants
{
  /// <summary>
  /// Device Manager Messages (Table A)
  /// </summary>
  public static class DeviceManagerCmd
  {
    /// <summary>
    /// Confirmation of message receipt.
    /// </summary>
    public const int Ack = 0x0001;                        // 1

    /// <summary>
    /// Indicates failed delivery of a message.
    /// </summary>
    public const int Nak = 0x0002;                        // 2

    /// <summary>
    /// Input channel turned ON status message (PUSH)
    /// (Device->Master)
    /// </summary>
    public const int InputChannelOnStatus = 0x0084;       // 132 Push

    /// <summary>
    /// Input channel turned OFF status message (RELEASE)
    /// (Device->Master)
    /// </summary>
    public const int InputChannelOffStatus = 0x0085;      // 133 Release

    /// <summary>
    /// Output turn ON message.
    /// If device does not Support channels, then message is only ACKed.
    /// (Master->Device)
    /// </summary>
    public const int OutputChannelOn = 0x0006;            // 6

    /// <summary>
    /// Output turned ON status message.
    /// (Device->Master)
    /// </summary>
    public const int OutputChannelOnStatus = 0x0086;      // 134

    /// <summary>
    /// Output turn OFF message.
    /// If device does not support channels, then message is only ACKed.
    /// (Master->Device)
    /// </summary>
    public const int OutputChannelOff = 0x0007;           // 7

    /// <summary>
    /// Output turned OFF status message.
    /// (Device->Master)
    /// </summary>
    public const int OutputChannelOffStatus = 0x0087;     // 135

    /// <summary>
    /// Input and Output channel turned ON message.
    /// (Device->Master)
    /// </summary>
    public const int InputOutputChannelOn = 0x0088;       // 136

    /// <summary>
    /// Input and Output channel turned OFF status message.
    /// (Device->Master)
    /// </summary>
    public const int InputOutputChannelOff = 0x0089;      // 137

    /// <summary>
    /// Indicates the feedback state the master is maintaining.
    /// Only generated for Diagnostic purposes.
    /// </summary>
    public const int FeedbackChannelOn = 0x0018;          // 24

    /// <summary>
    /// Indicates the feedback state the master is maintaining.
    /// Only generated for Diagnostic purposes.
    /// </summary>
    public const int FeedbackChannelOff = 0x0019;         // 25

    /// <summary>
    /// A level value changed.
    /// If device does not Support levels, then message is only ACKed.
    /// (Master->Device)
    /// </summary>
    public const int LevelValueMasterDev = 0x000A;        // 10

    /// <summary>
    /// A level value changed.
    /// (Device->Master)
    /// </summary>
    public const int LevelValueDevMaster = 0x008A;        // 138

    /// <summary>
    /// Used to transfer a String
    /// (Master->Device)
    /// </summary>
    public const int StringMasterDev = 0x000B;            // 11

    /// <summary>
    /// Used to transfer a String
    /// (Device->Master)
    /// </summary>
    public const int StringDevMaster = 0x008B;            // 139

    /// <summary>
    /// Used to transfer a Command
    /// (Master->Device)
    /// </summary>
    public const int CommandMasterDev = 0x000C;           // 12

    /// <summary>
    /// Used to transfer a Command
    /// (Device->Master)
    /// </summary>
    public const int CommandDevMaster = 0x008C;           // 140

    /// <summary>
    /// Requests the status of specified level value.
    /// If the level does not exist, the response message is ACK.
    /// (Master->Master) 
    /// </summary>
    public const int RequestLevelValue = 0x000E;          // 14

    /// <summary>
    /// Requests the status of specified Output channel or ALL Output channels that are ON (if channel = O). 
    /// If the request is for all channels and none are on then response is ACK.
    /// (Master->Master)
    /// </summary>
    public const int RequestOutputChannelStatus = 0x000F; // 15

    /// <summary>
    /// Request number of ports used by device.
    /// Assumed 1 if no response.
    /// (Master->Master)
    /// </summary>
    public const int RequestPortCount = 0x0010;           // 16

    /// <summary>
    /// Number of ports used device.
    /// Sent upon reporting by a device if it has more than one Port.
    /// Sent by a master as a response to Request Port Count.
    /// (Device->Master)
    /// (Master->Master)
    /// </summary>
    public const int PortCountBy = 0x0090;                // 144

    /// <summary>
    /// Request number of output channels used by specified port on device.
    /// Assumes 256/port if no response.
    /// (Master->Master)
    /// </summary>
    public const int RequestOutputChannelCount = 0x0011;  // 17

    /// <summary>
    /// Number of channels used by specified port.
    /// Sent upon reporting by a device/port if it has more than 256 channels.
    /// Sent by a master as a response to Request Output Channel Count.
    /// (Device->Master)
    /// (Master->Master)
    /// </summary>
    public const int OutputChannelCount = 0x0091;         // 145

    /// <summary>
    /// Request number of levels used by specified port.
    /// Assumes 8 port if no response.
    /// (Master->Master)
    /// </summary>
    public const int RequestLevelCount = 0x0012;          // 18

    /// <summary>
    /// Number of levels used by specified port.
    /// Sent upon reporting by a device/port if it has more than 8 levels.
    /// Sent by a master as a response to Request Level Count.
    /// (Device->Master)
    /// (Master->Master)
    /// </summary>
    public const int LevelCount = 0x0092;                 // 146

    /// <summary>
    /// Request number ofbytes Supported by device/port for a single SEND_STRING.
    /// Assume 64 bytes if no response.
    /// (Master->Master)
    /// </summary>
    public const int RequestStringSize = 0x0013;          // 19

    /// <summary>
    /// Number of bytes, string Supported by device.
    /// Sent upon reporting by a device/port if it supports than 64 byte strings.
    /// Sent by a master as a response to Request String Size.
    /// (Device->Master)
    /// (Master->Master)
    /// </summary>
    public const int StringSize = 0x0093;                 // 147

    /// <summary>
    /// Request number of bytes Supported by device/port for a single SEND_COMMAND.
    /// Assume 64 bytes if no response.
    /// (Master->Master)
    /// </summary>
    public const int RequestCommandSize = 0x0014;         // 20

    /// <summary>
    /// Number of bytes command Supported by device.
    /// Sent upon reporting by a device/port if it supports than 64 byte commands.
    /// Sent by a master as a response to Request Command Size.
    /// (Device->Master)
    /// (Master->Master)
    /// </summary>
    public const int CommandSize = 0x0094;                // 148

    /// <summary>
    /// Requests the data types Supported by a level.
    /// Assume BYTE (Type Ox10) if no response 
    /// (Master->Master)
    /// </summary>
    public const int RequestLevelSize = 0x0015;           // 21

    /// <summary>
    /// Highest data type Supported for a level.
    /// Sent upon reporting by a device if it supports more than BYTE (Type 0x10) data types.
    /// Sent by a master as a response to Request Level Size.
    /// (Device->Master)
    /// (Master->Master)
    /// </summary>
    public const int LevelSize = 0x0095;                  // 149

    /// <summary>
    /// Request status of the device and or Port.
    /// (Master->Master)
    /// </summary>
    public const int RequestStatus = 0x0016;              // 22

    /// <summary>
    /// Sent by device to update master of Status.
    /// Sent by master as a response to Request Status.
    /// (Device->Master)
    /// (Master->Master)
    /// </summary>
    public const int Status = 0x0096;                     // 150

    /// <summary>
    /// Type of device, version, etc including Sub devices.
    /// (Master->Device)
    /// </summary>
    public const int RequestDeviceInfo = 0x0017;          // 23

    /// <summary>
    /// Response to RequestDeviceInfo.
    /// (Device->Master)
    /// </summary>
    public const int DeviceInfo = 0x0097;                 // 151

    /// <summary>
    /// Indicates that all previously requested device Info has been transferred.
    /// (Master->Device)
    /// </summary>
    public const int DeviceInfoEOT = 0x0098;              // 152

    /// <summary>
    /// Request the status of a system master.
    /// (Device->Master)
    /// (Master->Master)
    /// </summary>
    public const int RequestMasterStatus = 0x00A1;        // 161

    /// <summary>
    /// Master status. Used to indicate various states of a Master.
    /// (Master->Device)
    /// (Master->Master)
    /// </summary>
    public const int MasterStatus = 0x0021;               // 33
  }
}