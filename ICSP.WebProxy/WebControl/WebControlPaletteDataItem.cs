using Newtonsoft.Json;

using TP.Design.Model;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlPaletteDataItem
  {
    [JsonIgnore]
    public int Index { get; set; }

    [JsonProperty("name", Order = 2)]
    public string Name { get; set; }

    [JsonProperty("color", Order = 3)]
    public string Color { get; set; }

    public static implicit operator WebControlPaletteDataItem(PaletteDataItem data)
    {
      return new WebControlPaletteDataItem()
      {
        Index = data.Index,
        Name = data.Name,
        Color = data.Value,
      };
    }

    public static implicit operator PaletteDataItem(WebControlPaletteDataItem data)
    {
      return new PaletteDataItem()
      {
        Index = data.Index,
        Name = data.Name,
        Value = data.Color,
      };
    }
  }
}