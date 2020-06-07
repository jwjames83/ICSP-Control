using System;

using Newtonsoft.Json.Linq;

namespace ICSP.WebProxy.Json
{
  public static class NewtonsoftExtensions
  {
    public static void Rename(this JToken token, string name)
    {
      var lParent = token.Parent ?? throw new InvalidOperationException("The parent is missing.");

      lParent.Replace(new JProperty(name, token));
    }
  }
}
