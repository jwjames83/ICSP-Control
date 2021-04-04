using Newtonsoft.Json;

namespace TP.Design.Model
{
  public class PaletteDataItem
  {
    [JsonProperty("index", Order = 1)]
    public int Index { get; set; }

    [JsonProperty("name", Order = 2)]
    public string Name { get; set; }

    [JsonProperty("#text", Order = 3, NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; set; }
  }
}
