using ICSP.Core.Constants;

namespace ICSP.Core.Manager.DeviceManager
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

    public MsgCmdFeedbackChannelOff(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdFeedbackChannelOff(bytes);
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
