using System.Collections.Generic;

namespace ICSP.WebProxy.Configuration
{
  public class ProxyConfig
  {
    public List<ProxyConnectionConfig> Connections { get; set; } = new List<ProxyConnectionConfig>();
  }
}
