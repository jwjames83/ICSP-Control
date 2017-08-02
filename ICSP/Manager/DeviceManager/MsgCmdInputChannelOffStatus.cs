using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  [MsgCmd(DeviceManagerCmd.InputChannelOffStatus)]
  public class MsgCmdInputChannelOffStatus : MsgBaseCmdChannel<MsgCmdInputChannelOffStatus>
  {
    /// <summary>
    /// The Input Channel OFF message is generated when a button is Released or 
    /// Input channel is turned OFF from a device/port or a master.
    /// </summary>
    private MsgCmdInputChannelOffStatus()
    {
    }

    public MsgCmdInputChannelOffStatus(ICSPMsgData msg) : base(msg)
    {
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