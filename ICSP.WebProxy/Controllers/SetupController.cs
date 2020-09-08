using System;
using System.IO;
using ICSP.Core.Environment;
using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Models;
using ICSP.WebProxy.Properties;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
    public IActionResult Configure([Bind("PanelType,PortCount,DeviceName")] DeviceConfiguration configuration, [FromServices] IOptions<ProxyConfig> config)
    {
      if(ModelState.IsValid)
      {
        try
        {
          var lConnectionConfig = config.Value.GetConfig(HttpContext);

          // Create js\project.js
          var lProject = Resources.DefaultProject;
          var lBaseDirectory = new DirectoryInfo(lConnectionConfig?.BaseDirectory ?? string.Empty);

          lProject = lProject?
            .Replace("{PanelType}", configuration.PanelType)
            .Replace("{PortCount}", configuration.PortCount.ToString())
            .Replace("{DeviceName}", configuration.DeviceName);

          var lFileNameProject = string.Format(@"{0}\js\{1}", lBaseDirectory.FullName, "project.js");

          System.IO.File.WriteAllText(lFileNameProject, lProject);

          // Create index.html
          var lHtml = Resources.MainPage;

          lHtml = lHtml?
            .Replace("{title}", configuration.DeviceName)
            .Replace("{width}", "1280")
            .Replace("{height}", "800");

          // LastBuild & Version
          lHtml = lHtml?
            .Replace("{LastBuild}", ProgramProperties.CompileDate.ToString("yyyy-MM-dd HH:mm:ss:ffffff")) // 2020-08-27 12:03:37.946386
            .Replace("{Title}", ProgramProperties.Title)
            .Replace("{version}", ProgramProperties.Version.ToString());

          var lFileNameMainPage = string.Format(@"{0}\{1}", lBaseDirectory.FullName, "index.html");

          System.IO.File.WriteAllText(lFileNameMainPage, lHtml);

          // Redirect to BaseUrl
          var lBaseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}{lConnectionConfig?.RequestPath}";

          return Redirect(lBaseUrl);
        }
        catch(Exception ex)
        {
          Logger.LogError(ex);
        }

        return View(configuration);
      }

      return View(nameof(Index));
    }
  }
}
