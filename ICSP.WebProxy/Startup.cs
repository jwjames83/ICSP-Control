using System;
using System.Linq;

using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Properties;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

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
      services.AddWebSocketManager();
      services.AddProxyClient();
      services.AddConfig(Configuration);
      services.AddControllersWithViews();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ProxyConfig> config, IOptions<StaticFiles> staticFiles)
    {
      var lFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
      var lProvider = lFactory.CreateScope().ServiceProvider;

      // app.UseStatusCodePages();

      if(env.IsDevelopment())
      {
        app.UseStatusCodePagesWithRedirects("/Error/{0}");
      }
      else
      {
        app.UseStatusCodePagesWithRedirects("/Error/{0}");

        app.UseHsts();
      }

      // app.UseHttpsRedirection();

      // Accept web socket requests
      app.UseWebSockets();

      app = app.MapWebSocketManager("", lProvider.GetService<WebSocketProxyClient>());

      var lConnections = config.Value.Connections.Where(p => p.Enabled).ToArray();

      // StaticFiles -> Directories
      if(staticFiles.Value.Directories.Count() > 0)
      {
        Logger.LogInfo($"======================================================================================================================================================");
        Logger.LogInfo($"Using StaticFiles:Directories:");

        foreach(var directory in staticFiles.Value.Directories)
        {
          var lRootDirectory = Environment.ExpandEnvironmentVariables(directory.BaseDirectory);

          Logger.LogInfo($"Root={lRootDirectory}, Root={directory.RequestPath}");

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

              //// Setup
              //lFileServerOptions = new FileServerOptions
              //{
              //  FileProvider = new PhysicalFileProvider(lRootDirectory),

              //  RequestPath = connection.RequestPath + "/setup",
              //};

              //lFileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
              //lFileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("setup.html");

              //app.UseFileServer(lFileServerOptions);
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

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Setup}/{action=Index}/{id?}");
      });

      app.Use(async (context, next) =>
      {
        await next();

        var lReferer = context.Request.GetTypedHeaders().Referer;

        // After going down the pipeline check if 404
        /*
        if(lReferer == null && context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
          await context.Response.WriteAsync(Resources.Error_404_html);
        }
        */
      });

      // app.UseAuthentication();
      // app.UseAuthorization();
      // app.UseSession();
    }
  }
}
