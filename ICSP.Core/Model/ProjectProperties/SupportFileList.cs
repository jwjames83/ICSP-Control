using Newtonsoft.Json;

namespace ICSP.Core.Model.ProjectProperties
{
  public class SupportFileList
  {
    [JsonProperty("mapFile", Order = 1)]
    public string MapFile { get; set; }

    [JsonProperty("colorFile", Order = 2)]
    public string ColorFile { get; set; }

    [JsonProperty("fontFile", Order = 3)]
    public string FontFile { get; set; }

    [JsonProperty("themeFile", Order = 4)]
    public string ThemeFile { get; set; }

    [JsonProperty("externalButtonFile", Order = 5)]
    public string ExternalButtonFile { get; set; }

    [JsonProperty("appFile", Order = 6)]
    public string AppFile { get; set; }

    [JsonProperty("logFile", Order = 7)]
    public string LogFile { get; set; }
  }
}