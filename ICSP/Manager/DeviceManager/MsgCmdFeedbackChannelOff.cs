using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Feedback Channel OFF message is generated for diagnostic purposes only.
  /// See the Diagnostic Manager specification for more information.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.FeedbackChannelOff)]
  public class MsgCmdFeedbackChannelOff : MsgBaseCmdChannel<MsgCmdFeedbackChannelOff>
  {
    private MsgCmdFeedbackChannelOff()
    {
    }

    public MsgCmdFeedbackChannelOff(ICSPMsgData msg) : base(msg)
    {
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.FeedbackChannelOff;
      }
    }
  }
}
