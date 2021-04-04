using Newtonsoft.Json;

namespace TP.Design.Model
{
  public class PageFlip
  {
    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type", Order = 1)]
    public PageFlipType Type { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    [JsonProperty("#text", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; set; }
  }
}
