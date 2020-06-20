using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.Core.Model
{
  public class BitmapEntry
  {
    [JsonExtensionData]
    private IDictionary<string, JToken> mAdditionalData;

    public BitmapEntry()
    {
      mAdditionalData = new Dictionary<string, JToken>();
    }

    // This field is not serialized. The OnDeserializedAttribute is used to set the member value after serialization.
    /// <summary>
    /// FileName
    /// </summary>
    [JsonIgnore]
    public string FileName { get; set; }

    // This field is not serialized. The OnDeserializedAttribute is used to set the member value after serialization.
    /// <summary>
    /// Property FileName refers a dynamic resource
    /// </summary>
    [JsonIgnore]
    public bool Dynamic { get; set; }

    [DefaultValue(JustificationType.CenterMiddle)]
    [JsonProperty("justification", Order = 2, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public JustificationType Justification { get; set; }

    /// <summary>
    /// Offset X
    /// </summary>
    [JsonProperty("offsetX", Order = 3, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int OffsetX { get; set; }

    /// <summary>
    /// Offset Y
    /// </summary>
    [JsonProperty("offsetY", Order = 4, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int OffsetY { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      if(mAdditionalData.TryGetValue("fileName", out var fileNameToken))
      {
        /*
        XML:
        <bitmapEntry><fileName>CalculatorThumb.png</fileName></bitmapEntry>
        <bitmapEntry><fileName dynamic="1">Test 2</fileName></bitmapEntry>

        JSON:
        "bitmapEntry": [
          { "fileName": "realvista_webdesign_3d_design_16.png", },
          { "fileName": { "dynamic": "1", "#text": "Test" } }
        ],
        */

        switch(fileNameToken)
        {
          case JValue value:
          {
            FileName = (string)value;
            break;
          }
          case JObject obj:
          {
            FileName = (string)obj["#text"];
            Dynamic = (int)(obj["dynamic"] ?? 0) == 1;
            break;
          }
        }
      }
    }
  }
}
