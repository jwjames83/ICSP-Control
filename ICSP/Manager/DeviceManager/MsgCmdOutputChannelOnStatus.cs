using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Output Channel ON Status message is generated when an Output channel is turned ON from a device/port.
  /// Note that when the master turns a channel ON, it assumes the channel is ON.
  /// Therefore, the device should not send this message in response to the channel being turned ON by the master.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.OutputChannelOnStatus)]
  public class MsgCmdOutputChannelOnStatus : MsgBaseCmdChannel<MsgCmdOutputChannelOnStatus>
  {
    private MsgCmdOutputChannelOnStatus()
    {
    }

    public MsgCmdOutputChannelOnStatus(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdOutputChannelOnStatus(bytes);
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.OutputChannelOnStatus;
      }
    }
  }
}
