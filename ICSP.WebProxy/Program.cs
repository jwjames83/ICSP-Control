using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog.Events;

namespace ICSP.WebProxy
{
  public class Program
  {
    private static CancellationTokenSource mCts = new CancellationTokenSource();

    private static bool mRestartRequest;

    public const int CLOSE_SOCKET_TIMEOUT_MS = 2500;

    public static async Task Main(string[] args)
    {
      // GetEncoding(1252)
      // System.NotSupportedException: No data is available for encoding 1252
      // Add a reference to the System.Text.Encoding.CodePages.dll assembly to your project.
      // PM: Install-Package System.Text.Encoding.CodePages
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

      // Remove old LogFiles ...
      bool lIsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

      if(lIsDevelopment)
      {
        try
        {
          var lDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

          foreach(var file in lDirectory.EnumerateFiles("*.log"))
          {
            try
            {
              file.Delete();
            }
            catch { }
          }
        }
        catch { }
      }

      await StartServer(args);

      while(mRestartRequest)
      {
        mRestartRequest = false;

        await StartServer(args);
      }
    }

    public static void Restart()
    {
      Logger.LogWarn("Restarting App");

      mRestartRequest = true;

      mCts.Cancel();
    }

    private static async Task StartServer(string[] args)
    {
      try
      {
        mCts = new CancellationTokenSource();

        var lLoggingConfig = GetLoggingConfiguration();

        // Initializes the Log system
        LoggingConfigurator.Configure(lLoggingConfig);

        Logger.LogInfo("Starting up");

        var lHostBuilder = CreateHostBuilder(args);

        // Initializes the Log system
        LoggingConfigurator.Configure(lHostBuilder, lLoggingConfig);

        await lHostBuilder.Build().RunAsync(mCts.Token);
      }
      catch(OperationCanceledException ex)
      {
        Logger.LogError(ex.Message);
      }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
      var lBuilder = Host.CreateDefaultBuilder(args);

      var config = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json")
       .Build();

      var lConfig = config.GetSection(nameof(ProxyConfig)).Get<ProxyConfig>() ?? new ProxyConfig();

      lConfig.Configure();

      var lUrls = lConfig.Connections.Where(p => p.Enabled).Select(s => s.LocalHost).ToArray();

      Logger.LogInfo("Enable running as a Windows service ... (UseWindowsService)");

      // Enable running as a Windows service
      lBuilder.UseWindowsService();

      lBuilder.ConfigureWebHostDefaults(webBuilder =>
    {
      foreach(var url in lUrls.ToArray())
        Logger.LogInfo($"UseUrl: {url}");

      webBuilder.UseUrls(lUrls);

      webBuilder.UseStartup<Startup>();
    });

      return lBuilder;
    }

    private static LoggingConfiguration GetLoggingConfiguration()
    {
      var lConfig = new LoggingConfiguration();

      var config = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json")
       .Build();

      var lConfigx = config["Logging:LogLevel:WebProxy"];

      switch(lConfigx?.ToLower())
      {
        case "none"        /**/: lConfig.LogLevel = (LogEventLevel)6; break;
        case "critical"    /**/: lConfig.LogLevel = LogEventLevel.Fatal; break;
        case "error"       /**/: lConfig.LogLevel = LogEventLevel.Error; break;
        case "warning"     /**/: lConfig.LogLevel = LogEventLevel.Warning; break;
        case "information" /**/: lConfig.LogLevel = LogEventLevel.Information; break;
        case "debug"       /**/: lConfig.LogLevel = LogEventLevel.Debug; break;
        case "trace"       /**/: lConfig.LogLevel = LogEventLevel.Verbose; break;
        case "verbose"     /**/: lConfig.LogLevel = LogEventLevel.Verbose; break;
      }

      return lConfig;
    }
  }
}
