using System.Text.Json.Serialization;

namespace ICSP.WebProxy.Models
{
  public class ErrorViewModel
  {
    public int StatusCode { get; set; }

    public string Reason { get; set; }

    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public string RequestId { get; set; }

    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public bool ShowRequestId
    {
      get
      {
        return !string.IsNullOrEmpty(RequestId);
      }
    }

    public string Path { get; internal set; }

    public string QueryString { get; internal set; }
  }
}
