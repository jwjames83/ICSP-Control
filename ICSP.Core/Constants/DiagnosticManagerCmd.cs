namespace ICSP.Core.Constants
{
  /// <summary>
  ///  Diagnostic Manager Messages
  /// </summary>
  public static class DiagnosticManagerCmd
  {
    /// <summary>
    ///  Used to display/convey a diagnostic message.
    ///  It includes a severity level, module ID, and a string. 
    ///  These messages are generated internally within the master.
    /// </summary>
    public const int InternalDiagnosticString = 0x0101;

    /// <summary>
    /// Registers the sending device with the diagnostic manager 
    /// such that diagnostic information be sent the source device.
    /// </summary>
    public const int RequestDiagnosticInformation = 0x0102;

    /// <summary>
    /// Master responds with Device Info message(s) for each device 
    /// currently online and the number of ports for each device.
    /// </summary>
    public const int RequestDevicesOnline = 0x0103;

    /// <summary>
    /// Indicates that all online device information has been sent.
    /// </summary>
    public const int RequestDevicesOnlineEOT = 0x0109;

    /// <summary>
    /// Master responds with a variety of messages that indicate the current state of the device.
    /// </summary>
    public const int RequestDeviceStatus = 0x0104;

    /// <summary>
    /// Indicates that all Device Status information has been sent.
    /// </summary>
    public const int RequestDeviceStatusEOT = 0x010A;

    /// <summary>
    /// Requests the entire list of device/ports currently in the Master's asynchronous notification list.
    /// </summary>
    public const int RequestAsynchronousNotificationList = 0x0105;

    /// <summary>
    /// Contains a single entry from the Master's asynchronous notification list.
    /// </summary>
    public const int AsynchronousNotificationList = 0x0106;

    /// <summary>
    /// Adds or modifies an entry in the Master's asynchronous notification list.
    /// </summary>
    public const int AddModifyAsynchronousNotificationList = 0x0107;

    /// <summary>
    /// Delete one or all entries in the Master's asynchronous notification list.
    /// </summary>
    public const int DeleteAsynchronousNotificationList = 0x0108;

    /// <summary>
    /// Unknown: (Probably IP Device Discovery)
    /// </summary>
    #warning Not confirmed 
    public const int RequestDiscoveryInfo = 0x010B;

    /// <summary>
    /// Unknown: (Probably IP Device Discovery -> Info)
    /// </summary>
    #warning Not confirmed
    public const int DiscoveryInfo = 0x010C;

    /// <summary>
    /// Unknown: (Probably IP Device Discovery -> EOT)
    /// </summary>
    #warning Not confirmed
    public const int DiscoveryInfoEOT = 0x010D;
  }
}