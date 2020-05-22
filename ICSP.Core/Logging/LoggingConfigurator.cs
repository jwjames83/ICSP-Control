using System;

using Serilog;
using Serilog.Events;

namespace ICSP.Core.Logging
{
  public class LoggingConfigurator
  {
    // public const string LogTemplateFile    /**/ = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level,-11:l}] {StackTrace}{Message:lj}{NewLine}";
    // 2020-05-01 14:56:23.084 +02:00 [Information] [ICSPClient.Connect] StartClient: Host="172.16.126.250", Port=1319
    // 2020-05-01 14:56:23.252 +02:00 [Information] [ICSPClient.Connect] Client connected: "172.16.126.250":1319

    public const string LogTemplateFile    /**/ = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {StackTrace}{Message:lj}{NewLine}";
    public const string LogTemplateConsole /**/ = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    public static void Configure(bool console = true)
    {
      Configure(new LoggingConfiguration(), console);
    }

    public static void Configure(LoggingConfiguration loggingConfiguration, bool console = true)
    {
      try
      {
        Logger.Flush();

        Logger.MethodInfo = loggingConfiguration.MethodInfo;

        var lPath = loggingConfiguration.Path;
        var lLimitBytes = (long)loggingConfiguration.MaxLogFileLength * 1024;
        var lFileCountLimit = loggingConfiguration.MaxSizeRollBackups;

        var lConf = new LoggerConfiguration();

        switch(loggingConfiguration.LogLevel)
        {
          case LogEventLevel.Fatal       /**/: lConf.MinimumLevel.Fatal(); break;
          case LogEventLevel.Error       /**/: lConf.MinimumLevel.Error(); break;
          case LogEventLevel.Warning     /**/: lConf.MinimumLevel.Warning(); break;
          case LogEventLevel.Information /**/: lConf.MinimumLevel.Information(); break;
          case LogEventLevel.Debug       /**/: lConf.MinimumLevel.Debug(); break;
          case LogEventLevel.Verbose     /**/: lConf.MinimumLevel.Verbose(); break;
        }

        // lConf.Enrich.With<CustomLevelEnricher>();
        // lConf.Enrich.With<StackTraceEnricher>();

        lConf.WriteTo.File(lPath, outputTemplate: LogTemplateFile, fileSizeLimitBytes: lLimitBytes, retainedFileCountLimit: lFileCountLimit, rollOnFileSizeLimit: true, flushToDiskInterval: TimeSpan.FromSeconds(1));

        if(console)
          lConf.WriteTo.Console(outputTemplate: LogTemplateConsole);

        Log.Logger = lConf.CreateLogger();
      }
      catch(Exception ex)
      {
        Logger.LogError(ex);
      }
    }
  }
}