namespace ICSP
{
  public enum ExtAddressType : byte
  {
    /// <summary>
    /// Neuron-ID. Length will be 6 and address will be the 48-bit Neuron-ID of the device.
    /// </summary>
    NeuronId = 0x01,

    /// <summary>
    /// IP4-Address. Length will be 4 and address will be the 4-byte IP-Address of the device.
    /// </summary>
    IPv4Address = 0x02,

    /// <summary>
    /// AXLink Connection. Length must be 0.
    /// </summary>
    AxLink = 0x03,

    /// <summary>
    /// Reserved
    /// </summary>
    IPv4PortMac = 0x06,

    /// <summary>
    /// RS232 Connection. Length must be 0.
    /// </summary>
    RS232 = 0x13,

    /// <summary>
    /// Reserved
    /// </summary>
    IPv4PortMacIPv6 = 0x18
  }
}
