using System;
using System.IO;
using System.Linq;

using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ICSP.WebProxy
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      var lConfig = Configuration.GetSection(nameof(ProxyConfig)).Get<ProxyConfig>() ?? new ProxyConfig();

      lConfig.Configure();

      Program.ProxyConfig = lConfig;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddWebSocketManager();
      services.AddProxyClient();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
      var lFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
      var lProvider = lFactory.CreateScope().ServiceProvider;

      // app.UseStatusCodePages();

      if(env.IsDevelopment())
      {
        // app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler(configure =>
        {
          configure.Run(async context =>
          {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";

            await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
            await context.Response.WriteAsync("ERROR!<br><br>\r\n");

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            // Use exceptionHandlerPathFeature to process the exception (for example, 
            // logging), but do NOT expose sensitive error information directly to 
            // the client.

            if(exceptionHandlerPathFeature?.Error is FileNotFoundException)
            {
              await context.Response.WriteAsync("File error thrown!<br><br>\r\n");
            }

            await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
            await context.Response.WriteAsync("</body></html>\r\n");
            await context.Response.WriteAsync(new string(' ', 512)); // IE padding
          });
        });

        app.UseHsts();
      }

      // app.UseHttpsRedirection();

      // Accept web socket requests
      app.UseWebSockets();

      app = app.MapWebSocketManager("", lProvider.GetService<WebSocketProxyClient>());

      var lConfigs = Program.ProxyConfig.Connections.Where(p => p.Enabled).ToArray();

      var lUseDefault = true;
           
      foreach(var config in lConfigs)
      {
        try
        {
          if(!string.IsNullOrWhiteSpace(config.BaseDirectory))
          {
            Logger.LogInfo($"UseStaticFiles: Path={config.BaseDirectory}, RequestPath={config.RequestPath}");

            app.UseDefaultFiles(new DefaultFilesOptions()
            {
              FileProvider = new PhysicalFileProvider(Path.Combine(config.BaseDirectory)),

              RequestPath = config.RequestPath,
            });

            app.UseStaticFiles(new StaticFileOptions
            {
              FileProvider = new PhysicalFileProvider(Path.Combine(config.BaseDirectory)),

              RequestPath = config.RequestPath,

              OnPrepareResponse = context =>
              {
                // Disable caching for all static files.
                context.Context.Response.Headers["Cache-Control"] /**/ = Configuration["StaticFiles:Headers:Cache-Control"];
                context.Context.Response.Headers["Pragma"]        /**/ = Configuration["StaticFiles:Headers:Pragma"];
                context.Context.Response.Headers["Expires"]       /**/ = Configuration["StaticFiles:Headers:Expires"];
              }
            });

            // Setup
            var lOptions = new DefaultFilesOptions()
            {
              FileProvider = new PhysicalFileProvider(Path.Combine(config.BaseDirectory)),

              RequestPath = config.RequestPath + "/setup",
            };

            lOptions.DefaultFileNames.Clear();
            lOptions.DefaultFileNames.Add("setup.html");

            app.UseDefaultFiles(lOptions);

            app.UseStaticFiles(new StaticFileOptions
            {
              FileProvider = new PhysicalFileProvider(Path.Combine(config.BaseDirectory)),

              RequestPath = config.RequestPath + "/setup",
            });

            lUseDefault = false;
          }
        }
        catch(Exception ex)
        {
          Logger.LogError(ex.Message);
        }
      }

      // No Base-Directories configured, use defaults ...
      if(lUseDefault)
      {
        app.UseDefaultFiles();
        app.UseStaticFiles();
      }

      // app.UseCookiePolicy();

      app.UseRouting();

      /*
      app.Use(async (context, next) =>
      {
        await next.Invoke();
        
        // After going down the pipeline check if we 404'd. 
        if(context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
          Console.WriteLine(context.Request.Path);

          var lSb = new StringBuilder();

          lSb.AppendLine("<html>");
          lSb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1'>");
          lSb.AppendLine("<style>");
          lSb.AppendLine("body, html {");
          lSb.AppendLine("  height: 100%;");
          lSb.AppendLine("  margin: 0;");
          lSb.AppendLine("}");
          lSb.AppendLine(".bg {");
          lSb.AppendLine("  background-image: url('data:image/png;base64," + Convert.ToBase64String(ICSP.WebProxy.Properties.Resources.Error_404) + "');");
          lSb.AppendLine("  height: 100%;");
          lSb.AppendLine("  background-position: center;");
          lSb.AppendLine("  background-repeat: no-repeat;");
          lSb.AppendLine("  background-size: cover;");
          lSb.AppendLine("}");
          lSb.AppendLine("</style>");
          lSb.AppendLine("<body>");
          lSb.AppendLine("<div class='bg'></div>");
          lSb.AppendLine("</body>");
          lSb.AppendLine("</html>");

          context.Response.ContentType = "text/html";

          await context.Response.WriteAsync(lSb.ToString());
        }
      });
      */

      // app.UseAuthentication();
      // app.UseAuthorization();
      // app.UseSession();
    }
  }
}
