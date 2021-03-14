using System;

using ICSP.Core.Manager.DeviceManager;

namespace ICSP.Core
{
  public class CreateDeviceInfoRequest
  {
    public CreateDeviceInfoRequest()
    {
      ID = Guid.NewGuid().ToString();

      State = DeviceConnectionState.Uninitialized;

      PortCount = 1;

      ChannelCount = 256;

      LevelCount = 8;
    }

    public CreateDeviceInfoRequest(ushort msgId, DeviceInfoData deviceInfo, ushort portCount = 1, ushort channelCount = 256, ushort levelCount = 8) : this()
    {
      MsgID = msgId;

      DeviceInfo = deviceInfo;

      PortCount = portCount;

      ChannelCount = channelCount;

      LevelCount = levelCount;
    }

    public string ID { get; private set; }

    public DeviceConnectionState State { get; set; } = DeviceConnectionState.Uninitialized;

    public ushort MsgID { get; set; }

    public DeviceInfoData DeviceInfo { get; set; }

    public ushort PortCount { get; set; }

    public ushort ChannelCount { get; set; }

    public ushort LevelCount { get; set; }
  }
}
