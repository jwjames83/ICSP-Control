using ICSP.Core.Model;

using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlIcon
  {
    [JsonProperty("number", Order = 1)]
    public int Number { get; set; }

    [JsonProperty("file", Order = 2)]
    public string File { get; set; }

    public static implicit operator WebControlIcon(Icon icon)
    {
      return new WebControlIcon()
      {
        Number = icon.Number,
        File = icon.File,
      };
    }

    public static implicit operator Icon(WebControlIcon icon)
    {
      return new Icon()
      {
        Number = icon.Number,
        File = icon.File,
      };
    }
  }
}
