using Newtonsoft.Json;

namespace TP.Design.Model.ProjectProperties
{
  public class Palette
  {
    [JsonProperty("name", Order = 1)]
    public string Name { get; set; }

    [JsonProperty("file", Order = 2)]
    public string File { get; set; }

    [JsonProperty("paletteID", Order = 3)]
    public int PaletteID { get; set; }
  }
}
