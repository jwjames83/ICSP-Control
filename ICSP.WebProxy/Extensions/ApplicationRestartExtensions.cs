using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ICSP.WebProxy.Extensions
{
  public static class ApplicationRestartExtensions
  {
    public static IHostBuilder UseApplicationRestart(this IHostBuilder hostBuilder, CancellationTokenSource cancellationTokenSource = default)
    {
      return hostBuilder.ConfigureServices((hostContext, services) =>
      {
        var lServiceDescriptor = ServiceDescriptor.Singleton<IApplicationRestart>(
          provider => new HostApplicationRestart(provider.GetService<IHostApplicationLifetime>(), cancellationTokenSource));

        services.Replace(lServiceDescriptor);
      });
    }
  }
}
