using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Output Channel OFF Status message is generated when an Output channel is turned OFF from a device/port.
  /// Note that when the master turns a channel OFF, it assumes the channel is OFF.
  /// Therefore, the device should not send this message in response to the channel being turned OFF by the master.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.OutputChannelOffStatus)]
  public class MsgCmdOutputChannelOffStatus : MsgBaseCmdChannel<MsgCmdOutputChannelOffStatus>
  {
    private MsgCmdOutputChannelOffStatus()
    {
    }

    public MsgCmdOutputChannelOffStatus(ICSPMsgData msg) : base(msg)
    {
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.OutputChannelOffStatus;
      }
    }
  }
}
