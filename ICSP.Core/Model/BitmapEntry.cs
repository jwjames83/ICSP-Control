using System.ComponentModel;
using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class BitmapEntry
  {
    /*
    "bitmapEntry": {
      "fileName": "realvista_webdesign_3d_design_16.png"
    },
    
    "bitmapEntry": [
      {
        "fileName": "realvista_webdesign_3d_design_16.png",
        "justification": "0",
        "offsetX": "11",
        "offsetY": "22"
      },
      {
        "fileName": {
          "dynamic": "1",
          "#text": "Test"
        }
      }
    ],
    */

    /// <summary>
    /// FileName or object of Type dynamic
    /// </summary>
    [JsonProperty("fileName", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
    public object FileName { get; set; }

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
  }

  public class BitmapEntryDynamic
  {
    /// <summary>
    /// Index of dynamic Images
    /// </summary>
    [JsonProperty("dynamic", Order = 1)]
    public int Dynamic { get; set; }

    /// <summary>
    /// Text
    /// </summary>
    [JsonProperty("#text", Order = 2)]
    public string Text { get; set; }
  }
}
