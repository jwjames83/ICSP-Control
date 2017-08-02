using System;

using Log4Net;
using Log4Net.Core;

namespace ICSPControl.Logging
{
  public static class ILogExtentions
  {
    public static void Trace(this ILog log, string message)
    {
      log.Trace(message, null);
    }

    public static void Trace(this ILog log, string message, Exception exception)
    {
      log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, Level.Trace, message, exception);
    }

    public static void Verbose(this ILog log, string message)
    {
      log.Verbose(message, null);
    }

    public static void Verbose(this ILog log, string message, Exception exception)
    {
      log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, Level.Verbose, message, exception);
    }
  }
}
