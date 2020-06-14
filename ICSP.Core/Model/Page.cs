using System.Collections.Generic;

using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class Page
  {
    /// <summary>
    /// Page type ("page", "subpage")
    /// </summary>
    [JsonProperty("type", Order = 1)]
    public PageType Type { get; set; }

    // [JsonProperty("popupType", Order = 2)]
    
    /// <summary>
    /// PageID
    /// </summary>
    [JsonProperty("pageID", Order = 3)]
    public int PageID { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name", Order = 4)]
    public string Name { get; set; }

    // [JsonProperty("left", Order = 5)]

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

    // [JsonProperty("group", Order = 9)]

    [JsonProperty("button", Order = 10)]
    public IList<Button> Buttons { get; set; }
  }
}
