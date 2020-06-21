
using Newtonsoft.Json;

namespace ICSP.Core.Model.ProjectProperties
{
  public class PageEntry
  {
    [JsonProperty("type", Order = 1)]
    public string Type { get; set; }

    [JsonProperty("name", Order = 2)]
    public string Name { get; set; }

    [JsonProperty("pageID", Order = 3)]
    public int PageID { get; set; }

    [JsonProperty("file", Order = 4)]
    public string File { get; set; }

    [JsonProperty("group", Order = 5)]
    public string Group { get; set; }

    [JsonProperty("isValid", Order = 6)]
    public int IsValid { get; set; }

    [JsonProperty("popupType", Order = 7)]
    public string PopupType { get; set; }
  }
}