using ICSP.WebProxy.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ConfigurationExtensions
  {
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config)
    {
      services.Configure<StaticFiles>(config.GetSection(nameof(StaticFiles)));

      // services.Configure<ProxyConfig>(config.GetSection(nameof(ProxyConfig)));

      services.ConfigureWritable<ProxyConfig>(config.GetSection(nameof(ProxyConfig)));

      services.Configure<ProxyConfig>(config =>
      {
        foreach(var connection in config.Connections)
        {
          connection.Value.Parent = config;

          connection.Value.ID = int.Parse(connection.Key);
        }
      });

      services.Configure<WebControlConfig>(config.GetSection(nameof(WebControlConfig)));

      return services;
    }
  }
}
