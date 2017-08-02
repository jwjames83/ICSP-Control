using System;
using System.Net;
using System.Net.NetworkInformation;

namespace ICSP
{
  public sealed class MessageReceivedEventArgs : EventArgs
  {
    public MessageReceivedEventArgs(ICSPMsg message)
    {
      Message = message;
    }

    public ICSPMsg Message { get; private set; }
    
    public bool Handled { get; set; }
  }

  public sealed class ChannelEventArgs : EventArgs
  {
    public ChannelEventArgs(int channelPort, int channelCode, bool enabled)
    {
      ChannelPort = channelPort;

      ChannelCode = channelCode;

      Enabled = enabled;
    }

    public int ChannelPort { get; private set; }

    public int ChannelCode { get; private set; }

    public bool Enabled { get; private set; }
  }

  public sealed class DeviceInfoEventArgs : EventArgs
  {
    public DeviceInfoEventArgs(ushort device, ushort system)
    {
      Device = device;

      System = system;
    }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort Device { get; set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort System { get; set; }

    /// <summary>
    /// 16-bit bit field. 
    /// Bit 0 - If set, this message was generated in 
    /// response to a button press while Identify mode is active.
    /// </summary>
    public ushort DataFlag { get; set; }

    /// <summary>
    /// Unsigned 8-bit value.
    /// </summary>
    public byte ObjectId { get; set; }

    /// <summary>
    /// Unsigned 8-bit value.
    /// </summary>
    public byte ParentId { get; set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// The Manufacture ID as reported in the Device Info message for ParentID = 0 and ObjectID = 0
    /// </summary>
    public ushort ManufactureId { get; set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// The Device ID as reported in the Device Info message for ParentID = 0 and ObjectID = 0
    /// </summary>
    public ushort DeviceId { get; set; }

    /// <summary>
    /// 16 bytes of data. Format not defined yet.
    /// </summary>
    public string SerialNumber { get; set; }

    /// <summary>
    ///  Unsigned 16-bit value.
    /// </summary>
    public ushort FirmwareId { get; set; }

    /// <summary>
    /// CHAR array, NULL terminated, containing a 
    /// version string. Generally, in this format: "v1.00\0"
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// CHAR array, NULL terminated, containing a model number.
    /// Generally, in this format: "NXC-232\0" (NetLinx Card-RS232)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// CHAR array, NULL terminated, containing 
    /// String the mfg.'s name. Generally, in this format: "AMX Corp/0"
    /// </summary>
    public string Manufacture { get; set; }

    /// <summary>
    /// 8-bit value. Used to indicate type of extended address to follow.
    /// </summary>
    public ExtAddressType ExtAddressType { get; set; }

    /// <summary>
    /// 8-bit value. Used to indicate length in bytes of extended address to follow
    /// </summary>
    public byte ExtAddressLength { get; set; }

    /// <summary>
    /// Extended Address as indicated by Address Type and Length.
    /// </summary>
    public byte[] ExtAddress { get; set; }

    public IPAddress IPv4Address { get; set; }

    public int IpPort { get; set; }

    public PhysicalAddress MacAddress { get; set; }

    public IPAddress IPv6Address { get; set; }
  }

  public sealed class PortCountEventArgs : EventArgs
  {
    public PortCountEventArgs(ushort device, ushort system, ushort portCount)
    {
      Device = device;

      System = system;

      PortCount = portCount;
    }

    public ushort Device { get; private set; }

    public ushort System { get; private set; }
    
    public ushort PortCount { get; private set; }
  }
  
  public sealed class DynamicDeviceCreatedArgs : EventArgs
  {
    public DynamicDeviceCreatedArgs(ushort system, ushort dynamicDevice)
    {
      System = system;

      DynamicDevice = dynamicDevice;
    }

    public ushort System { get; private set; }

    public ushort DynamicDevice { get; private set; }
  }
}
