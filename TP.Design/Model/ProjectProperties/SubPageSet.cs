using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TP.Design.Model.ProjectProperties
{
  public class SubPageSet
  {
    [JsonExtensionData]
    private readonly IDictionary<string, JToken> mAdditionalData;

    public SubPageSet()
    {
      mAdditionalData = new Dictionary<string, JToken>();

      Items = new List<SubPageSetItem>();
    }

    /*
    <subPageSets>
      <subPageSetEntry id="1">
        <name>SPSet 1</name>
        <pgWidth>0</pgWidth>
        <pgHeight>0</pgHeight>
      </subPageSetEntry>
      <subPageSetEntry id="2">
        <name>SPSet 2</name>
        <pgWidth>15</pgWidth>
        <pgHeight>15</pgHeight>
        <items>
          <item index="0">
            <pageID>503</pageID>
            <pageName>Sub-page 1</pageName>
          </item>
          <item index="1">
            <pageID>504</pageID>
            <pageName>Sub-page 2</pageName>
          </item>
        </items>
      </subPageSetEntry>
    </subPageSets>
    */

    [JsonProperty("id", Order = 1)]
    public int Id { get; set; }

    [JsonProperty("name", Order = 2)]
    public string Name { get; set; }

    [JsonProperty("pgWidth", Order = 3)]
    public int PgWidth { get; set; }

    [JsonProperty("pgHeight", Order = 4)]
    public int PgHeight { get; set; }

    [JsonIgnore]
    public List<SubPageSetItem> Items { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      try
      {
        if(mAdditionalData.TryGetValue("items", out var itemsToken))
        {
          if(itemsToken["item"] != null)
            Items.AddRange(itemsToken["item"]?.ToObject<List<SubPageSetItem>>());
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}