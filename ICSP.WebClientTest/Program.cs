using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace ICSP.WebClientTest
{
  static class Program
  {
    public const int KEYSTROKE_TRANSMIT_INTERVAL_MS = 100;
    public const int CLOSE_SOCKET_TIMEOUT_MS = 10000;

    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new TestForm());
    }

    public static void ReportException(Exception ex, [CallerMemberName]string location = "(Caller name not set)")
    {
      Console.WriteLine($"\n{location}:\n  Exception {ex.GetType().Name}: {ex.Message}");
      if(ex.InnerException != null) Console.WriteLine($"  Inner Exception {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
    }
  }
}
