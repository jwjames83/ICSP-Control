using System.Collections.Generic;

namespace ICSP.WebProxy.Configuration
{
  public class ProxyConfig
  {
    public Dictionary<string, ProxyConnectionConfig> Connections { get; set; } = new Dictionary<string, ProxyConnectionConfig>();
  }
}
