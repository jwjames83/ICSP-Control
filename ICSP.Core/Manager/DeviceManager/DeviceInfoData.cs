using System;
using System.Net;
using System.Net.NetworkInformation;

using ICSP.Core.Environment;

namespace ICSP.Core.Manager.DeviceManager
{
  public class DeviceInfoData
  {
    public DeviceInfoData(ushort device, ushort system, IPAddress ipAddress)
    {
      // >= 65535 => NI-700 Crash!
      if(device == 0 || device >= 65535)
        throw new ArgumentOutOfRangeException(nameof(device), "NetLinx allows device numbers in the range 1 - 65534");

      Device = device;
      System = system;
      DataFlag = 0x0020;
      ObjectId = 0;
      ParentId = 0;
      ManufactureId = 0x0001;
      DeviceId = 0x0001;
      SerialNumber = string.Empty;
      FirmwareId = 0x0001;
      
      Version = ProgramProperties.Version.ToString();
      Name = ProgramProperties.Title;
      Manufacture = ProgramProperties.Company;

      ExtAddressType = ExtAddressType.IPv4Address;
      IPv4Address = ipAddress;
      IpPort = 1319;
      MacAddress = PhysicalAddress.None;
      IPv6Address = IPv4Address;
    }

    #region Properties

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort Device { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort System { get; private set; }

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
    /// Unsigned 16-bit value.
    /// (Firmware ID) A 16-bit value that uniquely identifies the object code that the device requires
    /// </summary>
    public ushort FirmwareId { get; set; }

    /// <summary>
    /// CHAR array, NULL terminated, containing a 
    /// version string. Generally, in this format: "v1.00\0"
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// CHAR array, NULL terminated, containing a model 
    /// number.Generally, in this format: "NXC-232\0" (NetLinx Card-RS232)
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
    
    public IPAddress IPv4Address { get; set; }

    public int IpPort { get; set; }

    public PhysicalAddress MacAddress { get; set; }

    public IPAddress IPv6Address { get; set; }

    #endregion
  }
}
