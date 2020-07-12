
using ICSP.Core.Model.ProjectProperties;

using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlPageEntry
  {
    [JsonIgnore]
    public string Type { get; set; }

    [JsonProperty("name", Order = 2)]
    public string Name { get; set; }

    [JsonProperty("pageID", Order = 3)]
    public int PageID { get; set; }

    [JsonProperty("file", Order = 4)]
    public string File { get; set; }

    [JsonProperty("group", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("isValid", Order = 6)]
    public int IsValid { get; set; }

    [JsonProperty("popupType", Order = 7, NullValueHandling = NullValueHandling.Ignore)]
    public string PopupType { get; set; }

    public static implicit operator WebControlPageEntry(PageEntry page)
    {
      return new WebControlPageEntry()
      {
        Type = page.Type,
        Name = page.Name,
        PageID = page.PageID,
        File = page.File,
        Group = page.Group,
        IsValid = page.IsValid,
        PopupType = page.PopupType,
      };
    }

    public static implicit operator PageEntry(WebControlPageEntry page)
    {
      return new PageEntry()
      {
        Type = page.Type,
        Name = page.Name,
        PageID = page.PageID,
        File = page.File,
        Group = page.Group,
        IsValid = page.IsValid,
        PopupType = page.PopupType,
      };
    }
  }
}