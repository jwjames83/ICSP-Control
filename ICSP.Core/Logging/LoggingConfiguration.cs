using System;
using Serilog.Events;

namespace ICSP.Core.Logging
{
  public class LoggingConfiguration
  {
    public LoggingConfiguration()
    {
      var lBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

      var lExeFileName = AppDomain.CurrentDomain.FriendlyName;

      var lLogFileName = System.IO.Path.GetFileNameWithoutExtension(lExeFileName) + ".log";

      Path = System.IO.Path.Combine(lBaseDirectory, lLogFileName);
    }
    
    public string Path { get; set; }

    public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

    public MethodInfo MethodInfo { get; set; } = MethodInfo.Default;

    public int MaxLogFileLength { get; set; } = 2048;

    public int MaxSizeRollBackups { get; set; } = 5;
  }
}