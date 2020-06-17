using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

using ICSP.Core.Json;

using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class State
  {
    /// <summary>
    /// 1: State Off, 2: State On
    /// </summary>
    [JsonProperty("number", Order = 1)]
    public int Number { get; set; }

    /// <summary>
    /// Draw Order (G4 Only)
    /// </summary>
    [JsonProperty("do", Order = 2, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DrawOrder DrawOrder { get; set; }

    /// <summary>
    /// Draw Order (G4 Only)
    /// </summary>
    [JsonIgnore]
    public DrawOrderItem[] DrawOrderValues
    {
      get
      {
        var lOrderValues = new[]
        {
          DrawOrderItem.Fill,
          DrawOrderItem.Bitmap,
          DrawOrderItem.Icon,
          DrawOrderItem.Text,
          DrawOrderItem.Border,
        };

        var lDrawOrderValue = DrawOrder.ToString() ?? string.Empty;

        var lEnumValue = typeof(DrawOrder).GetField(lDrawOrderValue)?.GetCustomAttribute<EnumMemberAttribute>()?.Value;

        if(!string.IsNullOrWhiteSpace(lEnumValue))
        {
          try
          {
            lOrderValues[0] = (DrawOrderItem)int.Parse(lEnumValue.Substring(0, 2));
            lOrderValues[1] = (DrawOrderItem)int.Parse(lEnumValue.Substring(2, 2));
            lOrderValues[2] = (DrawOrderItem)int.Parse(lEnumValue.Substring(4, 2));
            lOrderValues[3] = (DrawOrderItem)int.Parse(lEnumValue.Substring(6, 2));
            lOrderValues[4] = (DrawOrderItem)int.Parse(lEnumValue.Substring(8, 2));
          }
          catch { }
        }

        return lOrderValues;
      }
    }

    /// <summary>
    /// Border Style
    /// </summary>
    [JsonProperty("bs", Order = 3)]
    public string BorderStyle { get; set; }

    /// <summary>
    /// Chameleon Image
    /// </summary>
    [JsonProperty("mi", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
    public string ChameleonImage { get; set; }

    /// <summary>
    /// Border Color (Alpha not supported)
    /// </summary>
    [JsonProperty("cb", Order = 5)]
    public string BorderColor { get; set; }

    /// <summary>
    /// Fill Color
    /// </summary>
    [JsonProperty("cf", Order = 6)]
    public string FillColor { get; set; }

    /// <summary>
    /// Text Color
    /// </summary>
    [JsonProperty("ct", Order = 7)]
    public string TextColor { get; set; }

    /// <summary>
    /// Text Effect Color
    /// </summary>
    [JsonProperty("ec", Order = 8)]
    public string TextEffectColor { get; set; }

    /// <summary>
    /// Overall Opacity
    /// </summary>
    [DefaultValue(255)]
    [JsonProperty("oo", Order = 9, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public byte OverallOpacity { get; set; }

    [DefaultValue(VideoFillType.None)]
    [JsonProperty("vf", Order = 10, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public VideoFillType VideoFill { get; set; }

    /// <summary>
    /// Streaming Source
    /// </summary>
    [JsonProperty("dv", Order = 11, NullValueHandling = NullValueHandling.Ignore)]
    public string StreamingSource { get; set; }

    /// <summary>
    /// Bitmap (G4 Only)
    /// </summary>
    [JsonProperty("bm", Order = 12, NullValueHandling = NullValueHandling.Ignore)]
    public string Bitmap { get; set; }

    /// <summary>
    /// Bitmap Justification (G4 Only)
    /// </summary>
    [DefaultValue(JustificationType.CenterMiddle)]
    [JsonProperty("jb", Order = 13, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public JustificationType BitmapJustification { get; set; }

    /// <summary>
    /// Bitmap X Offset (G4 Only)
    /// </summary>
    [JsonProperty("bx", Order = 14, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int BitmapOffsetX { get; set; }

    /// <summary>
    /// Bitmap Y Offset (G4 Only)
    /// </summary>
    [JsonProperty("by", Order = 15, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int BitmapOffsetY { get; set; }

    /// <summary>
    /// Bitmaps (G5 Only)
    /// </summary>
    [JsonProperty("bitmapEntry", Order = 16, NullValueHandling = NullValueHandling.Ignore)]
    public List<BitmapEntry> Bitmaps { get; set; }

    /// <summary>
    /// Icon Slot 1-500 (G4 Only)
    /// </summary>
    [JsonProperty("ii", Order = 17, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int IconSlot { get; set; }

    /// <summary>
    /// Icon Justification (G4 Only)
    /// </summary>
    [DefaultValue(JustificationType.CenterMiddle)]
    [JsonProperty("ji", Order = 18, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public JustificationType IconJustification { get; set; }

    /// <summary>
    /// Icon X Offset (G4 Only)
    /// </summary>
    [JsonProperty("ix", Order = 19, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int IconOffsetX { get; set; }

    /// <summary>
    /// Icon Y Offset (G4 Only)
    /// </summary>
    [JsonProperty("iy", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int IconOffsetY { get; set; }
    
    /// <summary>
    /// Font Index (G4 Only, Ref -> $fnt.xml)
    /// </summary>
    [JsonProperty("fi", Order = 21, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int FontIndex { get; set; }

    /// <summary>
    /// Font (G5 Only)
    /// </summary>
    [JsonProperty("ff", Order = 22)]
    public string Font { get; set; }

    /// <summary>
    /// Font Size (G5 Only)
    /// </summary>
    [JsonProperty("fs", Order = 23)]
    public int FontSize { get; set; }

    /// <summary>
    /// Text
    /// </summary>
    [JsonProperty("te", Order = 24, NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    /// <summary>
    /// Text Justification
    /// </summary>
    [DefaultValue(JustificationType.CenterMiddle)]
    [JsonProperty("jt", Order = 25, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public JustificationType TextJustification { get; set; }

    /// <summary>
    /// Text X Offset
    /// </summary>
    [JsonProperty("tx", Order = 26, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int TextOffsetX { get; set; }

    /// <summary>
    /// Text Y Offset
    /// </summary>
    [JsonProperty("ty", Order = 27, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int TextOffsetY { get; set; }

    /// <summary>
    /// Text Effect
    /// </summary>
    [JsonProperty("et", Order = 28, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int TextEffect { get; set; }

    /// <summary>
    /// Word Wrap
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ww", Order = 29, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool WordWrap { get; set; }

    /// <summary>
    /// Sound
    /// </summary>
    [JsonProperty("sd", Order = 30, NullValueHandling = NullValueHandling.Ignore)]
    public string Sound { get; set; }
    
    /// <summary>
    /// Marquee Direction (G4 Only)
    /// </summary>
    [JsonProperty("md", Order = 31, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public MarqueeDirection MarqueeDirection { get; set; }

    /// <summary>
    /// Marquee Repeat (G4 Only)
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("mr", Order = 32, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool MarqueeRepeat { get; set; }
  }
}
