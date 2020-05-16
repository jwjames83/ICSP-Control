using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Feedback Channel ON message is generated for diagnostic purposes only.
  /// See the Diagnostic Manager specification for more information.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.FeedbackChannelOn)]
  public class MsgCmdFeedbackChannelOn : MsgBaseCmdChannel<MsgCmdFeedbackChannelOn>
  {
    private MsgCmdFeedbackChannelOn()
    {
    }

    public MsgCmdFeedbackChannelOn(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdFeedbackChannelOn(bytes);
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.FeedbackChannelOn;
      }
    }
  }
}