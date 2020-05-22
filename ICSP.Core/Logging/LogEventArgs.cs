using System;

using Serilog.Events;

namespace ICSP.Core.Logging
{
  public sealed class LogEventArgs : EventArgs
  {
    private LogEventArgs()
    {
    }

    public LogEventArgs(LogEventLevel level, string methodInfo, string message)
    {
      Level = level;

      MethodInfo = methodInfo;

      Message = message;
    }

    public LogEventLevel Level { get; }

    public string MethodInfo { get; }

    public string Message { get; }
  }
}
