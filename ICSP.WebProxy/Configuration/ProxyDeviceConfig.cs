namespace ICSP.WebProxy.Configuration
{
  public class ProxyDeviceConfig
  {
    public ushort LocalPort { get; set; }

    public string RemoteHost { get; set; }

    public ushort RemotePort { get; set; }

    public ushort Device { get; set; }
  }
}
