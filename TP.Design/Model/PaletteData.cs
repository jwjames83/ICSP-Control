using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TP.Design.Model
{
  public class PaletteData
  {
    [JsonExtensionData]
    private readonly IDictionary<string, JToken> mAdditionalData;

    public PaletteData()
    {
      mAdditionalData = new Dictionary<string, JToken>();

      PaletteDataItems = new Dictionary<int, PaletteDataItem>();
    }

    [JsonProperty("name", Order = 1)]
    public string Name { get; set; }

    [JsonIgnore]
    public Dictionary<int, PaletteDataItem> PaletteDataItems { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      try
      {
        if(mAdditionalData.TryGetValue("paletteData", out var paletteData))
        {
          Name = paletteData["name"]?.ToString();

          if(paletteData["color"] != null)
          {
            var lPaletteDataList = paletteData["color"]?.ToObject<List<PaletteDataItem>>();

            foreach(var paletteDataItem in lPaletteDataList)
            {
              if(!PaletteDataItems.ContainsKey(paletteDataItem.Index))
                PaletteDataItems.Add(paletteDataItem.Index, paletteDataItem);
            }
          }
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}
