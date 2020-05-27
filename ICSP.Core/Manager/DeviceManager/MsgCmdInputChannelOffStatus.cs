using ICSP.Core.Constants;

namespace ICSP.Core.Manager.DeviceManager
{
  /// <summary>
  /// The Input Channel OFF message is generated when a button is Released or 
  /// Input channel is turned OFF from a device/port or a master.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.InputChannelOffStatus)]
  public class MsgCmdInputChannelOffStatus : MsgBaseCmdChannel<MsgCmdInputChannelOffStatus>
  {
    private MsgCmdInputChannelOffStatus()
    {
    }

    public MsgCmdInputChannelOffStatus(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdInputChannelOffStatus(bytes);
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.InputChannelOffStatus;
      }
    }
  }
}