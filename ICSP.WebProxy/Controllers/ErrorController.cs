using System;
using System.Diagnostics;

using ICSP.WebProxy.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

namespace ICSP.WebProxy.Controllers
{
  public class ErrorController : Controller
  {
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index(int statusCode)
    {
      // Handling exceptions ...
      // IsDevelopment: -> UseDeveloperExceptionPage
      // Otherwise    : -> return Json
      if(HttpContext.Features.Get<IExceptionHandlerPathFeature>() is IExceptionHandlerPathFeature lException)
        return Json(new
        {
          path = lException.Path,
          type = lException.Error.GetType().FullName,
          error = lException.Error.Message
        });

      var lReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

      // index.html not found => redirect to setup ...
      if(statusCode == 404 && lReExecuteFeature?.OriginalPath == "/")
        return Redirect("/Setup");

      var lReason = ReasonPhrases.GetReasonPhrase(statusCode);
      if(string.IsNullOrWhiteSpace(lReason))
        lReason = "Unknown";

      var lModel = new ErrorViewModel
      {
        StatusCode = statusCode,
        Reason = lReason,
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
        Path = Request.Path,
        QueryString = Request.QueryString.Value,
      };

      if(lReExecuteFeature != null)
      {
        lModel.Path = lReExecuteFeature?.OriginalPath;
        lModel.QueryString = lReExecuteFeature?.OriginalQueryString;
      }

      var lReferer = Request.Headers[HeaderNames.Referer];

      // Request is from html page => return Json
      if(!string.IsNullOrWhiteSpace(lReferer))
      {
        if(Request.ContentType != null)
          Console.WriteLine(Request.ContentType);

        var lIsScript = lModel.Path?.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ?? false;
        var lIsChrome = Request.Headers[HeaderNames.UserAgent].ToString().Contains("Chrome", StringComparison.OrdinalIgnoreCase);

        // Fix: Chrome/Inspector
        // Preview or response tab is empty on network tab of inspector (This request has no response data available.)
        // User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36
        if(lIsScript && lIsChrome)
        {
          // Add Custom header
          Response.Headers["JsonResponse"] = JsonConvert.SerializeObject(lModel);

          // Preview or response tab works only correct, if status code returns a success response;
          // Response.StatusCode = 204; // No Content;
        }

        return Json(lModel);
      }

      return View(lModel);
    }
  }
}
