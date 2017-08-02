using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Input/Output Channel OFF Status message is generated when an Input/Output channel is turned OFF from a device/port.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.InputOutputChannelOff)]
  public class MsgCmdInputOutputChannelOff : MsgBaseCmdChannel<MsgCmdInputOutputChannelOff>
  {
    private MsgCmdInputOutputChannelOff()
    {
    }

    public MsgCmdInputOutputChannelOff(ICSPMsgData msg) : base(msg)
    {
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.InputOutputChannelOff;
      }
    }
  }
}