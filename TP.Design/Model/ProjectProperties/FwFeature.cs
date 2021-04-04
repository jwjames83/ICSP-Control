using Newtonsoft.Json;

namespace TP.Design.Model.ProjectProperties
{
  public class FwFeature
  {
    [JsonProperty("featureID", Order = 1)]
    public string FeatureID { get; set; }

    [JsonProperty("usageCount", Order = 2)]
    public int UsageCount { get; set; }
  }
}
