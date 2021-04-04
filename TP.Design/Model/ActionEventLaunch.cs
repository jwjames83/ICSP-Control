using System.ComponentModel;

using Newtonsoft.Json;

namespace TP.Design.Model
{
  public class ActionEventLaunch
  {
    /// <summary>
    /// Item
    /// </summary>
    [JsonProperty("item", Order = 1)]
    public int Item { get; set; }

    /// <summary>
    /// Action
    /// </summary>
    [JsonProperty("action", Order = 2)]
    public LaunchAction Action { get; set; }

    /// <summary>
    /// ID
    /// </summary>
    [JsonProperty("id", Order = 3)]
    public int ID { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    [JsonProperty("#text", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; set; }
  }
}