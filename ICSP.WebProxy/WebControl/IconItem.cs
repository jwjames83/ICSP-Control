
using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class IconItem
  {
    [JsonProperty("file", Order = 1)]
    public string File { get; set; }
  }
}
