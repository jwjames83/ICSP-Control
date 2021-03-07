namespace ICSP.Core
{
  public enum ExtAddressType : byte
  {
    /// <summary>
    /// No Address
    /// </summary>
    NoAddress = 0, 

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
    /// MAC-Address
    /// </summary>
    MacAddress = 0x04,

    /// <summary>
    /// IPv4 and Port
    /// </summary>
    IPv4Port = 0x05,

    /// <summary>
    /// IPv4, Port and MAC-Address
    /// </summary>
    IPv4PortMac = 0x06,

    /// <summary>
    /// Neuron SubNode ICSP
    /// </summary>
    NeuronSubNode_ICSP = 0x10,

    /// <summary>
    /// Neuron SubNode PL
    /// </summary>
    NeuronSubNode_PL = 0x11,

    /// <summary>
    /// IP Socket Address
    /// </summary>
    IPSocketAddress = 0x12,

    /// <summary>
    /// RS232 Connection. Length must be 0.
    /// </summary>
    RS232 = 0x13,

    /// <summary>
    /// Local
    /// </summary>
    Local = 0x14,

    /// <summary>
    /// IPv4 to IPv6 Address
    /// </summary>
    IPv4IPv6Address = 0x16,

    /// <summary>
    /// IPv4 and Port to IPv6 Address
    /// </summary>
    IPv4PortIPv6 = 0x17,

    /// <summary>
    /// IPv4, Port and MAC-Address to IPv6 Address
    /// </summary>
    IPv4PortMacIPv6 = 0x18,
  }
}
