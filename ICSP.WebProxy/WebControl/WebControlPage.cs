using System.Collections.Generic;
using System.Linq;

using ICSP.Core.Model;

using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlPage
  {
    /// <summary>
    /// Page type ("page", "subpage")
    /// </summary>
    [JsonIgnore]
    public PageType Type { get; set; }

    // SubPage:
    // [JsonProperty("popupType", Order = 2)]

    /// <summary>
    /// PageID
    /// </summary>
    [JsonProperty("pageID", Order = 3)]
    public int PageID { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonIgnore]
    public string Name { get; set; }

    // SubPage:
    // [JsonProperty("left", Order = 5)]

    // SubPage:
    // [JsonProperty("top", Order = 6)]

    /// <summary>
    /// Width
    /// </summary>
    [JsonProperty("width", Order = 7)]
    public int Width { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    [JsonProperty("height", Order = 8)]
    public int Height { get; set; }

    // SubPage:
    // [JsonProperty("group", Order = 9)]

#warning Correct Order: 10
    [JsonProperty("buttons", Order = 0)]
    public List<WebControlButton> Buttons { get; set; }

    [JsonProperty("states", Order = 11)]
    public Dictionary<int, WebControlState> States { get; set; }

    public static implicit operator WebControlPage(Page page)
    {
      return new WebControlPage()
      {
        Type = page.Type,
        PageID = page.PageID,
        Name = page.Name,
        Width = page.Width,
        Height = page.Height,
        Buttons = page.Buttons?.Select(s => (WebControlButton)s)?.ToList() ?? new List<WebControlButton>(),
        States = page.States?.ToDictionary(k => k.Number, e => (WebControlState)e) ?? new Dictionary<int, WebControlState>(),
      };
    }

    public static implicit operator Page(WebControlPage page)
    {
      return new Page()
      {
        Type = page.Type,
        PageID = page.PageID,
        Name = page.Name,
        Width = page.Width,
        Height = page.Height,
        Buttons = page.Buttons?.Select(s => (Button)s)?.ToList(),
        States = page.States?.Values?.Select(s => (State)s)?.ToList(),
      };
    }
  }
}
