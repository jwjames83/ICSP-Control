using System.Collections.Generic;
using System.Runtime.Serialization;

using TP.Design.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TP.Design.Model.ProjectProperties
{
  public class Resource
  {
    [JsonExtensionData]
    private readonly IDictionary<string, JToken> mAdditionalData;

    public Resource()
    {
      mAdditionalData = new Dictionary<string, JToken>();
    }

    [JsonProperty("type", Order = 1)]
    public string Type { get; set; }

    [JsonProperty("name", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("protocol", Order = 3)]
    public ProtocolType Protocol { get; set; }

    [JsonProperty("user", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
    public string User { get; set; }

    [JsonIgnore]
    public bool PasswordEncrypted { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    [JsonProperty("host", Order = 6, NullValueHandling = NullValueHandling.Ignore)]
    public string Host { get; set; }

    [JsonProperty("path", Order = 7, NullValueHandling = NullValueHandling.Ignore)]
    public string Path { get; set; }

    [JsonProperty("file", Order = 8, NullValueHandling = NullValueHandling.Ignore)]
    public string File { get; set; }

    [JsonProperty("refresh", Order = 9, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RefreshRate { get; set; }

    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("preserve", Order = 10, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool RefreshOnlyAtPanelStartup { get; set; }

    [JsonProperty("format", Order = 11)]
    public FormatType Format { get; set; } // xport-s, csv-headers, csv

    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("forceReload", Order = 12, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool ForceReload { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      /*
      XML:
      <password>9EE9C277D087E86A</password>
      <password encrypted="1">9EE9C277D087E86A</password>

      JSON:
      "password": "9EE9C277D087E86A",
      "password": { "encrypted": "1", "#text": "9EE9C277D087E86A"},
      */

      if(mAdditionalData.TryGetValue("password", out var passwordToken))
      {
        switch(passwordToken)
        {
          case JValue value:
          {
            Password = (string)value;
            break;
          }
          case JObject obj:
          {
            PasswordEncrypted = (int)(obj["encrypted"] ?? 0) == 1;
            Password = (string)obj["#text"];
            break;
          }
        }
      }
    }
  }
}
