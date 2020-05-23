using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ICSP.WebProxy
{
  public static class WebSocketManagerExtensions
  {
    public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
    {
      services.AddTransient<ConnectionManager>();

      foreach(var type in Assembly.GetEntryAssembly().ExportedTypes)
      {
        if(type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
        {
          services.AddSingleton(type);
        }
      }

      return services;
    }

    public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler handler)
    {
      return app.Map(path, app => app.UseMiddleware<WebSocketManagerMiddleware>(handler));
    }
  }
}
