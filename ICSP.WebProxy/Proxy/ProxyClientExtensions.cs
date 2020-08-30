using ICSP.Core.Reflection;
using ICSP.WebProxy.Converter;
using ICSP.WebProxy.Proxy;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ProxyClientExtensions
  {
    public static IServiceCollection AddProxyClient(this IServiceCollection services)
    {
      services.AddSingleton<ICSPConnectionManager>();

      services.AddScoped<ProxyClient>();
      
      var lTypes = TypeHelper.GetImplementedClassesForInterface(typeof(IMessageConverter));

      foreach(var type in lTypes)
        services.AddScoped(typeof(IMessageConverter), type);

      return services;
    }
  }
}
