using Newtonsoft.Json;

namespace ICSP.Core.Model.ProjectProperties
{
  public class DropGroupItem
  {
    [JsonProperty("index", Order = 1)]
    public int Index { get; set; }

    [JsonProperty("pgID", Order = 1)]
    public int PgID { get; set; }

    [JsonProperty("pgName", Order = 1)]
    public string PgName { get; set; }

    [JsonProperty("btnID", Order = 1)]
    public int BtnID { get; set; }

    [JsonProperty("btnName", Order = 1)]
    public string BtnName { get; set; }

    [JsonProperty("port", Order = 1)]
    public int Port { get; set; }

    [JsonProperty("code", Order = 1)]
    public int Code { get; set; }
  }
}