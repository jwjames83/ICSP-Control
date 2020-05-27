using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

using ICSP.Core.Client;

using Microsoft.AspNetCore.Http;

namespace ICSP.WebProxy.Configuration
{
  public static class ProxyConfigManager
  {
    private static Regex RegexUrl = new Regex(@"^((?<scheme>[^:/?#]+):(?=//))?(//)?(((?<login>[^:]+)(?::(?<password>[^@]+)?)?@)?(?<host>[^@/?#:]*)(?::(?<port>\d+)?)?)?(?<path>[^?#]*)(\?(?<query>[^#]*))?(#(?<fragment>.*))?", RegexOptions.None);

    public static void Configure(this ProxyConfig config)
    {
      if(config == null)
        throw new ArgumentNullException(nameof(config));

      if(config.Connections == null)
        config.Connections = new List<ProxyConnectionConfig>();

      if(config.Connections.Count == 0)
      {
        var lDefaultConfig = new ProxyConnectionConfig()
        {
          RemoteHost = "localhost",
          RemotePort = ICSPClient.DefaultPort
        };

        config.Connections.Add(lDefaultConfig);
      }
    }

    public static ProxyConnectionConfig GetConfig(HttpContext context, WebSocket socket)
    {
      var lLocalScheme = context.Request.Scheme;
      var lLocalPort = context.Connection.LocalPort;

      foreach(var item in Program.ProxyConfig.Connections)
      {
        var lMatch = RegexUrl.Match(item.LocalHost);

        if(lMatch.Success)
        {
          var lScheme = lMatch.Groups["scheme"].Value;
          ushort.TryParse(lMatch.Groups["port"].Value, out var lPort);

          if(lPort == 0)
            lPort = 80;

          if(lLocalScheme.Equals(lScheme, StringComparison.OrdinalIgnoreCase) && lLocalPort == lPort)
            return item;
        }
      }

      // First Default ...
      return Program.ProxyConfig.Connections[0];
    }
  }
}
