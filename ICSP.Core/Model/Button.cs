using System.Collections.Generic;
using System.ComponentModel;

using ICSP.Core.Json;

using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class Button
  {
    /// <summary>
    /// Button Type<br/>
    /// (general, multiGeneral, bargraph, multiBargraph, textArea, subPageView, listView)
    /// </summary>
    [JsonProperty("type", Order = 1)]
    public ButtonType Type { get; set; }

    // ============================================================================================
    // TAB: General
    // ============================================================================================
    
      /// <summary>
    /// Index
    /// </summary>
    [JsonProperty("bi", Order = 2)]
    public int Index { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("na", Order = 3)]
    public string Name { get; set; }

    /// <summary>
    /// Lock Button Name
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("li", Order = 4, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool LockButtonName { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("bd", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
    public string Description { get; set; }

    /// <summary>
    /// Position Left
    /// </summary>
    [JsonProperty("lt", Order = 6)]
    public int Left { get; set; }

    /// <summary>
    /// Position Top
    /// </summary>
    [JsonProperty("tp", Order = 7)]
    public int Top { get; set; }

    /// <summary>
    /// Width
    /// </summary>
    [JsonProperty("wt", Order = 8)]
    public int Width { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    [JsonProperty("ht", Order = 9)]
    public int Height { get; set; }

    /// <summary>
    /// Z-Order ([<]Back, [>]:Top)
    /// </summary>
    [JsonProperty("zo", Order = 10)]
    public int ZOrder { get; set; }

    /// <summary>
    /// Drag/Drop Type<br/>
    /// (none, draggable, drop target)
    /// </summary>
    [DefaultValue(DragDropType.None)]
    [JsonProperty("ddt", Order = 11, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DragDropType DragDropType { get; set; }

    /// <summary>
    /// Drop Group
    /// </summary>
    [JsonProperty("ddg", Order = 12, NullValueHandling = NullValueHandling.Ignore)]
    public string DropGroup { get; set; }

    /// <summary>
    /// Touch Style  (bounding, passThru)
    /// </summary>
    [DefaultValue(TouchStyle.ActiveTouch)]
    [JsonProperty("hs", Order = 13, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public TouchStyle TouchStyle { get; set; }

    /// <summary>
    /// Border Style (-> TAB General)
    /// </summary>
    [JsonProperty("bs", Order = 14)]
    public string BorderStyle { get; set; }

    /// <summary>
    /// Disabled
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("da", Order = 15, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Hidden
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("hd", Order = 16, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Hidden { get; set; }

    /// <summary>
    /// Touch Style  (bounding, passThru)
    /// </summary>
    [DefaultValue(PasswordProtection.None)]
    [JsonProperty("pp", Order = 17, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public PasswordProtection PasswordProtection { get; set; }

    // ============================================================================================
    // TAB: Programming
    // ============================================================================================

    /// <summary>
    /// Feedback
    /// </summary>
    [DefaultValue(FeedbackType.Channel)]
    [JsonProperty("fb", Order = 18, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public FeedbackType Feedback { get; set; }

    /// <summary>
    /// Address Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("ap", Order = 19, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AddressPort { get; set; }

    /// <summary>
    /// Address Code
    /// </summary>
    [JsonProperty("ad", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AddressCode { get; set; }

    /// <summary>
    /// Channel Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("cp", Order = 21, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ChannelPort { get; set; }

    /// <summary>
    /// Channel Code
    /// </summary>
    [JsonProperty("ch", Order = 22, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ChannelCode { get; set; }

    /// <summary>
    /// Range Type<br/>
    /// (sByte, sint, "slong"long, int)
    /// </summary>
    [DefaultValue(RangeType.Byte)]
    [JsonProperty("rt", Order = 23, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public RangeType RangeType { get; set; }

    /// <summary>
    /// Level Control Type
    /// </summary>
    [DefaultValue(LevelControlType.None)]
    [JsonProperty("vt", Order = 24, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public LevelControlType LevelControlType { get; set; }

    /// <summary>
    /// Level Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("lp", Order = 25, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelPort { get; set; }

    /// <summary>
    /// Level Code
    /// </summary>
    [JsonProperty("lv", Order = 26, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelCode { get; set; }

    /// <summary>
    /// Level Control Value
    /// </summary>
    [JsonProperty("va", Order = 27, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelControlValue { get; set; }

    /// <summary>
    /// Range Low
    /// </summary>
    [JsonProperty("rl", Order = 28)]
    public int RangeLow { get; set; }

    /// <summary>
    /// Range High
    /// </summary>
    [JsonProperty("rh", Order = 29)]
    public int RangeHigh { get; set; }

    /// <summary>
    /// Range Time Up
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("lu", Order = 30, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RangeTimeUp { get; set; }

    /// <summary>
    /// Range Time Down
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("ld", Order = 31, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RangeTimeDown { get; set; }
    
    /// <summary>
    /// Unknown (G4)
    /// </summary>
    [JsonProperty("ac", Order = 32)]
    public int ac { get; set; }

    // ============================================================================================
    // TAB: States
    // ============================================================================================

    /// <summary>
    /// States
    /// </summary>
    [JsonProperty("sr", Order = 33)]
    public IList<State> States { get; set; }

    // ============================================================================================
    // TAB: Events (G5 Only)
    // ============================================================================================

    /// <summary>
    /// Events: Button Press
    /// </summary>
    [JsonProperty("ep", Order = 34)]
    public Events EventsButtonPress { get; set; }

    /// <summary>
    /// Events: Button Release
    /// </summary>
    [JsonProperty("er", Order = 35)]
    public Events EventsButtonRelease { get; set; }

    /// <summary>
    /// Events: Gesture Any
    /// </summary>
    [JsonProperty("ga", Order = 36)]
    public Events EventsGestureAny { get; set; }

    /// <summary>
    /// Events: Gesture Up
    /// </summary>
    [JsonProperty("gu", Order = 37)]
    public Events EventsGestureUp { get; set; }

    /// <summary>
    /// Events: Gesture Down
    /// </summary>
    [JsonProperty("gd", Order = 38)]
    public Events EventsGestureDown { get; set; }

    /// <summary>
    /// Events: Gesture Right
    /// </summary>
    [JsonProperty("gr", Order = 39)]
    public Events EventsGestureRight { get; set; }

    /// <summary>
    /// Events: Gesture Left
    /// </summary>
    [JsonProperty("gl", Order = 40)]
    public Events EventsGestureLeft { get; set; }

    /// <summary>
    /// Events: Gesture Double-Tap
    /// </summary>
    [JsonProperty("gt", Order = 41)]
    public Events EventsGestureDoubleTap { get; set; }

    /// <summary>
    /// Events: Gesture 2-Finger Up
    /// </summary>
    [JsonProperty("tu", Order = 42)]
    public Events EventsGesture2FingerUp { get; set; }

    /// <summary>
    /// Events: Gesture 2-Finger Down
    /// </summary>
    [JsonProperty("td", Order = 43)]
    public Events EventsGesture2FingerDown { get; set; }

    /// <summary>
    /// Events: Gesture 2-Finger Right
    /// </summary>
    [JsonProperty("tr", Order = 44)]
    public Events EventsGesture2FingerRight { get; set; }

    /// <summary>
    /// Events: Gesture 2-Finger Left
    /// </summary>
    [JsonProperty("tl", Order = 45)]
    public Events EventsGesture2FingerLeft { get; set; }

    /*
    <dst / ==> Events: 
    <dca / ==> Events: 
    <den / ==> Events: 
    <dex / ==> Events: 
    <ddr / ==> Events: 
    */
  }
}
