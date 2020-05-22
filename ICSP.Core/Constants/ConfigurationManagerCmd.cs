namespace ICSP.Core.Constants
{
  /// <summary>
  /// Configuration Manager Messages
  /// </summary>
  public static class ConfigurationManagerCmd
  {
    /// <summary>
    /// Sets the device system number of the receiving device. 
    /// (Master->Device, Master->Master) 
    /// </summary>
    public const int SetDeviceNumber = 0x0201;

    /// <summary>
    /// Enables/Disables identify mode for the device and establishes the new device number.
    /// (Master->Device)
    /// </summary>
    public const int SetIdentifyModeAddress = 0x0202;

    /// <summary>
    /// Sets the 16-byte serial number of the device.
    /// </summary>
    public const int SetSerialNumber = 0x0203;

    /// <summary>
    /// Contains file transfer information
    /// </summary>
    public const int FileTransfer = 0x0204;

    /// <summary>
    /// Requests the list of IP address the device will attempt to contact.
    /// </summary>
    public const int RequestIpAddressList = 0x0205;

    /// <summary>
    /// List of IP address
    /// </summary>
    public const int IpAddressList = 0x0206;

    /// <summary>
    ///  Add the specified IP address to the contact list.
    /// </summary>
    public const int AddIpAddress = 0x0207;

    /// <summary>
    /// Delete an specified IP address from the contact list.
    /// </summary>
    public const int DeleteIpAddress = 0x0208;

    /// <summary>
    /// Sets the DNS IP address list and domain name.
    /// </summary>
    public const int SetDnsIpAddresses = 0x0209;

    /// <summary>
    /// Requests the DNS IP Addresses address list and domain ill.
    /// </summary>
    public const int RequestDnsIpAddresses = 0x020A;

    /// <summary>
    /// Gets the DNS IP address list and domain name.
    /// </summary>
    public const int GetDnsIpAddresses = 0x020B;

    /// <summary>
    /// Sets the IP address, subnet mask, and gateway.
    /// </summary>
    public const int SetEthernetIPAddress = 0x020C;

    /// <summary>
    /// Requests the Ethernet interface's IP address, Subnet mask, and gateway.
    /// </summary>
    public const int RequestEthernetIpAddress = 0x020D;

    /// <summary>
    /// Response to Request Ethernet IPAddress.
    /// </summary>
    public const int GetEthernetIpAddress = 0x020E;

    /// <summary>
    /// Sets the Time and Date.
    /// </summary>
    public const int SetDateTime = 0x020F;

    /// <summary>
    /// Requests the current Time and Date.
    /// </summary>
    public const int RequestDateTime = 0x0210;

    /// <summary>
    /// Gets the current Time and Date.
    /// </summary>
    public const int GetDateTime = 0x0211;

    /// <summary>
    /// Response to Identify Mode / Address.
    /// </summary>
    public const int IdentifyModeAddressResponse = 0x0282;

    /// <summary>
    /// Restart device or subset of device.
    /// </summary>
    public const int Restart = 0x0212;

    /// <summary>
    /// For some messages, confirms successful completion or failure of a message.
    /// </summary>
    public const int CompletionCode = 0x0213;

    // Reserved message range: OxO2EO - OxO2FF
  }
}