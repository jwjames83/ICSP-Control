using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

using Log4Net;
using Log4Net.Core;

namespace ICSP.Logging
{
  public static class Logger
  {
    private static readonly ILog Log4NetLogger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public static event EventHandler<LogEventArgs> OnLogEvent;

    public static readonly Regex AsychMethodName = new Regex(@"^<(?<Name>\w+)>\w+?");

    static Logger()
    {
      Trace.Listeners.Add(new Log4NetTraceListener());
    }

    public static void Log(string format, params object[] args)
    {
      LogInternal(Level.Info, true, format, args);
    }

    public static void Log(Level level, string format, params object[] args)
    {
      if(level != null)
        LogInternal(level, true, format, args);
      else
        LogInternal(Level.Info, true, format, args);
    }

    private static void LogInternal(Level level, bool methodInfo, string format, params object[] args)
    {
      try
      {
        var lCallerMethod = new StackTrace(1).GetFrame(1).GetMethod();

        var lDeclaringType = lCallerMethod.DeclaringType?.Name;
        var lMethodName = lCallerMethod.Name;

        // Fix Asynch-Method
        if(lMethodName == "MoveNext")
        {
          try
          {
            lDeclaringType = lCallerMethod.DeclaringType?.DeclaringType?.Name;
            lMethodName = AsychMethodName.Match(lCallerMethod.DeclaringType.Name).Groups["Name"].Value;
          }
          catch { }
        }

        var lMessage = format;

        if(args != null && args.Length > 0)
          lMessage = string.Format(format, args);

        OnLogEvent?.Invoke(null, new LogEventArgs(level, methodInfo, lCallerMethod, lMessage));

        if(methodInfo)
          lMessage = string.Format("{0}.{1}: {2}", lDeclaringType, lMethodName, lMessage);

        switch(level.Value)
        {
          case Level.LevelValueFatal: Log4NetLogger.Fatal(lMessage); break;
          case Level.LevelValueError: Log4NetLogger.Error(lMessage); break;
          case Level.LevelValueWarn: Log4NetLogger.Warn(lMessage); break;
          case Level.LevelValueInfo: Log4NetLogger.Info(lMessage); break;
          case Level.LevelValueDebug: Log4NetLogger.Debug(lMessage); break;
          case Level.LevelValueTrace: Trace.WriteLine(lMessage); break;
        }
      }
      catch(Exception ex)
      {
        Trace.WriteLine(ex.ToString());

        if(ex.InnerException != null)
          Trace.WriteLine(ex.InnerException.Message);
      }
    }

    public static void LogFatal(string message)
    {
      LogInternal(Level.Fatal, true, message, null);
    }

    public static void LogFatal(string format, params object[] args)
    {
      LogInternal(Level.Fatal, true, format, args);
    }

    public static void LogFatal(bool methodInfo, string message)
    {
      LogInternal(Level.Fatal, methodInfo, message, null);
    }

    public static void LogFatal(bool methodInfo, string format, params object[] args)
    {
      LogInternal(Level.Fatal, methodInfo, format, args);
    }

    public static void LogError(string message)
    {
      LogInternal(Level.Error, true, message, null);
    }

    public static void LogError(string format, params object[] args)
    {
      LogInternal(Level.Error, true, format, args);
    }

    public static void LogError(bool methodInfo, string message)
    {
      LogInternal(Level.Error, methodInfo, message, null);
    }

    public static void LogError(bool methodInfo, string format, params object[] args)
    {
      LogInternal(Level.Error, methodInfo, format, args);
    }

    public static void LogError(Exception ex)
    {
      LogInternal(Level.Error, true, GetExString(ex), null);
    }

    public static void LogError(bool methodInfo, Exception ex)
    {
      LogInternal(Level.Error, methodInfo, GetExString(ex), null);
    }

    public static void LogWarn(string message)
    {
      LogInternal(Level.Warn, true, message, null);
    }

    public static void LogWarn(string format, params object[] args)
    {
      LogInternal(Level.Warn, true, format, args);
    }

    public static void LogWarn(bool methodInfo, string message)
    {
      LogInternal(Level.Warn, methodInfo, message, null);
    }

    public static void LogWarn(bool methodInfo, string format, params object[] args)
    {
      LogInternal(Level.Warn, methodInfo, format, args);
    }

    public static void LogInfo(string message)
    {
      LogInternal(Level.Info, true, message, null);
    }

    public static void LogInfo(string format, params object[] args)
    {
      LogInternal(Level.Info, true, format, args);
    }

    public static void LogInfo(bool methodInfo, string message)
    {
      LogInternal(Level.Info, methodInfo, message, null);
    }

    public static void LogInfo(bool methodInfo, string format, params object[] args)
    {
      LogInternal(Level.Info, methodInfo, format, args);
    }

    public static void LogDebug(string message)
    {
      LogInternal(Level.Debug, true, message, null);
    }

    public static void LogDebug(string format, params object[] args)
    {
      LogInternal(Level.Debug, true, format, args);
    }

    public static void LogDebug(bool methodInfo, string message)
    {
      LogInternal(Level.Debug, methodInfo, message, null);
    }

    public static void LogDebug(bool methodInfo, string format, params object[] args)
    {
      LogInternal(Level.Debug, methodInfo, format, args);
    }

    public static void LogTrace(string message)
    {
      LogInternal(Level.Trace, true, message, null);
    }

    public static void LogTrace(string format, params object[] args)
    {
      LogInternal(Level.Trace, true, format, args);
    }

    public static void LogTrace(bool methodInfo, string message)
    {
      LogInternal(Level.Trace, methodInfo, message, null);
    }

    public static void LogTrace(bool methodInfo, string format, params object[] args)
    {
      LogInternal(Level.Trace, methodInfo, format, args);
    }

    private static string GetExString(Exception ex)
    {
      var lStr = string.Format("{0}\r\nStack : {1}", ex.Message, ex.StackTrace);

      if(ex.InnerException != null)
        lStr = lStr + "\n InnerException: " + GetExString(ex.InnerException);

      return lStr;
    }
  }
}