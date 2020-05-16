using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

using ICSP.Client;
using ICSP.Constants;

namespace ICSP.Manager.ConnectionManager
{
  /// <summary>
  /// This message requests a device number from a master.
  /// The Newbee flag must be set in the message header.
  /// </summary>
  [MsgCmd(ConnectionManagerCmd.DynamicDeviceAddressRequest)]
  public class MsgCmdDynamicDeviceAddressRequest : ICSPMsg
  {
    public const int MsgCmd = ConnectionManagerCmd.DynamicDeviceAddressRequest;

    public const int NewbieFlag = 0x0222;

    private MsgCmdDynamicDeviceAddressRequest()
    {
    }

    public MsgCmdDynamicDeviceAddressRequest(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdDynamicDeviceAddressRequest(bytes);
    }

    public static ICSPMsg CreateRequest()
    {
      var lRandomAddress = GetRandomPrivate​IpAddress();

      return CreateRequest(32001, lRandomAddress);
    }

    public static ICSPMsg CreateRequest(ushort proposedDevice)
    {
      var lRandomAddress = GetRandomPrivate​IpAddress();

      return CreateRequest(proposedDevice, lRandomAddress);
    }

    public static ICSPMsg CreateRequest(IPAddress ipAddress)
    {
      return CreateRequest(32001, ipAddress);
    }

    public static ICSPMsg CreateRequest(ushort proposedDevice, IPAddress ipAddress)
    {
      // Device 0 refers to the Master
      // NetLinx allows device numbers in the range 0 - 32767
      // numbers above 32767 are reserved for internal use
      //     1 - 32000: Physical devices range
      // 32768 - 36863: Virtual devices range

      // >= 65535 => NI-700 Crash!
      if(proposedDevice == 0 || proposedDevice >= 65535)
        throw new ArgumentOutOfRangeException(nameof(proposedDevice), "NetLinx allows device numbers in the range 1 - 65534");
      
      var lSource = new AmxDevice(proposedDevice, 0, 0);

      var lRequest = new MsgCmdDynamicDeviceAddressRequest
      {
        IPv4Address = ipAddress,

        ProposedDevice = lSource.Device,
        ExtAddressType = ExtAddressType.IPv4Address,
        ExtAddressLength = 4
      };

      lRequest.ExtAddress = lRequest.IPv4Address.GetAddressBytes();

      byte[] lData;

      using(var lStream = new MemoryStream())
      {
        // Device
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.ProposedDevice), 0, 2);
        
        // ExtAddressType (IP)
        lStream.Write(AmxUtils.Int16To8Bit((byte)lRequest.ExtAddressType), 0, 1);

        // ExtAddressLength
        lStream.Write(AmxUtils.Int16To8Bit(lRequest.ExtAddressLength), 0, 1);

        // ExtAddress
        lStream.Write(lRequest.ExtAddress, 0, lRequest.ExtAddressLength);

        // Fill (Port[2], Mac[6], IPv6[16])
        lStream.Write(new byte[24], 0, 24);
        
        lData = lStream.ToArray();
      }

      return lRequest.Serialize(NewbieFlag, AmxDevice.Empty, lSource, 0, MsgCmd, lData);
    }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort ProposedDevice { get; private set; }

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

    private static IPAddress GetRandomPrivate​IpAddress()
    {
      var lRandom = new Random();

      return new IPAddress(new byte[] { 169, 254, (byte)lRandom.Next(1, 255), (byte)lRandom.Next(1, 255) });
    }
  }
}
