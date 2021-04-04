using ICSP.WebProxy.Configuration.Options;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Options
{
  public static class ServiceCollectionExtensions
  {
    public static void ConfigureWritable<T>(this IServiceCollection services, IConfigurationSection section, string file = "appsettings.json") where T : class, new()
    {
      services.Configure<T>(section);

      services.AddTransient<IWritableOptions<T>>(provider =>
      {
        var lEnvironment = provider.GetService<IWebHostEnvironment>();

        var lOptions = provider.GetService<IOptionsMonitor<T>>();

        return new WritableOptions<T>(lEnvironment, lOptions, section.Key, file);
      });
    }
  }
}
