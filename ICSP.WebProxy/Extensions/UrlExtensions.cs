using System;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ICSP.WebProxy.Extensions
{
  public static class UrlExtensions
  {
    public static string GetBaseUrl(this ControllerBase data)
    {
      var lController /**/ = data.RouteData.Values["controller"]?.ToString();
      var lAction     /**/ = data.RouteData.Values["action"]?.ToString();

      var lUrl = data.HttpContext.Request.GetEncodedUrl();

      var lPath = string.Concat(
        lController == null ? string.Empty : $"/{lController}",
        lAction == null ? string.Empty : $"/{lAction}");

      if(lUrl.EndsWith(lPath, StringComparison.OrdinalIgnoreCase))
        return lUrl.Remove(lUrl.LastIndexOf(lPath, StringComparison.OrdinalIgnoreCase));
      
      lPath += "/";

      if(lUrl.EndsWith(lPath, StringComparison.OrdinalIgnoreCase))
        return lUrl.Remove(lUrl.LastIndexOf(lPath, StringComparison.OrdinalIgnoreCase));

      return lUrl;
    }
  }
}
