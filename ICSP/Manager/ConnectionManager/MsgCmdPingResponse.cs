using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.ConnectionManager
{
  /// <summary>
  /// This message is the response to a Ping Request message.
  /// The device responds with this message to inform the requester that the device is still on-line.
  /// </summary>
  [MsgCmd(ConnectionManagerCmd.PingResponse)]
  public class MsgCmdPingResponse : ICSPMsg
  {
    public const int MsgCmd = ConnectionManagerCmd.PingResponse;

    private MsgCmdPingResponse()
    {
    }

    public MsgCmdPingResponse(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        // Device
        Device = msg.Data.GetBigEndianInt16(0);

        // System
        System = msg.Data.GetBigEndianInt16(2);
        
        // MfgId
        ManufactureId = msg.Data.GetBigEndianInt16(4);

        // DeviceID
        DeviceId = msg.Data.GetBigEndianInt16(6);
        
        // ExtAddressType
        ExtAddressType = (ExtAddressType)msg.Data[8];

        // ExtAddressLength
        ExtAddressLength = msg.Data[9];

        // ExtAddress
        ExtAddress = msg.Data.Range(10, ExtAddressLength);
      }

      if(ExtAddressType == ExtAddressType.IPv4Address)
      {
        try
        {
          IPv4Address = new IPAddress(ExtAddress.Range(0, 4));
        }
        catch(Exception ex)
        {
          Logger.LogError("MsgCmdPingResponse: {0}", ex.Message);
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
          Logger.LogError("MsgCmdPingResponse: {0}", ex.Message);
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
          Logger.LogError("MsgCmdPingResponse: {0}", ex.Message);
        }
      }
    }

    public static ICSPMsg CreateRequest(ushort device, ushort system, ushort mfgID, ushort deviceId, IPAddress ipAddress)
    {
      var lSource = new AmxDevice(device, 0, system);
      
      var lRequest = new MsgCmdPingResponse();

      lRequest.Device = device;
      lRequest.System = system;
      lRequest.ManufactureId = mfgID;
      lRequest.DeviceId = deviceId;
      lRequest.IPv4Address = ipAddress;

      lRequest.ExtAddressType = ExtAddressType.IPv4Address;
      lRequest.ExtAddressLength = 4;
      lRequest.ExtAddress = lRequest.IPv4Address.GetAddressBytes();

      byte[] lData;

      using(var lStream = new MemoryStream())
      {
        // Device
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.Device), 0, 2);

        // System
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.System), 0, 2);
        
        // MfgId
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.ManufactureId), 0, 2);

        // DeviceID
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.DeviceId), 0, 2);
        
        // ExtAddressType (IP)
        lStream.Write(AmxUtils.Int16To8Bit((byte)lRequest.ExtAddressType), 0, 1);

        // ExtAddressLength
        lStream.Write(AmxUtils.Int16To8Bit(lRequest.ExtAddressLength), 0, 1);

        // ExtAddress
        lStream.Write(lRequest.ExtAddress, 0, lRequest.ExtAddressLength);

        lData = lStream.ToArray();
      }

      return lRequest.Serialize(AmxDevice.Empty, lSource, MsgCmd, lData);
    }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort Device { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort System { get; private set; }
    
    /// <summary>
    /// Unsigned 16-bit value.
    /// The Manufacture ID as reported in the Device Info message for ParentID = 0 and ObjectID = 0
    /// </summary>
    public ushort ManufactureId { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.The Device ID as 
    /// The Device ID as reported in the Device Info message for ParentID = 0 and ObjectID = 0
    /// </summary>
    public ushort DeviceId { get; private set; }

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

    public int IpPort { get; private set; }

    public PhysicalAddress MacAddress { get; private set; }

    public IPAddress IPv6Address { get; private set; }

    protected override void WriteLogExtended()
    {
      var lAddressType = "Unknown";

      Logger.LogDebug(false, "{0} Device          : {1:00000}", GetType().Name, Device);
      Logger.LogDebug(false, "{0} System          : {1}", GetType().Name, System);
      Logger.LogDebug(false, "{0} MfgID           : {1}", GetType().Name, ManufactureId);
      Logger.LogDebug(false, "{0} DeviceID        : 0x{1:X4}", GetType().Name, DeviceId);

      switch(ExtAddressType)
      {
        case ExtAddressType.NeuronId: lAddressType = "Neuron-ID"; break;
        case ExtAddressType.IPv4Address: lAddressType = "IP4-Address"; break;
        case ExtAddressType.AxLink: lAddressType = "AXLink"; break;
        case ExtAddressType.RS232: lAddressType = "RS232"; break;
        case ExtAddressType.IPv4PortMac: lAddressType = "IPv4, Port, MAC"; break;
        case ExtAddressType.IPv4PortMacIPv6: lAddressType = "IPv4, Port, MAC, IPv6"; break;
      }

      if(ExtAddressType > 0)
        Logger.LogDebug(false, "{0} ExtAddressType  : 0x{1:X2} ({2})", GetType().Name, (byte)ExtAddressType, lAddressType);
      else
        Logger.LogDebug(false, "{0} ExtAddressType  : 0x{1:X2}", GetType().Name, ExtAddressType);

      Logger.LogDebug(false, "{0} ExtAddressLength: {1}", GetType().Name, ExtAddressLength);

      // NI-700: IP, Port, MAC
      if(ExtAddressType == ExtAddressType.IPv4PortMac || ExtAddressType == ExtAddressType.IPv4PortMacIPv6)
      {
        Logger.LogDebug(false, "{0} IPv4Address     : {1}", GetType().Name, IPv4Address);
        Logger.LogDebug(false, "{0} IpPort          : {1}", GetType().Name, IpPort);
        Logger.LogDebug(false, "{0} MacAddress      : {1}", GetType().Name, string.Join(":", MacAddress.GetAddressBytes().Select(b => b.ToString("X2"))));
      }

      // NX-1200: IPV4, Port, MAC, IPV6
      if(ExtAddressType == ExtAddressType.IPv4PortMacIPv6)
      {
        // :FFFF:AC10:108D
        Logger.LogDebug(false, "{0} IPv6Address     : {1}", GetType().Name, IPv6Address);
      }
    }
  }
}
