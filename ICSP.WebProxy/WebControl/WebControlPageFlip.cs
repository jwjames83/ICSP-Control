using ICSP.Core.Model;

using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlPageFlip
  {
    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type", Order = 1)]
    public PageFlipType Type { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    [JsonProperty("value", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; set; }

    public static implicit operator WebControlPageFlip(PageFlip data)
    {
      return new WebControlPageFlip()
      {
        Type = data.Type,
        Value = data.Value,
      };
    }

    public static implicit operator PageFlip(WebControlPageFlip data)
    {
      return new PageFlip()
      {
        Type = data.Type,
        Value = data.Value,
      };
    }
  }
}
