﻿using System;
using System.IO;

using ICSP.Core.Environment;
using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Extensions;
using ICSP.WebProxy.Models;
using ICSP.WebProxy.Properties;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ICSP.WebProxy.Controllers
{
  public class InstallController : Controller
  {
    // GET: /Install/
    public IActionResult Index()
    {
      return View(new DeviceConfiguration());
    }

    // Post: /Install/
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

    // Post: /Install/Configure/ 
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
          var lHtml = Resources.MainPage_V02;

          lHtml = lHtml?
            .Replace("[Title]", string.IsNullOrWhiteSpace(configuration.DeviceName) ? "WebControl" : configuration.DeviceName)
            .Replace("[Width]", "1280")
            .Replace("[Height]", "800");

          // LastBuild & Version
          lHtml = lHtml?
            .Replace("[LastBuild]", ProgramProperties.CompileDate.ToString("yyyy-MM-dd HH:mm:ss:ffffff")) // 2020-08-27 12:03:37.946386
            .Replace("[Version]", ProgramProperties.Version.ToString());

          var lFileNameMainPage = string.Format(@"{0}\{1}", lBaseDirectory.FullName, "index.html");

          System.IO.File.WriteAllText(lFileNameMainPage, lHtml);

          // Redirect to BaseUrl
          return Redirect(this.GetBaseUrl());
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
