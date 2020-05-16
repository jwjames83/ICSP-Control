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

    public MsgCmdGetEthernetIpAddress(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        // DataFlag = (RestartType)Data.GetBigEndianInt16(0);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdGetEthernetIpAddress(bytes);
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
      Logger.LogDebug(false, "{0:l}", GetType().Name);
    }
  }
}
