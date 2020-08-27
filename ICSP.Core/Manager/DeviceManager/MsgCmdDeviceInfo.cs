using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
{
  [MsgCmd(DeviceManagerCmd.DeviceInfo)]
  public class MsgCmdDeviceInfo : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.DeviceInfo;

    private MsgCmdDeviceInfo()
    {
    }

    public MsgCmdDeviceInfo(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        // Device
        Device = Data.GetBigEndianInt16(0);

        // System
        System = Data.GetBigEndianInt16(2);

        // DataFlag
        DataFlag = Data.GetBigEndianInt16(4);

        // ObjId
        ObjectId = Data[6];

        // ParentID
        ParentId = Data[7];

        // MfgId
        ManufactureId = Data.GetBigEndianInt16(8);

        // DeviceID
        DeviceId = Data.GetBigEndianInt16(10);

        // SerialNumber
        SerialNumber = AmxUtils.GetString(Data, 12, 16);
        // SerialNumber = Encoding.Default.GetString(Data.Range(12, 16)).TrimEnd(new char[] { '\0', ' ' });

        // FWID
        FirmwareId = Data.GetBigEndianInt16(28);

        var lOffset = 30;

        // Null-Terminated Strings ...

        // Version
        Version = AmxUtils.GetNullStr(Data, ref lOffset);

        // DeviceIdStr
        Name = AmxUtils.GetNullStr(Data, ref lOffset);

        // Manufacture
        Manufacture = AmxUtils.GetNullStr(Data, ref lOffset);

        // ExtAddressType
        ExtAddressType = (ExtAddressType)Data[lOffset++];

        // ExtAddressLength
        ExtAddressLength = Data[lOffset++];

        // ExtAddress
        ExtAddress = Data.Range(lOffset, ExtAddressLength);
      }

      if(ExtAddressType == ExtAddressType.IPv4Address)
      {
        try
        {
          IPv4Address = new IPAddress(ExtAddress.Range(0, 4));
        }
        catch(Exception ex)
        {
          Logger.LogError("DeviceInfo : {0}", ex.Message);
        }
      }

      // NI-700, NX-1200: IP, Port, MAC
      if(ExtAddressType == ExtAddressType.IPv4PortMac || ExtAddressType == ExtAddressType.IPv4PortMacIPv6)
      {
        try
        {
          IPv4Address = new IPAddress(ExtAddress.Range(0, 4));

          IpPort = ExtAddress.GetBigEndianInt16(4);

          MacAddress = new PhysicalAddress(ExtAddress.Range(6, 6));
        }
        catch(Exception ex)
        {
          Logger.LogError("DeviceInfo : {0}", ex.Message);
        }
      }

      // NX-1200: IPV4, Port, MAC, IPV6
      if(ExtAddressType == ExtAddressType.IPv4PortMacIPv6)
      {
        try
        {
          IPv6Address = new IPAddress(ExtAddress.Range(12, 16));
        }
        catch(Exception ex)
        {
          Logger.LogError("DeviceInfo : {0}", ex.Message);
        }
      }
    }
    
    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdDeviceInfo(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, DeviceInfoData deviceInfo)
    {
      var lRequest = new MsgCmdDeviceInfo
      {
        IPv4Address = deviceInfo.IPv4Address,
        Device = deviceInfo.Device,
        System = deviceInfo.System,
        DataFlag = deviceInfo.DataFlag,
        ObjectId = deviceInfo.ObjectId,
        ParentId = deviceInfo.ParentId,
        ManufactureId = deviceInfo.ManufactureId,
        DeviceId = deviceInfo.DeviceId,
        SerialNumber = deviceInfo.SerialNumber
      };

      Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");
      Logger.LogDebug(false, "MsgCmdDeviceInfo.CreateRequest: Dest={0:l}, Source={1:l}, IPv4Address={2:l}", dest, source, lRequest.IPv4Address);
      Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");

      // SerialNumber => 16 bytes of data
      if(string.IsNullOrWhiteSpace(lRequest.SerialNumber))
        lRequest.SerialNumber = new string('\0', 16);
      else
        lRequest.SerialNumber = lRequest.SerialNumber.PadRight(16, '\0');

      lRequest.FirmwareId = deviceInfo.FirmwareId;
      lRequest.Version = deviceInfo.Version;

      // Version => Generally, in this format: "v1.00\0"
      if(!string.IsNullOrWhiteSpace(lRequest.Version))
      {
        if(!lRequest.Version.StartsWith("v", StringComparison.CurrentCultureIgnoreCase))
          lRequest.Version = "v" + lRequest.Version;
      }
      
      lRequest.Name = deviceInfo.Name;
      lRequest.Manufacture = deviceInfo.Manufacture;

      lRequest.ExtAddressType = deviceInfo.ExtAddressType;

      // ExtAddressLength
      // NeuronId       : Length will be 6 and address will be the 48 - bit Neuron ID of the device.
      // IPv4Address    : Length will be 4 and address will be the 4-byte IP address of the device
      // AxLink         : Length must be 0
      // IPv4PortMac    : [IPV4, Port, MAC] => 4 + 2 + 6
      // RS232          : Length must be 0
      // IPv4PortMacIPv6: [IPV4, Port, MAC, IPv6] => 4 + 2 + 6 + 16
      switch(lRequest.ExtAddressType)
      {
        case ExtAddressType.NeuronId        /**/: lRequest.ExtAddressLength = 6; break;
        case ExtAddressType.IPv4Address     /**/: lRequest.ExtAddressLength = 4; break;
        case ExtAddressType.AxLink          /**/: lRequest.ExtAddressLength = 0; break;
        case ExtAddressType.IPv4PortMac     /**/: lRequest.ExtAddressLength = 12; break;
        case ExtAddressType.RS232           /**/: lRequest.ExtAddressLength = 0; break;
        case ExtAddressType.IPv4PortMacIPv6 /**/: lRequest.ExtAddressLength = 28; break;
      }

      lRequest.ExtAddress = new byte[] { };

      switch(lRequest.ExtAddressType)
      {
        case ExtAddressType.IPv4Address:
          {
            lRequest.ExtAddress = lRequest.IPv4Address?.GetAddressBytes();
            break;
          }
        case ExtAddressType.IPv4PortMac:
          {
            lRequest.ExtAddress =
              lRequest.IPv4Address?.GetAddressBytes().
              Concat(ArrayExtensions.Int16ToBigEndian(lRequest.IpPort)).
              Concat(lRequest.MacAddress.GetAddressBytes()).ToArray();
            break;
          }
        case ExtAddressType.IPv4PortMacIPv6:
          {
            lRequest.ExtAddress =
              lRequest.IPv4Address?.GetAddressBytes().
              Concat(ArrayExtensions.Int16ToBigEndian(lRequest.IpPort)).
              Concat(lRequest.MacAddress.GetAddressBytes()).
            Concat(lRequest.IPv6Address.GetAddressBytes()).ToArray();

            break;
          }
      }

      byte[] lData;

      using(var lStream = new MemoryStream())
      {
        // Device
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.Device), 0, 2);

        // System
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.System), 0, 2);

        // Flag
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.DataFlag), 0, 2);

        // ObjId
        lStream.Write(AmxUtils.Int16To8Bit(lRequest.ObjectId), 0, 1);

        // ParentId
        lStream.Write(AmxUtils.Int16To8Bit(lRequest.ParentId), 0, 1);

        // MfgId
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.ManufactureId), 0, 2);

        // DeviceID
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.DeviceId), 0, 2);

        // SerialNumber
        var lBytes = Encoding.GetEncoding(1252).GetBytes(lRequest.SerialNumber);
        lStream.Write(lBytes, 0, 16);

        // FWID
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.FirmwareId), 0, 2);

        // Null-Terminated Strings ...

        // Version
        lBytes = Encoding.GetEncoding(1252).GetBytes(lRequest.Version + "\0");
        lStream.Write(lBytes, 0, lBytes.Length);

        // DeviceID
        lBytes = Encoding.GetEncoding(1252).GetBytes(lRequest.Name + "\0");
        lStream.Write(lBytes, 0, lBytes.Length);

        // Manufacture
        lBytes = Encoding.GetEncoding(1252).GetBytes(lRequest.Manufacture + "\0");
        lStream.Write(lBytes, 0, lBytes.Length);

        // ExtAddressType (IP)
        lStream.Write(AmxUtils.Int16To8Bit((byte)lRequest.ExtAddressType), 0, 1);

        // ExtAddressLength
        lStream.Write(AmxUtils.Int16To8Bit(lRequest.ExtAddressLength), 0, 1);

        // ExtAddress
        lStream.Write(lRequest.ExtAddress, 0, lRequest.ExtAddressLength);

        lData = lStream.ToArray();
      }

      // Flag: 0x0212
      return lRequest.Serialize(0x0212, dest, source, 0, MsgCmd, lData);
    }

    protected override void WriteLogExtended()
    {
      var lAddressType = "Unknown";

      Logger.LogDebug(false, "{0:l} Device          : {1:00000}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} System          : {1}", GetType().Name, System);
      Logger.LogDebug(false, "{0:l} Flag            : {1}", GetType().Name, DataFlag);
      Logger.LogDebug(false, "{0:l} ObjectID        : {1}", GetType().Name, ObjectId);
      Logger.LogDebug(false, "{0:l} ParentID        : {1}", GetType().Name, ParentId);
      Logger.LogDebug(false, "{0:l} MfgID           : {1}", GetType().Name, ManufactureId);
      Logger.LogDebug(false, "{0:l} DeviceID        : 0x{1:X4}", GetType().Name, DeviceId);
      Logger.LogDebug(false, "{0:l} SerialNumber    : {1:l}", GetType().Name, SerialNumber);
      Logger.LogDebug(false, "{0:l} FWID            : 0x{1:X4}", GetType().Name, FirmwareId);
      Logger.LogDebug(false, "{0:l} Version         : {1:l}", GetType().Name, Version);
      Logger.LogDebug(false, "{0:l} Name            : {1:l}", GetType().Name, Name);
      Logger.LogDebug(false, "{0:l} Manufacture     : {1:l}", GetType().Name, Manufacture);

      switch(ExtAddressType)
      {
        case ExtAddressType.NeuronId        /**/: lAddressType = "Neuron-ID"; break;
        case ExtAddressType.IPv4Address     /**/: lAddressType = "IP4-Address"; break;
        case ExtAddressType.AxLink          /**/: lAddressType = "AXLink"; break;
        case ExtAddressType.RS232           /**/: lAddressType = "RS232"; break;
        case ExtAddressType.IPv4PortMac     /**/: lAddressType = "IPv4, Port, MAC"; break;
        case ExtAddressType.IPv4PortMacIPv6 /**/: lAddressType = "IPv4, Port, MAC, IPv6"; break;
      }

      if(ExtAddressType > 0)
        Logger.LogDebug(false, "{0:l} ExtAddressType  : 0x{1:X2} ({2:l})", GetType().Name, (byte)ExtAddressType, lAddressType);
      else
        Logger.LogDebug(false, "{0:l} ExtAddressType  : 0x{1:X2}", GetType().Name, ExtAddressType);

      Logger.LogDebug(false, "{0:l} ExtAddressLength: {1}", GetType().Name, ExtAddressLength);

      // NI-700: IP, Port, MAC
      if(ExtAddressType == ExtAddressType.IPv4PortMac || ExtAddressType == ExtAddressType.IPv4PortMacIPv6)
      {
        Logger.LogDebug(false, "{0:l} IPv4Address     : {1:l}", GetType().Name, IPv4Address);
        Logger.LogDebug(false, "{0:l} IpPort          : {1}", GetType().Name, IpPort);
        Logger.LogDebug(false, "{0:l} MacAddress      : {1:l}", GetType().Name, string.Join(":", MacAddress.GetAddressBytes().Select(b => b.ToString("X2"))));
      }

      // NX-1200: IPV4, Port, MAC, IPV6
      if(ExtAddressType == ExtAddressType.IPv4PortMacIPv6)
      {
        // :FFFF:AC10:108D
        Logger.LogDebug(false, "{0:l} IPv6Address     : {1:l}", GetType().Name, IPv6Address);
      }
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
    public ushort DataFlag { get; private set; }

    /// <summary>
    /// Unsigned 8-bit value.
    /// </summary>
    public byte ObjectId { get; private set; }

    /// <summary>
    /// Unsigned 8-bit value.
    /// </summary>
    public byte ParentId { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// The Manufacture ID as reported in the Device Info message for ParentID = 0 and ObjectID = 0
    /// </summary>
    public ushort ManufactureId { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// The Device ID as reported in the Device Info message for ParentID = 0 and ObjectID = 0
    /// </summary>
    public ushort DeviceId { get; private set; }

    /// <summary>
    /// 16 bytes of data. Format not defined yet.
    /// </summary>
    public string SerialNumber { get; private set; }

    /// <summary>
    ///  Unsigned 16-bit value.
    /// </summary>
    public ushort FirmwareId { get; private set; }

    /// <summary>
    /// CHAR array, NULL terminated, containing a 
    /// version string. Generally, in this format: "v1.00\0"
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// CHAR array, NULL terminated, containing a model 
    /// number.Generally, in this format: "NXC-232\0" (NetLinx Card-RS232)
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// CHAR array, NULL terminated, containing 
    /// String the mfg.'s name. Generally, in this format: "AMX Corp/0"
    /// </summary>
    public string Manufacture { get; private set; }

    /// <summary>
    /// 8-bit value. Used to indicate type of extended address to follow.
    /// </summary>
    public ExtAddressType ExtAddressType { get; private set; }

    /// <summary>
    /// 8-bit value. Used to indicate length in bytes of extended address to follow
    /// </summary>
    public byte ExtAddressLength { get; private set; }

    /// <summary>
    /// Extended Address as indicated by Address Type and Length.
    /// </summary>
    public byte[] ExtAddress { get; private set; }

    public IPAddress IPv4Address { get; private set; }

    public ushort IpPort { get; private set; }

    public PhysicalAddress MacAddress { get; private set; }

    public IPAddress IPv6Address { get; private set; }

    #endregion
  }
}
