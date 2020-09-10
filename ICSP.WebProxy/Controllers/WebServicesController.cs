using System;

using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ICSP.WebProxy.Controllers
{
  public class WebServicesController : Controller
  {
    private IHostApplicationLifetime ApplicationLifetime { get; set; }

    public WebServicesController(IHostApplicationLifetime appLifetime)
    {
      ApplicationLifetime = appLifetime;
    }

    // Get: /WebServices/Restart
    public IActionResult Restart([FromServices] IOptions<ProxyConfig> config)
    {
      try
      {
        var lConnectionConfig = config.Value.GetConfig(HttpContext);

        Program.Restart();

        // Redirect to BaseUrl
        return Redirect(this.GetBaseUrl());
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }

      return Content("Done");
    }

    // Get: /WebServices/Shutdown
    public IActionResult Shutdown()
    {
      try
      {
        ApplicationLifetime.StopApplication();

        return Content("Done");
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }

      return Content("Done");
    }
  }
}
