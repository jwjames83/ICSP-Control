using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Output Channel ON message is generated when an Output channel is turned ON from a master
  /// </summary>
  [MsgCmd(DeviceManagerCmd.OutputChannelOn)]
  public class MsgCmdOutputChannelOn : MsgBaseCmdChannel<MsgCmdOutputChannelOn>
  {
    private MsgCmdOutputChannelOn()
    {
    }

    public MsgCmdOutputChannelOn(ICSPMsgData msg) : base(msg)
    {
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.OutputChannelOn;
      }
    }
  }
}