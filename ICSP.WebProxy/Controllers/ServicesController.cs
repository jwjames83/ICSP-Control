using System;

using ICSP.Core.Logging;

using Microsoft.AspNetCore.Mvc;

namespace ICSP.WebProxy.Controllers
{
  public class ServicesController : Controller
  {
    private IApplicationRestart mApplicationLifetime;

    public ServicesController(IApplicationRestart appLifetime)
    {
      mApplicationLifetime = appLifetime;
    }

    // Get: /Services/Restart
    public IActionResult Restart()
    {
      try
      {
        Logger.LogInfo("Restarting App");

        mApplicationLifetime.RestartApplication();
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }
      
      return Redirect("/Services/Wait");
    }

    // Get: /Services/Shutdown
    public IActionResult Shutdown()
    {
      try
      {
        Logger.LogInfo("Shutdown App");

        mApplicationLifetime.StopApplication();
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }

      return View();
    }

    // Get: /Services/Wait
    public IActionResult Wait()
    {
      return View();
    }

    // Get: /Services/Success
    public IActionResult Success()
    {
      return View();
    }
  }
}
