using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

using ICSP.Constants;
using ICSP.Logging;
using ICSP.Manager.DeviceManager;

namespace ICSP.Manager.ConfigurationManager
{
  /// <summary>
  /// Get the IP address, subnet mask, and gateway of the units Ethernet interface.
  /// </summary>
  [MsgCmd(ConfigurationManagerCmd.GetEthernetIpAddress)]
  public class MsgCmdGetEthernetIpAddress : ICSPMsg
  {
    public const int MsgCmd = ConfigurationManagerCmd.GetEthernetIpAddress;

    private MsgCmdGetEthernetIpAddress()
    {
    }

    public MsgCmdGetEthernetIpAddress(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        // DataFlag = (RestartType)msg.Data.GetBigEndianInt16(0);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, IPAddress ipAddress)
    {
      var lRequest = new MsgCmdGetEthernetIpAddress();

      byte[] lData;

      var lInterfaces = NetworkInterface
          .GetAllNetworkInterfaces()
          .Where(n => n.OperationalStatus == OperationalStatus.Up)
          .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

      UnicastIPAddressInformation lIpAddressInfo = null;
      IPAddress lGatewayInfo = null;

      foreach(var nic in lInterfaces)
      {
        lIpAddressInfo = nic.GetIPProperties()?.UnicastAddresses
          .Where(a => a?.Address?.Equals(ipAddress) ?? false)
          .FirstOrDefault();

        if(lIpAddressInfo != null)
        {
          lGatewayInfo = nic.GetIPProperties()?.GatewayAddresses
          .Select(g => g?.Address)
          .Where(a => a != null)
          // .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
          // .Where(a => Array.FindIndex(a.GetAddressBytes(), b => b != 0) >= 0)
          .FirstOrDefault();

          break;
        }
      }

      using(var lStream = new MemoryStream())
      {
        var lUseDhcp = false;

        // Flags
        // Bitfield:
        // Bit O - It set, use DHCP for IP address and subnet mask.
        //         The IP address and Subnet mask fields must be supplied, but are ignored.
        // Bit 1-7 Unused.
        var lFlags = lUseDhcp ? 0b_10000000 : 0b_00000000;
        lStream.Write(AmxUtils.Int16To8Bit(lFlags), 0, 1);

        // HostName: Null terminated host name string. (e.g. "NetLinx")
        var lBytes = Encoding.Default.GetBytes(global::System.Environment.MachineName + "\0");
        lStream.Write(lBytes, 0, lBytes.Length);

        // IP Address: Null terminated address string. Must be the IP address in dot notation form (e.g. "192.168.25.5")
        lBytes = Encoding.Default.GetBytes(lIpAddressInfo?.Address + "\0");
        lStream.Write(lBytes, 0, lBytes.Length);

        // Subnet Mask: Null terminated address string. Must be the IP address in dot notation form (e.g. "255.255.255.0")
        lBytes = Encoding.Default.GetBytes(lIpAddressInfo?.IPv4Mask + "\0");
        lStream.Write(lBytes, 0, lBytes.Length);

        // Gateway: Null terminated address string. Must be the IP address in dot notation form (e.g. "192.168.26.2"). 
        lBytes = Encoding.Default.GetBytes(lGatewayInfo + "\0");
        lStream.Write(lBytes, 0, lBytes.Length);

        lData = lStream.ToArray();
      }

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0}", GetType().Name);
    }


    #region Properties

    /// <summary>
    /// Bitfield:
    /// </summary>
    public ushort Flags { get; private set; }

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
