using ICSP.WebProxy.Proxy;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ProxyClientExtensions
  {
    public static IServiceCollection AddProxyClient(this IServiceCollection services)
    {
      services.AddSingleton<ICSPConnectionManager>();

      services.AddScoped<ProxyClient>();

      return services;
    }
  }
}
