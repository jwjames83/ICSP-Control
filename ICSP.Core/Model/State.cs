using System.Collections.Generic;
using System.ComponentModel;
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
    /// Border Style
    /// </summary>
    [JsonProperty("bs", Order = 2)]
    public string BorderStyle { get; set; }

    /// <summary>
    /// Chameleon Image
    /// </summary>
    [JsonProperty("mi", Order = 3, NullValueHandling = NullValueHandling.Ignore)]
    public string ChameleonImage { get; set; }

    /// <summary>
    /// Border Color (Alpha not supported)
    /// </summary>
    [JsonProperty("cb", Order = 4)]
    public string BorderColor { get; set; }

    /// <summary>
    /// Fill Color
    /// </summary>
    [JsonProperty("cf", Order = 5)]
    public string FillColor { get; set; }

    /// <summary>
    /// Text Color
    /// </summary>
    [JsonProperty("ct", Order = 6)]
    public string TextColor { get; set; }

    /// <summary>
    /// Text Effect Color
    /// </summary>
    [JsonProperty("ec", Order = 7)]
    public string TextEffectColor { get; set; }

    /// <summary>
    /// Overall Opacity
    /// </summary>
    [DefaultValue(255)]
    [JsonProperty("oo", Order = 8, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public byte OverallOpacity { get; set; }

    [DefaultValue(VideoFillType.None)]
    [JsonProperty("vf", Order = 9, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public VideoFillType VideoFill { get; set; }

    /// <summary>
    /// Streaming Source
    /// </summary>
    [JsonProperty("dv", Order = 10, NullValueHandling = NullValueHandling.Ignore)]
    public string StreamingSource { get; set; }

    /// <summary>
    /// Bitmap (G4 Only)
    /// </summary>
    [JsonProperty("bm", Order = 11, NullValueHandling = NullValueHandling.Ignore)]
    public string Bitmap { get; set; }

    /// <summary>
    /// Bitmap Justification (G4 Only)
    /// </summary>
    [DefaultValue(JustificationType.CenterMiddle)]
    [JsonProperty("jb", Order = 12, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public JustificationType BitmapJustification { get; set; }

    /// <summary>
    /// Bitmap X Offset (G4 Only)
    /// </summary>
    [JsonProperty("bx", Order = 13, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int BitmapOffsetX { get; set; }

    /// <summary>
    /// Bitmap Y Offset (G4 Only)
    /// </summary>
    [JsonProperty("by", Order = 14, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int BitmapOffsetY { get; set; }

    /// <summary>
    /// Bitmaps (G5 Only)
    /// </summary>
    [JsonProperty("bitmapEntry", Order = 15, NullValueHandling = NullValueHandling.Ignore)]
    public List<BitmapEntry> Bitmaps { get; set; }

    /// <summary>
    /// Font Index (G4 Only, Ref -> $fnt.xml)
    /// </summary>
    [JsonProperty("fi", Order = 16, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int FontIndex { get; set; }

    /// <summary>
    /// Font (G5 Only)
    /// </summary>
    [JsonProperty("ff", Order = 17)]
    public string Font { get; set; }

    /// <summary>
    /// Font Size (G5 Only)
    /// </summary>
    [JsonProperty("fs", Order = 18)]
    public int FontSize { get; set; }

    /// <summary>
    /// Text
    /// </summary>
    [JsonProperty("te", Order = 19, NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    /// <summary>
    /// Text Justification
    /// </summary>
    [DefaultValue(JustificationType.CenterMiddle)]
    [JsonProperty("jt", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public JustificationType TextJustification { get; set; }

    /// <summary>
    /// Text X Offset
    /// </summary>
    [JsonProperty("tx", Order = 21, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int TextOffsetX { get; set; }

    /// <summary>
    /// Text Y Offset
    /// </summary>
    [JsonProperty("ty", Order = 22, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int TextOffsetY { get; set; }

    /// <summary>
    /// Text Effect
    /// </summary>
    [JsonProperty("et", Order = 23, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int TextEffect { get; set; }
    
    /// <summary>
    /// Word Wrap
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ww", Order = 24, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool WordWrap { get; set; }

    /// <summary>
    /// Sound
    /// </summary>
    [JsonProperty("sd", Order = 25, NullValueHandling = NullValueHandling.Ignore)]
    public string Sound { get; set; }
  }
}
