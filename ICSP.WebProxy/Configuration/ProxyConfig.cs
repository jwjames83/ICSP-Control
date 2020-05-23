using System.Collections.Generic;

namespace ICSP.WebProxy.Configuration
{
  public class ProxyConfig
  {
    public List<ProxyDefaultConfig> Default { get; set; } = new List<ProxyDefaultConfig>();

    public List<ProxyDeviceConfig> Devices { get; set; } = new List<ProxyDeviceConfig>();
  }
}
