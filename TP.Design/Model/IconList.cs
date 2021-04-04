using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TP.Design.Model
{
  public class IconList
  {
    [JsonExtensionData]
    private readonly IDictionary<string, JToken> mAdditionalData;

    public IconList()
    {
      mAdditionalData = new Dictionary<string, JToken>();

      Icons = new List<Icon>();
    }

    [JsonProperty("maxIcons", Order = 1)]
    public int MaxIcons { get; set; }

    [JsonIgnore]
    public List<Icon> Icons { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      try
      {
        if(mAdditionalData.TryGetValue("icon", out var allIcons))
          Icons.AddRange(allIcons?.ToObject<List<Icon>>());
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}
