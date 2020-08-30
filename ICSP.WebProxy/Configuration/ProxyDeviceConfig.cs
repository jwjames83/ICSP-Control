namespace ICSP.WebProxy.Configuration
{
  public class ProxyDeviceConfig
  {
    public ushort PortCount { get; set; } = 1;

    public string DeviceName { get; set; }

    public string DeviceVersion { get; set; }

    public ushort DeviceId { get; set; }
  }
}