using System;

using ICSP.Core.Logging;

using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

namespace ICSP.WebProxy
{
  public static class LoggingConfigurator
  {
    public const string LogTemplateFile    /**/ = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level,-11:l}] {StackTrace}{Message:lj}{NewLine}";
    public const string LogTemplateConsole /**/ = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    public static void Configure(IHostBuilder hostBuilder, LoggingConfiguration loggingConfiguration, bool console = true)
    {
      Logger.Flush();

      if(loggingConfiguration == null)
        throw new ArgumentNullException(nameof(loggingConfiguration));

      try
      {
        Logger.MethodInfo = loggingConfiguration.MethodInfo;

        var lPath = loggingConfiguration.Path;
        var lLimitBytes = (long)loggingConfiguration.MaxLogFileLength * 1024;
        var lFileCountLimit = loggingConfiguration.MaxSizeRollBackups;

        hostBuilder.UseSerilog((context, configuration) =>
        {
          switch(loggingConfiguration.LogLevel)
          {
            case LogEventLevel.Fatal: configuration.MinimumLevel.Fatal(); break;
            case LogEventLevel.Error: configuration.MinimumLevel.Error(); break;
            case LogEventLevel.Warning: configuration.MinimumLevel.Warning(); break;
            case LogEventLevel.Information: configuration.MinimumLevel.Information(); break;
            case LogEventLevel.Debug: configuration.MinimumLevel.Debug(); break;
            case LogEventLevel.Verbose: configuration.MinimumLevel.Verbose(); break;
          }

          // lConf.Enrich.With<CustomLevelEnricher>();
          // lConf.Enrich.With<StackTraceEnricher>();

          configuration.WriteTo.File(lPath, outputTemplate: LogTemplateFile, fileSizeLimitBytes: lLimitBytes, retainedFileCountLimit: lFileCountLimit, rollOnFileSizeLimit: true, flushToDiskInterval: TimeSpan.FromSeconds(1));

          if(console)
            configuration.WriteTo.Console(outputTemplate: LogTemplateConsole);
        });
      }
      catch(Exception ex)
      {
        Logger.LogError(ex);
      }
    }
  }
}