using System;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core.Logging;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;

namespace ICSP.WebProxy
{
  public class HostApplicationRestart : IApplicationRestart
  {
    IHostApplicationLifetime mApplicationLifetime;

    private readonly CancellationTokenSource mCts;

    public HostApplicationRestart(IHostApplicationLifetime applicationLifetime, CancellationTokenSource cancellationTokenSource = default)
    {
      mApplicationLifetime = applicationLifetime;

      mCts = cancellationTokenSource;
    }

    // IHostApplicationLifetime

    public CancellationToken ApplicationStarted => mApplicationLifetime.ApplicationStarted;

    public CancellationToken ApplicationStopping => mApplicationLifetime.ApplicationStopping;

    public CancellationToken ApplicationStopped => mApplicationLifetime.ApplicationStopped;

    public void StopApplication() => mApplicationLifetime.StopApplication();

    public CancellationToken ApplicationRestart => mCts.Token;

    public void RestartApplication()
    {
      if(WindowsServiceHelpers.IsWindowsService())
      {
        Task.Run(async () =>
        {
          await Task.Delay(500);

          Logger.LogInfo("Environment.Exit ...");

          // If exit with exitCode != 0, then services subsystem will think,
          // that the service crashed and try restart it (If setup it in service settings)
          Environment.Exit(1);
        });
      }
      else
      {
        mCts.Cancel();
      }
    }
  }
}
