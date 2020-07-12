using System.Collections.Generic;
using System.Linq;

using ICSP.Core.Model;

using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlSubPage : WebControlPage
  {
    /// <summary>
    /// Popup type ("popup", "subpage")
    /// </summary>
    [JsonProperty("popupType", Order = 2, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public PopupType PopupType { get; set; }

    /// <summary>
    /// Left
    /// </summary>
    [JsonProperty("left", Order = 5, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Left { get; set; }

    /// <summary>
    /// Top
    /// </summary>
    [JsonProperty("top", Order = 6, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Top { get; set; }

    /// <summary>
    /// Group
    /// </summary>
    [JsonProperty("group", Order = 9, NullValueHandling = NullValueHandling.Ignore)]
    public string Group { get; set; }

    public static implicit operator WebControlSubPage(SubPage page)
    {
      return new WebControlSubPage()
      {
        Type = page.Type,
        PopupType = page.PopupType,
        PageID = page.PageID,
        Name = page.Name,
        Left = page.Left,
        Top = page.Top,
        Width = page.Width,
        Height = page.Height,
        Group = page.Group,
        Buttons = page.Buttons?.Select(s => (WebControlButton)s)?.ToList() ?? new List<WebControlButton>(),
        States = page.States?.ToDictionary(k => k.Number, e => (WebControlState)e) ?? new Dictionary<int, WebControlState>(),
      };
    }

    public static implicit operator SubPage(WebControlSubPage page)
    {
      return new SubPage()
      {
        Type = page.Type,
        PopupType = page.PopupType,
        PageID = page.PageID,
        Name = page.Name,
        Left = page.Left,
        Top = page.Top,
        Width = page.Width,
        Height = page.Height,
        Group = page.Group,
        Buttons = page.Buttons?.Select(s => (Button)s)?.ToList(),
        States = page.States?.Values?.Select(s => (State)s)?.ToList(),
      };
    }
  }
}
