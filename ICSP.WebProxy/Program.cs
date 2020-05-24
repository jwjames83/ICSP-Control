using System.Text;

using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;

using Microsoft.AspNetCore.Hosting;
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

      // CreateHostBuilder(args).Build().Run();

      var lHostBuilder = CreateHostBuilder(args);

      // Initializes the Log system
      LoggingConfigurator.Configure(lHostBuilder, new LoggingConfiguration() { LogLevel = LogEventLevel.Information });

      Logger.LogLevel = LogEventLevel.Information;

      lHostBuilder.Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      var lBuilder = Host.CreateDefaultBuilder(args);

      lBuilder.ConfigureWebHostDefaults(webBuilder =>
      {
        webBuilder.UseStartup<Startup>();
      });

      return lBuilder;
    }

    public static ProxyConfig ProxyConfig { get; set; }
  }
}
