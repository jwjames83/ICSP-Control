namespace ICSP.WebProxy.Configuration
{
  public class ProxyDeviceConfig
  {
    public ushort PortCount { get; set; } = 1;

    public ushort AddressCount { get; set; } = 1;

    public ushort ChannelCount { get; set; } = 256;

    public ushort LevelCount { get; set; } = 8;

    public string DeviceName { get; set; }

    public string DeviceVersion { get; set; }

    public ushort DeviceId { get; set; }
  }
}