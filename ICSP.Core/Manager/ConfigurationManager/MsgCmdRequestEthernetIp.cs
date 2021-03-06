﻿using ICSP.Core.Constants;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConfigurationManager
{
  /// <summary>
  /// Request the device's Ethernet interface IP address, subnetmask, and gateway of the unit’s Ethernet interface.
  /// The response to this message is the Get Ethernet IPAddress message.
  /// </summary>
  [MsgCmd(ConfigurationManagerCmd.RequestEthernetIpAddress)]
  public class MsgCmdRequestEthernetIp : ICSPMsg
  {
    public const int MsgCmd = ConfigurationManagerCmd.RequestEthernetIpAddress;

    private MsgCmdRequestEthernetIp()
    {
    }

    public MsgCmdRequestEthernetIp(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestEthernetIp(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdRequestEthernetIp();
      
      return lRequest.Serialize(dest, source, MsgCmd, null);
    }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l}", GetType().Name);
    }
  }
}
