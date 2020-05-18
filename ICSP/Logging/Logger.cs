using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSP.Reflection;

using Serilog;
using Serilog.Events;

namespace ICSP.Logging
{
  public static class Logger
  {
    public static event EventHandler<LogEventArgs> OnLogEvent;

    private static readonly Serilog.Core.Logger TextLogger;

    private static readonly StringWriter StringWriter;

    private static readonly StringBuilder StringBuilder;

    static Logger()
    {
      try
      {
        Trace.Listeners.Add(new TraceListener());

        StringBuilder = new StringBuilder();

        StringWriter = new StringWriter(StringBuilder);

        TextLogger = new LoggerConfiguration()
          .WriteTo.TextWriter(StringWriter, outputTemplate: "{Message}")
          .CreateLogger();
      }
      catch { }
    }

    public static MethodInfo MethodInfo { get; set; } = MethodInfo.Default;

    public static LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

    private static void LogInternal(LogEventLevel level, bool methodInfo, string format, params object[] args)
    {
      if(Logger.LogLevel > level)
        return;

      try
      {
        var lMessage = format;

        if(TextLogger != null)
        {
          // Format String
          // ------------------------------------------------------------------------
          TextLogger.Information(format, args);

          lMessage = StringWriter.ToString();

          StringBuilder.Clear();
          // ------------------------------------------------------------------------
        }
        else
        {
          lMessage = string.Format(format, args);
        }

        if(MethodInfo == MethodInfo.Default && methodInfo || MethodInfo == MethodInfo.Allways)
        {
          var lCallerMethod = new StackTrace(1).GetFrame(1).GetMethod();

          var lInfo = lCallerMethod.GetMethodName();

          var lMethodInfo = string.Format("{0}.{1}", lInfo.Type, lInfo.Name);

          OnLogEvent?.Invoke(null, new LogEventArgs(level, lMethodInfo, lMessage));

          lMessage = string.Format("[{0}] {1}", lMethodInfo, lMessage);
        }
        else
        {
          OnLogEvent?.Invoke(null, new LogEventArgs(level, null, lMessage));
        }

        switch(level)
        {
          case LogEventLevel.Fatal       /**/: Log.Fatal(lMessage); break;
          case LogEventLevel.Error       /**/: Log.Error(lMessage); break;
          case LogEventLevel.Warning     /**/: Log.Warning(lMessage); break;
          case LogEventLevel.Information /**/: Log.Information(lMessage); break;
          case LogEventLevel.Debug       /**/: Log.Debug(lMessage); break;
          case LogEventLevel.Verbose     /**/: Log.Verbose(lMessage); break;
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
      LogInternal(LogEventLevel.Fatal, true, message, null);
    }

    public static void LogFatal(string format, params object[] args)
    {
      LogInternal(LogEventLevel.Fatal, true, format, args);
    }

    public static void LogFatal(bool methodInfo, string message)
    {
      LogInternal(LogEventLevel.Fatal, methodInfo, message, null);
    }

    public static void LogFatal(bool methodInfo, string format, params object[] args)
    {
      LogInternal(LogEventLevel.Fatal, methodInfo, format, args);
    }

    public static void LogError(string message)
    {
      LogInternal(LogEventLevel.Error, true, message, null);
    }

    public static void LogError(string format, params object[] args)
    {
      LogInternal(LogEventLevel.Error, true, format, args);
    }

    public static void LogError(bool methodInfo, string message)
    {
      LogInternal(LogEventLevel.Error, methodInfo, message, null);
    }

    public static void LogError(bool methodInfo, string format, params object[] args)
    {
      LogInternal(LogEventLevel.Error, methodInfo, format, args);
    }

    public static void LogError(Exception ex)
    {
      LogInternal(LogEventLevel.Error, true, GetExString(ex), null);
    }

    public static void LogError(bool methodInfo, Exception ex)
    {
      LogInternal(LogEventLevel.Error, methodInfo, GetExString(ex), null);
    }

    public static void LogWarn(string message)
    {
      LogInternal(LogEventLevel.Warning, true, message, null);
    }

    public static void LogWarn(string format, params object[] args)
    {
      LogInternal(LogEventLevel.Warning, true, format, args);
    }

    public static void LogWarn(bool methodInfo, string message)
    {
      LogInternal(LogEventLevel.Warning, methodInfo, message, null);
    }

    public static void LogWarn(bool methodInfo, string format, params object[] args)
    {
      LogInternal(LogEventLevel.Warning, methodInfo, format, args);
    }

    public static void LogInfo(string message)
    {
      LogInternal(LogEventLevel.Information, true, message, null);
    }

    public static void LogInfo(string format, params object[] args)
    {
      LogInternal(LogEventLevel.Information, true, format, args);
    }

    public static void LogInfo(bool methodInfo, string message)
    {
      LogInternal(LogEventLevel.Information, methodInfo, message, null);
    }

    public static void LogInfo(bool methodInfo, string format, params object[] args)
    {
      LogInternal(LogEventLevel.Information, methodInfo, format, args);
    }

    public static void LogDebug(string message)
    {
      LogInternal(LogEventLevel.Debug, true, message, null);
    }

    public static void LogDebug(string format, params object[] args)
    {
      LogInternal(LogEventLevel.Debug, true, format, args);
    }

    public static void LogDebug(bool methodInfo, string message)
    {
      LogInternal(LogEventLevel.Debug, methodInfo, message, null);
    }

    public static void LogDebug(bool methodInfo, string format, params object[] args)
    {
      LogInternal(LogEventLevel.Debug, methodInfo, format, args);
    }

    public static void LogVerbose(string message)
    {
      LogInternal(LogEventLevel.Verbose, true, message, null);
    }

    public static void LogVerbose(string format, params object[] args)
    {
      LogInternal(LogEventLevel.Verbose, true, format, args);
    }

    public static void LogVerbose(bool methodInfo, string message)
    {
      LogInternal(LogEventLevel.Verbose, methodInfo, message, null);
    }

    public static void LogVerbose(bool methodInfo, string format, params object[] args)
    {
      LogInternal(LogEventLevel.Verbose, methodInfo, format, args);
    }

    private static string GetExString(Exception ex)
    {
      var lStr = string.Format("{0}\r\nStack : {1}", ex.Message, ex.StackTrace);

      if(ex.InnerException != null)
        lStr = lStr + "\n InnerException: " + GetExString(ex.InnerException);

      return lStr;
    }

    public static void Flush()
    {
      Log.CloseAndFlush();
    }
  }
}