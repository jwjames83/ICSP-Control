using System;
using System.Collections.Generic;

namespace ICSP.WebProxy.Configuration
{
  public static class ProxyConfigurator
  {
    public static void Configure(this ProxyConfig config)
    {
      if(config == null)
        throw new ArgumentNullException(nameof(config));

      if(config.Default == null)
        config.Default = new List<ProxyDefaultConfig>();

      if(config.Default.Count == 0)
      {
        var lDefaultConfig = new ProxyDefaultConfig()
        {
          LocalPort = 8000,
          RemoteHost = "localhost",
          RemotePort = 1319
        };

        config.Default.Add(lDefaultConfig);      
      }
      
      if(config.Devices == null)
        config.Devices = new List<ProxyDeviceConfig>();
    }
  }
}
