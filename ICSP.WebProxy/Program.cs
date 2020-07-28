using System;
using System.IO;
using System.Linq;
using System.Text;

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
    public const int CLOSE_SOCKET_TIMEOUT_MS = 2500;

    public static void Main(string[] args)
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

      var lLoggingConfig = GetLoggingConfiguration(args);

      // Initializes the Log system
      LoggingConfigurator.Configure(lLoggingConfig);

      Logger.LogInfo("Starting up");

      var lHostBuilder = CreateHostBuilder(args);

      // Initializes the Log system
      LoggingConfigurator.Configure(lHostBuilder, lLoggingConfig);

      lHostBuilder.Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
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
          Logger.LogInfo($"Use Url: {url}");

        webBuilder.UseUrls(lUrls);

        webBuilder.UseStartup<Startup>();

      });

      return lBuilder;
    }

    private static LoggingConfiguration GetLoggingConfiguration(string[] args)
    {
      var lConfig = new LoggingConfiguration();

      var config = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json")
       .Build();

      var lConfigx = config["Logging:LogLevel:Default"];

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

    public static ProxyConfig ProxyConfig { get; set; }
  }
}
