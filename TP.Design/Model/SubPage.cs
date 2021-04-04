using System.ComponentModel;

using TP.Design.Json;

using Newtonsoft.Json;

namespace TP.Design.Model
{
  public class SubPage : Page
  {
    /// <summary>
    /// Popup type ("popup", "subpage")
    /// </summary>
    [JsonProperty("popupType", Order = 2)]
    public PopupType PopupType { get; set; }

    // Page:
    // [JsonProperty("pageID", Order = 3)]
    // [JsonProperty("name", Order = 4)]
    // [JsonProperty("description", Order = 5)]

    /// <summary>
    /// Left
    /// </summary>
    [JsonProperty("left", Order = 6)]
    public int Left { get; set; }

    /// <summary>
    /// Top
    /// </summary>
    [JsonProperty("top", Order = 7)]
    public int Top { get; set; }

    // Page
    // [JsonProperty("width", Order = 8)]
    // [JsonProperty("height", Order = 9)]

    /// <summary>
    /// Reset Pos On Show
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("resetPos", Order = 10, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool ResetPosOnShow { get; set; }

    /// <summary>
    /// Group
    /// </summary>
    [JsonProperty("group", Order = 11, NullValueHandling = NullValueHandling.Ignore)]
    public string Group { get; set; }

    /// <summary>
    /// Timeout
    /// </summary>
    [JsonProperty("timeout", Order = 12, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Timeout { get; set; }

    /// <summary>
    /// Modal
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("modal", Order = 13, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Modal { get; set; }

    /// <summary>
    /// Show Effect
    /// </summary>
    [JsonProperty("showEffect", Order = 14, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ShowEffect { get; set; }

    /// <summary>
    /// Show Effect X Pos
    /// </summary>
    [JsonProperty("showLocX", Order = 15, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ShowLocX { get; set; }

    /// <summary>
    /// Show Effect Y Pos
    /// </summary>
    [JsonProperty("showLocY", Order = 16, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ShowLocY { get; set; }

    /// <summary>
    /// Show Effect Time
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("showTime", Order = 17, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ShowTime { get; set; }

    /// <summary>
    /// Hide Effect
    /// </summary>
    [JsonProperty("hideEffect", Order = 18, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int HideEffect { get; set; }

    /// <summary>
    /// Hide Effect X Pos
    /// </summary>
    [JsonProperty("hideLocX", Order = 19, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int HideLocX { get; set; }

    /// <summary>
    /// Hide Effect Y Pos
    /// </summary>
    [JsonProperty("hideLocY", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int HideLocY { get; set; }

    /// <summary>
    /// Hide Effect Time
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("hideTime", Order = 21, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int HideTime { get; set; }

    /// <summary>
    /// Collapse Direction (G5 Only)
    /// </summary>
    [JsonProperty("collapseDirection", Order = 22, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int CollapseDirection { get; set; }

    /// <summary>
    /// Collapse Offset (G5 Only)
    /// </summary>
    [JsonProperty("collapseOffset", Order = 23, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int CollapseOffset { get; set; }

    /// <summary>
    /// Show Open (G5 Only)
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("collapseShowOpen", Order = 24, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool CollapseShowOpen { get; set; }
  }
}
