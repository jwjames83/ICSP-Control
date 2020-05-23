namespace ICSP.WebProxy.Configuration
{
  public class ProxyDefaultConfig
  {
    public ushort LocalPort { get; set; }

    public string RemoteHost { get; set; } = "localhost";

    public ushort RemotePort { get; set; } = 1319;
  }
}