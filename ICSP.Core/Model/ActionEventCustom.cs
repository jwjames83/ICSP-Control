using System.ComponentModel;

using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class ActionEventCustom
  {
    /// <summary>
    /// Item
    /// </summary>
    [JsonProperty("item", Order = 1)]
    public int Item { get; set; }

    /// <summary>
    /// Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("port", Order = 2, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Port { get; set; }

    /// <summary>
    /// Key<br/>
    /// Allways: 0
    /// </summary>
    [JsonProperty("key", Order = 3, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Key { get; set; }
    
    /// <summary>
    /// ID
    /// </summary>
    [JsonProperty("id", Order = 4, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ID { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type", Order = 5, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Type { get; set; }

    /// <summary>
    /// Flag
    /// </summary>
    [JsonProperty("flag", Order = 6, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Flag { get; set; }

    /// <summary>
    /// Value 1
    /// </summary>
    [JsonProperty("value1", Order = 7, NullValueHandling = NullValueHandling.Ignore)]
    public string Value1 { get; set; }

    /// <summary>
    /// Value 2
    /// </summary>
    [JsonProperty("value2", Order = 8, NullValueHandling = NullValueHandling.Ignore)]
    public string Value2 { get; set; }

    /// <summary>
    /// Value 3
    /// </summary>
    [JsonProperty("value3", Order = 9, NullValueHandling = NullValueHandling.Ignore)]
    public string Value3 { get; set; }

    /// <summary>
    /// Text
    /// </summary>
    [JsonProperty("text", Order = 10, NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    /// <summary>
    /// Encode
    /// </summary>
    [JsonProperty("encode", Order = 11, NullValueHandling = NullValueHandling.Ignore)]
    public string Encode { get; set; }

    /// <summary>
    /// Value<br/>
    /// Allways: "None"
    /// </summary>
    [JsonProperty("#text", Order = 12, NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; set; }
  }
}
