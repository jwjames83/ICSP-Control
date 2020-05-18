using ICSP.Constants;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// The Input Channel ON message is generated when a button is Pushed or 
  /// Input channel is turned ON from a device/port or a master.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.InputChannelOnStatus)]
  public class MsgCmdInputChannelOnStatus : MsgBaseCmdChannel<MsgCmdInputChannelOnStatus>
  {
    private MsgCmdInputChannelOnStatus()
    {
    }

    public MsgCmdInputChannelOnStatus(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdInputChannelOnStatus(bytes);
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.InputChannelOnStatus;
      }
    }
  }
}