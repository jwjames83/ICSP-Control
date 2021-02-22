using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

      // A Windows Service app returns the C:\WINDOWS\system32 folder as its current directory.
      Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

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

      // Failed to bind to address http://[::]:80: address already in use.
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
        Logger.LogInfo($"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}");

        var lHostBuilder = CreateHostBuilder(args);

        // Initializes the Log system
        LoggingConfigurator.Configure(lHostBuilder, lLoggingConfig);

        await lHostBuilder.Build().RunAsync(mCts.Token);
      }
      catch(OperationCanceledException ex)
      {
        Logger.LogError(ex.Message);
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
      var lBuilder = Host.CreateDefaultBuilder(args);

      Logger.LogInfo("Enable running as a Windows service ... (UseWindowsService)");

      // Enable running as a Windows service
      lBuilder.UseWindowsService();

      // Remove duplicate urls
      // Prevent Error: Failed to bind to address http://[::]:80: address already in use.
      var lUrls = ConfigGetUrls();

      if(lUrls.Length == 0)
        Logger.LogWarn($"Invalid Settings in appsettings.json: ProxyConfig.Connections -> No connections configured");

      foreach(var url in lUrls)
        Logger.LogInfo($"UseUrl[]: {url}");


      lBuilder.ConfigureWebHostDefaults(webBuilder =>
      {
        try
        {
          webBuilder.UseUrls(lUrls);
        }
        catch(Exception ex)
        {
          Logger.LogError(ex.Message);
        }

        webBuilder.UseStartup<Startup>();
      });

      return lBuilder;
    }

    private static readonly Regex RegexUrl = new Regex(@"^((?<scheme>[^:/?#]+):(?=//))?(//)?(((?<login>[^:]+)(?::(?<password>[^@]+)?)?@)?(?<host>[^@/?#:]*)(?::(?<port>\d+)?)?)?(?<path>[^?#]*)(\?(?<query>[^#]*))?(#(?<fragment>.*))?", RegexOptions.None);

    private static string[] ConfigGetUrls()
    {
      var lValidUrls = new Dictionary<string, string>();

      try
      {
        var lConfig = GetConfigSection<ProxyConfig>("appsettings.json", nameof(ProxyConfig));

        var lUrls = lConfig.Connections.Values.Where(p => p.Enabled).Select(s => s.LocalHost).ToArray();

        foreach(var url in lUrls)
        {
          var lMatch = RegexUrl.Match(url);

          if(lMatch.Success)
          {
            var lScheme = lMatch.Groups["scheme"].Value;
            var lHost = lMatch.Groups["host"].Value;
            ushort.TryParse(lMatch.Groups["port"].Value, out var lPort);

            if(lPort == 0)
              lPort = 80;

            var lKey = $"{lScheme}:{lPort}";

            if(!lValidUrls.ContainsKey(lKey))
            {
              lValidUrls.Add(lKey, url);

              Logger.LogDebug($"Url: {url}");
            }
            else
            {
              Logger.LogWarn($"Url: {url} has ignored, {lScheme}://[::]:{lPort}: address already in use");
            }
          }
        }
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }

      return lValidUrls.Values.ToArray();
    }

    private static LoggingConfiguration GetLoggingConfiguration()
    {
      var lLoggingConfig = new LoggingConfiguration();

      var lLogLevel = GetConfigValue<string>("appsettings.json", "Logging.LogLevel.WebProxy");

      switch(lLogLevel?.ToLower())
      {
        case "none"        /**/: lLoggingConfig.LogLevel = (LogEventLevel)6; break;
        case "critical"    /**/: lLoggingConfig.LogLevel = LogEventLevel.Fatal; break;
        case "error"       /**/: lLoggingConfig.LogLevel = LogEventLevel.Error; break;
        case "warning"     /**/: lLoggingConfig.LogLevel = LogEventLevel.Warning; break;
        case "information" /**/: lLoggingConfig.LogLevel = LogEventLevel.Information; break;
        case "debug"       /**/: lLoggingConfig.LogLevel = LogEventLevel.Debug; break;
        case "trace"       /**/: lLoggingConfig.LogLevel = LogEventLevel.Verbose; break;
        case "verbose"     /**/: lLoggingConfig.LogLevel = LogEventLevel.Verbose; break;
      }

      return lLoggingConfig;
    }

    public static T GetConfigSection<T>(string path, string key) where T : class, new()
    {
      // This code not work (IConfigurationRoot.Reload) ... 
      /*
      var config = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json", true, true)
       .Build();

      config.Reload();
      */

      if(!File.Exists(path))
        return new T();

      var lJsonObj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));

      return lJsonObj.TryGetValue(key, out JToken section) ? JsonConvert.DeserializeObject<T>(section.ToString()) : (new T());
    }

    public static T GetConfigValue<T>(string path, string key)
    {
      // This code not work (IConfigurationRoot.Reload) ... 
      /*
      var config = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json", true, true)
       .Build();

      config.Reload();
      */

      if(!File.Exists(path))
        return default;

      var lJsonObj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));

      var lToken = lJsonObj.SelectToken(key);

      if(lToken != null)
        return lToken.Value<T>();

      return default;
    }
  }
}
