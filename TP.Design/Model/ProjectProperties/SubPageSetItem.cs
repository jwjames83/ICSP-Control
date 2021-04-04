using Newtonsoft.Json;

namespace TP.Design.Model.ProjectProperties
{
  public class SubPageSetItem
  {
    [JsonProperty("index", Order = 1)]
    public int Index { get; set; }

    [JsonProperty("pageID", Order = 2)]
    public int PageID { get; set; }

    [JsonProperty("pageName", Order = 3)]
    public string PageName { get; set; }
  }
}