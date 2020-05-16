﻿using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.ConfigurationManager
{
  /// <summary>
  /// This message is generated by the master or device to confirm receipt of a message sent.
  /// The MessageID field must match the original message's MessageID and the Response bit should be set.
  /// </summary>
  [MsgCmd(ConfigurationManagerCmd.Restart)]
  public class MsgCmdRestart : ICSPMsg
  {
    public const int MsgCmd = ConfigurationManagerCmd.Restart;

    private MsgCmdRestart()
    {
    }

    public MsgCmdRestart(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        DataFlag = (RestartType)Data.GetBigEndianInt16(0);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRestart(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, RestartType flag)
    {
      var lRequest = new MsgCmdRestart
      {
        DataFlag = flag
      };

      return lRequest.Serialize(device, source, MsgCmd, AmxUtils.Int16ToBigEndian((ushort)lRequest.DataFlag));
    }

    /// <summary>
    /// 0 - Device reboot.
    /// The device should perform the “coldest” boot possible.
    /// 
    /// 1 - NetLinx restart.
    /// Shutdown and restart the NetLinx interpreter.
    ///  
    /// 65535 - Smart reboot.
    /// The device should perform the necessary steps to activate any previous configuration parameters.
    /// Including, but not limited to, “cold” rebooting the device.
    /// For example, if the IP address configuration has changed, and a reboot is 
    /// required to begin using the new IP, then the device should reboot.
    /// </summary>
    public RestartType DataFlag { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} RestartType: {1} ({2})", GetType().Name, (ushort)DataFlag, DataFlag);
    }
  }
}
