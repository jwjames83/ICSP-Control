using ICSP.WebProxy.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ICSP.WebProxy
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      var lConfig = Configuration.GetSection(nameof(ProxyConfig)).Get<ProxyConfig>() ?? new ProxyConfig();

      lConfig.Configure();

      Program.ProxyConfig = lConfig;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddWebSocketManager();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      var lFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
      var lProvider = lFactory.CreateScope().ServiceProvider;

      if(env.IsDevelopment())
        app.UseDeveloperExceptionPage();

      app.UseRouting();

      // Accept web socket requests
      app.UseWebSockets();

      var lOptions = new DefaultFilesOptions();

      lOptions.DefaultFileNames.Clear();
      lOptions.DefaultFileNames.Add("index.html");

      app.UseDefaultFiles(lOptions);

      app.UseStaticFiles();

      app.MapWebSocketManager("", lProvider.GetService<ConnectedClient>());
    }
  }
}
