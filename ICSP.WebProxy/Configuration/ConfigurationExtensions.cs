using ICSP.WebProxy.Configuration;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ConfigurationExtensions
  {
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config)
    {
      services.Configure<StaticFiles>(config.GetSection(nameof(StaticFiles)));

      services.Configure<ProxyConfig>(config.GetSection(nameof(ProxyConfig)));

      services.Configure<WebControlConfig>(config.GetSection(nameof(WebControlConfig)));
      
      return services;
    }
  }
}
