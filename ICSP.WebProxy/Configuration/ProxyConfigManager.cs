using System;
using System.Collections.Generic;
using System.Net.WebSockets;

using ICSP.Core.Client;

using Microsoft.AspNetCore.Http;

namespace ICSP.WebProxy.Configuration
{
  public static class ProxyConfigManager
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
          RemotePort = ICSPClient.DefaultPort
        };

        config.Default.Add(lDefaultConfig);      
      }
      
      if(config.Devices == null)
        config.Devices = new List<ProxyDeviceConfig>();
    }

    public static ProxyDeviceConfig GetConfig(HttpContext context, WebSocket socket)
    {
      foreach(var item in Program.ProxyConfig.Devices)
      {
        if(item.LocalPort == context.Connection.LocalPort)
        {
          return item;
        }
      }

      foreach(var item in Program.ProxyConfig.Default)
      {
        if(item.LocalPort == context.Connection.LocalPort)
        {
          return new Configuration.ProxyDeviceConfig()
          {
            Device = item.Device,
            RemoteHost = item.RemoteHost,
            RemotePort = item.RemotePort
          };
        }
      }

      // First Default ...
      var lDefault = Program.ProxyConfig.Default[0];

      return new Configuration.ProxyDeviceConfig()
      {
        Device = 0,
        RemoteHost = lDefault.RemoteHost,
        RemotePort = lDefault.RemotePort
      };
    }
  }
}
