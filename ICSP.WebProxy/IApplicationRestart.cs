using System.Threading;

using Microsoft.Extensions.Hosting;

namespace ICSP.WebProxy
{
  public interface IApplicationRestart : IHostApplicationLifetime
  {
    /// <summary>
    /// Requests the restart of the current application.
    /// </summary>
    public void RestartApplication();

    /// <summary>
    /// Triggered when the application host is performing a graceful restart.
    /// </summary>
    CancellationToken ApplicationRestart { get; }
  }
}
