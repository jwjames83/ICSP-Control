using System;
using System.Reflection;

using Log4Net.Core;

namespace ICSP.Logging
{
  public sealed class LogEventArgs : EventArgs
  {
    private LogEventArgs()
    {
    }

    public LogEventArgs(Level level, bool methodInfo, MethodBase method, string message)
    {
      Level = level;

      MethodInfo = methodInfo;

      Method = method;

      Message = message;
    }

    public Level Level { get; }

    public bool MethodInfo { get; }

    public MethodBase Method { get; }

    public string Message { get; }
  }
}
