using ICSP.WebProxy.Models;

using Microsoft.AspNetCore.Mvc;

namespace ICSP.WebProxy.Controllers
{
  public class SetupController : Controller
  {
    // GET: /Home/
    public IActionResult Index()
    {
      return View(new DeviceConfiguration());
    }

    // GET: /Home/
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index([Bind("PanelType,PortCount,DeviceName")] DeviceConfiguration configuration)
    {
      if(ModelState.IsValid)
      {
        return RedirectToActionPreserveMethod(nameof(Configure));
      }

      return View(configuration);
    }

    // GET: /Home/Configure/ 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Configure([Bind("PanelType,PortCount,DeviceName")] DeviceConfiguration configuration)
    {
      if(ModelState.IsValid)
      {
        return View(configuration);
      }

      return View(nameof(Index));
    }
  }
}
