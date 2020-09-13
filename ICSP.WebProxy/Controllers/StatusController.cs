using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICSP.WebProxy.Controllers
{
  public class StatusController : Controller
  {
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index(int statusCode)
    {
      ViewData["StatusCode"] = statusCode;

      return View();
    }
  }
}
