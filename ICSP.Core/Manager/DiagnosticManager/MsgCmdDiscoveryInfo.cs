using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DiagnosticManager
{
  /// <summary>
  /// Unknown: (Probably IP Device Discovery)
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.DiscoveryInfo)]
  public class MsgCmdDiscoveryInfo : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.DiscoveryInfo;

    private MsgCmdDiscoveryInfo()
    {
    }

    public MsgCmdDiscoveryInfo(byte[] buffer) : base(buffer)
    {
      ProgramName = string.Empty;

      MainFile = string.Empty;

      // System
      System = Data.GetBigEndianInt16(2);

      // DeviceID
      DeviceId = Data.GetBigEndianInt16(6);

      // NX-1200 Master v1.5.78 (Null Terminated)
      // ICSP-Test "Main.axs"   (Null Terminated)
      // 18 1c ac 10 ... Unkwnown

      var lOffset = 8;

      // Description
      Description = AmxUtils.GetNullStr(Data, ref lOffset);

      // ProgramInfo
      var lProgramInfo = AmxUtils.GetNullStr(Data, ref lOffset);

      // [ProgramName][Space]["MainFile"]
      if(lProgramInfo.Contains("\""))
      {
        lProgramInfo = lProgramInfo.TrimEnd('\"');

        var lPos = lProgramInfo.LastIndexOf('"');

        if(lPos >= 0 && lPos + 1 <= lProgramInfo.Length)
        {
          ProgramName = lProgramInfo.Substring(0, lPos);

          MainFile = lProgramInfo.Substring(lPos + 1);
        }
      }

      if(string.IsNullOrWhiteSpace(ProgramName))
        ProgramName = lProgramInfo;

      // ExtAddressType
      ExtAddressType = (ExtAddressType)Data[lOffset++];

      // ExtAddressLength
      ExtAddressLength = Data[lOffset++];

      // ExtAddress
      ExtAddress = Data.Range(lOffset, ExtAddressLength);

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
      return new MsgCmdDiscoveryInfo(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdDiscoveryInfo();

      var lData = new byte[] { };

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }

    /// <summary>
    /// Unsigned 16-bit value.
    /// </summary>
    public ushort System { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// The Device ID as reported in the Device Info message for ParentID = 0 and ObjectID = 0
    /// </summary>
    public ushort DeviceId { get; private set; }

    public string Description { get; private set; }

    public string ProgramName { get; private set; }

    public string MainFile { get; private set; }

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

    protected override void WriteLogExtended()
    {
      var lAddressType = "Unknown";

      Logger.LogDebug(false, "{0:l} System          : {1}", GetType().Name, System);
      Logger.LogDebug(false, "{0:l} DeviceID        : 0x{1:X4}", GetType().Name, DeviceId);
      Logger.LogDebug(false, "{0:l} Description     : {1:l}", GetType().Name, Description);
      Logger.LogDebug(false, "{0:l} ProgramName     : {1:l}", GetType().Name, ProgramName);
      Logger.LogDebug(false, "{0:l} MainFile        : {1:l}", GetType().Name, MainFile);

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
  }
}