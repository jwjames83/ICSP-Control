using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class Icon
  {
    [JsonProperty("number", Order = 1)]
    public int Number { get; set; }

    [JsonProperty("file", Order = 2)]
    public string File { get; set; }
  }
}
