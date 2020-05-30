using System.Reflection;

using ICSP.WebProxy;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
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
      return app.UseWhen(context => context.WebSockets.IsWebSocketRequest, appBuilder =>
      {
        appBuilder.Map(path, app => app.UseMiddleware<WebSocketManagerMiddleware>(handler));
      });
    }
  }
}
