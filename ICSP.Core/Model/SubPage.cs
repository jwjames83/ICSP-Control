using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class SubPage : Page
  {
    /// <summary>
    /// Popup type ("popup", "subpage")
    /// </summary>
    [JsonProperty("popupType", Order = 2)]
    public PopupType PopupType { get; set; }

    /// <summary>
    /// Left
    /// </summary>
    [JsonProperty("left", Order = 5)]
    public int Left { get; set; }

    /// <summary>
    /// Top
    /// </summary>
    [JsonProperty("top", Order = 6)]
    public int Top { get; set; }

    /// <summary>
    /// Group
    /// </summary>
    [JsonProperty("group", Order = 9, NullValueHandling = NullValueHandling.Ignore)]
    public string Group { get; set; }
  }
}
