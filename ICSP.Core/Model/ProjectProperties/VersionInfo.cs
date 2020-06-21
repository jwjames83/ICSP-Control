using Newtonsoft.Json;

namespace ICSP.Core.Model.ProjectProperties
{
  public class VersionInfo
  {
    [JsonProperty("formatVersion", Order = 1, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int FormatVersion { get; set; }

    [JsonProperty("graphicsVersion", Order = 2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int GraphicsVersion { get; set; }

    [JsonProperty("g5appsVersion", Order = 3, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int G5AppsVersion { get; set; }

    [JsonProperty("fileVersion", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
    public int FileVersion { get; set; }

    [JsonProperty("designVersion", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
    public int DesignVersion { get; set; }
  }
}