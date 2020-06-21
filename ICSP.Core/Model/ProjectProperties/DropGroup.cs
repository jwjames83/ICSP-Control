using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.Core.Model.ProjectProperties
{
  public class DropGroup
  {
    [JsonExtensionData]
    private IDictionary<string, JToken> mAdditionalData;

    public DropGroup()
    {
      mAdditionalData = new Dictionary<string, JToken>();

      DropGroupItems = new List<DropGroupItem>();
    }

    [JsonProperty("id", Order = 1)]
    public int Id { get; set; }

    [JsonProperty("name", Order = 2)]
    public string Name { get; set; }

    [JsonIgnore]
    public List<DropGroupItem> DropGroupItems { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      try
      {
        if(mAdditionalData.TryGetValue("dgItems", out var dropGroupsToken))
        {
          if(dropGroupsToken["dgItem"] != null)
            DropGroupItems.AddRange(dropGroupsToken["dgItem"]?.ToObject<List<DropGroupItem>>());
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}
