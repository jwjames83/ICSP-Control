using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TP.Design.Model.ProjectProperties
{
  public class ProjectInfo
  {
    [JsonExtensionData]
    private readonly IDictionary<string, JToken> mAdditionalData;

    public ProjectInfo()
    {
      mAdditionalData = new Dictionary<string, JToken>();
    }

    [JsonProperty("protection", Order = 1)]
    public ProtectionType Protection { get; set; }

    /// <summary>
    /// Password is Encrypted
    /// </summary>
    [JsonIgnore]
    public bool PasswordEncrypted { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [JsonIgnore]
    public string Password { get; set; }

    [JsonProperty("panelType", Order = 3)]
    public string PanelType { get; set; }

    [JsonProperty("fileRevision", Order = 4)]
    public string FileRevision { get; set; }

    [JsonProperty("userProfile", Order = 5)]
    public string UserProfile { get; set; }

    [JsonProperty("dealerId", Order = 6)]
    public string DealerId { get; set; }

    [JsonProperty("jobName", Order = 7)]
    public string JobName { get; set; }

    [JsonProperty("salesOrder", Order = 8)]
    public string SalesOrder { get; set; }

    [JsonProperty("purchaseOrder", Order = 9)]
    public string PurchaseOrder { get; set; }

    [JsonProperty("jobComment", Order = 10)]
    public string JobComment { get; set; }

    [JsonProperty("designerId", Order = 11)]
    public string DesignerId { get; set; }

    [JsonProperty("creationDate", Order = 12)]
    public string CreationDate { get; set; }

    [JsonProperty("revisionDate", Order = 13)]
    public string RevisionDate { get; set; }

    [JsonProperty("lastSaveDate", Order = 14)]
    public string LastSaveDate { get; set; }

    [JsonProperty("fileName", Order = 15)]
    public string FileName { get; set; }

    [JsonProperty("convLog", Order = 16)]
    public string ConvLog { get; set; }

    [JsonProperty("colorChoice", Order = 17)]
    public string ColorChoice { get; set; }

    [JsonProperty("specifyPortCount", Order = 18)]
    public int SpecifyPortCount { get; set; }

    [JsonProperty("specifyChanCount", Order = 19)]
    public int SpecifyChanCount { get; set; }

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