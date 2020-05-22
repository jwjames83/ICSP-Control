namespace ICSP.Core.Manager
{
  public class MsgCmdUnknown : ICSPMsg
  {
    public MsgCmdUnknown(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdUnknown(bytes);
    }
  }
}
