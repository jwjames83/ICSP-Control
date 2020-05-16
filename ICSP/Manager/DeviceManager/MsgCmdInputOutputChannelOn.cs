using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Input/Output Channel ON Status message is generated when an Input/Output channel is turned ON from a device/port.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.InputOutputChannelOn)]
  public class MsgCmdInputOutputChannelOn : MsgBaseCmdChannel<MsgCmdInputOutputChannelOn>
  {
    private MsgCmdInputOutputChannelOn()
    {
    }

    public MsgCmdInputOutputChannelOn(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdInputOutputChannelOn(bytes);
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.InputOutputChannelOn;
      }
    }
  }
}
