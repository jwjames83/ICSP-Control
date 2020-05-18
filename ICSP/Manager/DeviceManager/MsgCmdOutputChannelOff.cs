using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Output Channel OFF message is generated when an Output channel is turned OFF from a master.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.OutputChannelOff)]
  public class MsgCmdOutputChannelOff : MsgBaseCmdChannel<MsgCmdOutputChannelOff>
  {
    private MsgCmdOutputChannelOff()
    {
    }

    public MsgCmdOutputChannelOff(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdOutputChannelOff(bytes);
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.OutputChannelOff;
      }
    }
  }
}