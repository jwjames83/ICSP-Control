using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ICSP.Logging;

using ICSPControl.Dialogs;
using ICSPControl.Environment;
using ICSPControl.Properties;

namespace ICSPControl
{
  public static class ShellApplication
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    [HandleProcessCorruptedStateExceptions]
    static void Main()
    {
      // Add the event handler for handling UI thread exceptions to the event.
      Application.ThreadException += OnThreadException;

      // Set the unhandled exception mode to force for all Windows Forms errors.
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

      // Add the event handler for handling non-UI thread exceptions to the event. 
      AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

      // Ensure unobserved task exceptions (unawaited async methods returning Task or Task<T>) are handled
      TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

      // Initializes the Log system
      LoggingConfigurator.Configure(new LoggingConfiguration() { LogLevel = Settings.Default.LogLevel }, false);

      Logger.LogLevel = Settings.Default.LogLevel;

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.ApplicationExit += OnApplicationExit;

      var lExists = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1;

      // Check to see if Application is already running
      if(lExists)
      {
        InfoMessageBox.Show("Applikation wurde bereits gestartet.");
        return;
      }

      Logger.LogVerbose("ExecutablePath: " + ProgramProperties.ExecutablePath);

      try
      {
        Application.Run(new DlgMain());
      }
      catch(Exception ex)
      {
        Logger.LogError("ApplicationException: {0}", ex.Message);
        Logger.LogError(ex);

        MessageService.CreateMsg(null, string.Format("ApplicationException: {0}", ex.Message));
      }
    }

    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
      Logger.LogError("ThreadException: {0}", e.Exception.Message);
      MessageService.CreateMsg(null, string.Format("ThreadException: {0}", e.Exception.Message));
    }

    static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
      e.SetObserved();

      Logger.LogError("UnobservedTaskException: {0}", e.Exception.Message);
      MessageService.CreateMsg(null, string.Format("UnobservedTaskException: {0}", e.Exception.Message));
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      var lEx = e.ExceptionObject as Exception;

      Logger.LogError("UnhandledException: {0}", lEx.Message);
      MessageService.CreateMsg(null, string.Format("UnhandledException: {0}", lEx.Message));
    }

    private static void OnApplicationExit(object sender, EventArgs e)
    {
      Logger.LogInfo("{0} wurde beendet", ProgramProperties.Title);
    }
  }
}