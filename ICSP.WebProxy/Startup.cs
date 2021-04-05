using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using Serilog;

namespace ICSP.WebProxy
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      // add data protection services
      services.AddDataProtection()
        .SetApplicationName("shared app name"); ;

      services.AddWebSocketManager();
      services.AddProxyClient();
      services.AddConfig(Configuration);
      services.AddControllersWithViews();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, IOptions<ProxyConfig> proxyConfig, IOptions<StaticFiles> staticFiles)
    {
      var lFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
      var lProvider = lFactory.CreateScope().ServiceProvider;
      
      if(env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");

        app.UseHsts();
      }

      app.UseSerilogRequestLogging();

      app.UseStatusCodePagesWithReExecute("/Error/{0}");

      // app.UseHttpsRedirection();

      // Accept web socket requests
      app.UseWebSockets();

      app = app.MapWebSocketManager("", lProvider.GetService<WebSocketProxyClient>());

      var lConnections = proxyConfig.Value.Connections.Values.Where(p => p.Enabled);

      // StaticFiles -> Directories
      if(staticFiles.Value.Directories.Count() > 0)
      {
        Logger.LogInfo($"======================================================================================================================================================");
        Logger.LogInfo($"Using StaticFiles:Directories:");

        foreach(var directory in staticFiles.Value.Directories)
        {
          var lRootDirectory = Environment.ExpandEnvironmentVariables(directory.BaseDirectory);

          Logger.LogInfo($"Root={lRootDirectory}, Root={directory.RequestPath}");

          if(Directory.Exists(lRootDirectory))
          {
            var lFileServerOptions = new FileServerOptions
            {
              FileProvider = new PhysicalFileProvider(lRootDirectory),

              RequestPath = directory.RequestPath,
            };

            lFileServerOptions.StaticFileOptions.OnPrepareResponse = context =>
            {
              // Disable caching for all static files.
              context.Context.Response.Headers[HeaderNames.CacheControl] /**/ = staticFiles.Value.Headers.CacheControl;
              context.Context.Response.Headers[HeaderNames.Pragma]       /**/ = staticFiles.Value.Headers.Pragma;
              context.Context.Response.Headers[HeaderNames.Expires]      /**/ = staticFiles.Value.Headers.Expires;
            };

            app.UseFileServer(lFileServerOptions);
          }
          else
          {
            Logger.LogWarn($"Invalid Settings in appsettings.json: StaticFiles.Directories.BaseDirectory -> Directory not exists: {lRootDirectory}");
          }
        }

        Logger.LogInfo($"======================================================================================================================================================");
      }

      if(lConnections.Count() == 0)
      {
        Logger.LogInfo($"======================================================================================================================================================");
        Logger.LogWarn($"No valid connections configured -> (see appsettings.json)");
        Logger.LogInfo($"======================================================================================================================================================");
      }
      else
      {
        Logger.LogInfo($"======================================================================================================================================================");
        Logger.LogInfo($"Configured connections:");

        foreach(var connection in lConnections)
        {
          try
          {
            if(!string.IsNullOrWhiteSpace(connection.BaseDirectory))
            {
              var lRootDirectory = Environment.ExpandEnvironmentVariables(connection.BaseDirectory);

              Logger.LogInfo($"======================================================================================================================================================");
              Logger.LogInfo($"LocalHost  : {connection.LocalHost}");
              Logger.LogInfo($"RemoteHost : {connection.RemoteHost}");
              Logger.LogInfo($"Devices    : {string.Join(", ", connection.Devices)}");
              Logger.LogInfo($"Root       : {lRootDirectory}");
              Logger.LogInfo($"RequestPath: {connection.RequestPath}");

              var lFileServerOptions = new FileServerOptions
              {
                FileProvider = new PhysicalFileProvider(lRootDirectory),

                RequestPath = connection.RequestPath,
              };

              lFileServerOptions.StaticFileOptions.OnPrepareResponse = context =>
              {
                // Disable caching for all static files.
                context.Context.Response.Headers[HeaderNames.CacheControl] /**/ = staticFiles.Value.Headers.CacheControl;
                context.Context.Response.Headers[HeaderNames.Pragma]       /**/ = staticFiles.Value.Headers.Pragma;
                context.Context.Response.Headers[HeaderNames.Expires]      /**/ = staticFiles.Value.Headers.Expires;
              };

              app.UseFileServer(lFileServerOptions);
            }
          }
          catch(Exception ex)
          {
            Logger.LogError(ex.Message);
          }
        }

        Logger.LogInfo($"======================================================================================================================================================");
      }

      // Enable all static file middleware (except directory browsing) for the current
      // request path in the current directory.
      app.UseFileServer();

      // app.UseCookiePolicy();

      app.UseRouting();

      ConfigureRoutes(app, lConnections);

      // app.UseAuthentication();
      // app.UseAuthorization();
      // app.UseSession();
    }

    private void ConfigureRoutes(IApplicationBuilder app, IEnumerable<ProxyConnectionConfig> lConnections)
    {
      // MVC-Routes
      // ------------------------
      // /Error/{errorCode?}
      // /Status/{statusCode?}
      // /Controller/action/{id?}
      // ------------------------

      try
      {
        /*
        foreach(var connection in lConnections)
        {
          if(!string.IsNullOrWhiteSpace(connection.RequestPath))
          {
            app.UseEndpoints(endpoints =>
            {
              var lName = string.Concat(connection.RequestPath, "/error");
              var lPattern = string.Concat(connection.RequestPath, "/Error/{errorCode?}");

              // endpoints.MapControllerRoute(lName, lPattern, new { controller = "Error", action = "Index" });

              lName = string.Concat(connection.RequestPath, "/status");
              lPattern = string.Concat(connection.RequestPath, "/Status/{statusCode?}");

              // endpoiMapControllerRoute(lName, lPattern, new { controller = "Status", action = "Index" });

              lName = string.Concat(connection.RequestPath, "/default");
              lPattern = string.Concat(connection.RequestPath, "/{controller=Install}/{action=Index}/{id?}");

              // endpoints.MapControllerRoute(lName, lPattern);
            });
          }
        }
        */

        app.UseEndpoints(endpoints =>
        {
          endpoints.MapControllerRoute("error", "/Error/{statusCode?}", new { controller = "Error", action = "Index" });
          endpoints.MapControllerRoute("status", "/Status/{statusCode?}", new { controller = "Status", action = "Index" });
          endpoints.MapControllerRoute("default", "/{controller}/{action=Index}/{id?}");
        });
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }
    }
  }
}
