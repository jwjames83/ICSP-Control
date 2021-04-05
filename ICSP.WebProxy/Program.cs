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
using ICSP.WebProxy.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;

namespace ICSP.WebProxy
{
  public class Program
  {
    private static CancellationTokenSource CtsRestart = new CancellationTokenSource();

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

      var lEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

      // Remove old log files ...
      if(lEnvironment.Equals("Development", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          var lDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"));

          if(lDirectory.Exists)
            lDirectory.Delete(true);
        }
        catch { }
      }

      // Serilog Two-stage initialization
      // Initializes the log system (CreateBootstrapLogger)
      try
      {
        Logger.MethodInfo = MethodInfo.Default;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json")
            .AddJsonFile($"appsettings.{lEnvironment}.json", true)
            .Build();

        Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(configuration)
          .CreateBootstrapLogger();

        Logger.LogInfo($"Application starting");
        Logger.LogInfo($"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}");
      }
      catch(Exception ex)
      {
        Log.Fatal(ex.Message);
      }

      // Failed to bind to address http://[::]:80: address already in use.
      await StartServer(args);

      // Restart Server
      while(CtsRestart.IsCancellationRequested)
        await StartServer(args);
    }

    private static async Task StartServer(string[] args)
    {
      try
      {
        CtsRestart = new CancellationTokenSource();

        var lHostBuilder = CreateHostBuilder(args);

        // Serilog Two-stage initialization
        // Initializes the log system (create the final logger)
        try
        {
          Logger.LogInfo("Configure Serilog: ...");

          lHostBuilder.UseSerilog((context, services, configuration) =>
          {
            configuration.ReadFrom.Configuration(context.Configuration);
            // configuration.ReadFrom.Services(services);
          });
        }
        catch(Exception ex)
        {
          Logger.LogError(ex);
        }

        Logger.LogInfo("Start RunAsync with cancellation token ...");

        await lHostBuilder.Build().RunAsync(CtsRestart.Token);

        Logger.LogInfo("Stopped ...");
      }
      /*
      catch(OperationCanceledException ex)
      {
        mRestartRequest = true;

        Logger.LogError("StartServer[OperationCanceledException]: Type={0}, Message={1}", ex.GetType().FullName, ex.Message);

        Logger.LogError(ex);
      }*/
      catch(Exception ex)
      {
        Logger.LogError("StartServer[Exception]: Type={0}, Message={1}", ex.GetType().FullName, ex.Message);

        Logger.LogError(ex);

        CtsRestart.Cancel();
      }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
      var lBuilder = Host.CreateDefaultBuilder(args);

      if(WindowsServiceHelpers.IsWindowsService())
      {
        Logger.LogInfo("Running as windows service ...");

        // Enable running as a Windows service
        lBuilder.UseWindowsService();
      }
      else
      {
        Logger.LogInfo("Running as console process ...");
      }

      lBuilder.UseApplicationRestart(CtsRestart);

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

        Logger.LogInfo("IWebHostBuilder.UseStartup ...");

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

    //private static LoggingConfiguration GetLoggingConfiguration()
    //{
    //  var lLoggingConfig = new LoggingConfiguration();

    //  var lLogLevel = GetConfigValue<string>("appsettings.json", "Logging.LogLevel.WebProxy");

    //  switch(lLogLevel?.ToLower())
    //  {
    //    case "none"        /**/: lLoggingConfig.LogLevel = (LogEventLevel)6; break;
    //    case "critical"    /**/: lLoggingConfig.LogLevel = LogEventLevel.Fatal; break;
    //    case "error"       /**/: lLoggingConfig.LogLevel = LogEventLevel.Error; break;
    //    case "warning"     /**/: lLoggingConfig.LogLevel = LogEventLevel.Warning; break;
    //    case "information" /**/: lLoggingConfig.LogLevel = LogEventLevel.Information; break;
    //    case "debug"       /**/: lLoggingConfig.LogLevel = LogEventLevel.Debug; break;
    //    case "trace"       /**/: lLoggingConfig.LogLevel = LogEventLevel.Verbose; break;
    //    case "verbose"     /**/: lLoggingConfig.LogLevel = LogEventLevel.Verbose; break;
    //  }

    //  return lLoggingConfig;
    //}

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
