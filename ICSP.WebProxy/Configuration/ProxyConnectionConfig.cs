using System.Collections.Generic;

using ICSP.Core.Client;

namespace ICSP.WebProxy.Configuration
{
  public class ProxyConnectionConfig
  {
    public string LocalHost { get; set; }

    public string RemoteHost { get; set; }

    public ushort RemotePort { get; set; } = ICSPClient.DefaultPort;

    public List<ushort> Devices { get; set; } = new List<ushort>();

    public string BaseDirectory { get; set; }

    public string RequestPath { get; set; }

    public bool Enabled { get; set; } = true;

    public string Converter { get; set; }

    public Dictionary<string, ProxyDeviceConfig> DeviceConfig { get; set; } = new Dictionary<string, ProxyDeviceConfig>();
  }
}
