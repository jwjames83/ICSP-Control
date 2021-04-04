using System.ComponentModel;

using Newtonsoft.Json;

namespace TP.Design.Model
{
  public class ActionEventCommand
  {
    /// <summary>
    /// Item
    /// </summary>
    [JsonProperty("item", Order = 1)]
    public int Item { get; set; }

    /// <summary>
    /// Port
    /// </summary>
    [JsonProperty("port", Order = 2)]
    public int Port { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    [JsonProperty("#text", Order = 3, NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; set; }
  }
}
