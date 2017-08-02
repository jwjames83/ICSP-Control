using System.Diagnostics;
using System.Reflection;
using Log4Net;

namespace ICSPControl.Logging
{
  public class Log4NetTraceListener : TraceListener
  {
    private readonly ILog mLog;

    public Log4NetTraceListener()
    {
      mLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }

    public Log4NetTraceListener(ILog log)
    {
      mLog = log;
    }

    public override void Write(string message)
    {
      if(mLog != null)
        mLog.Trace(message);
    }

    public override void WriteLine(string message)
    {
      if(mLog != null)
        mLog.Trace(message);
    }
  }
}