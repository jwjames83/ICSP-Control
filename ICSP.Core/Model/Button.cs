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
    /// Above Popups (G4 Only)
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("vp", Order = 10, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool AbovePopups { get; set; }

    /// <summary>
    /// Z-Order ([<]Back, [>]:Top)
    /// </summary>
    [JsonProperty("zo", Order = 11)]
    public int ZOrder { get; set; }

    /// <summary>
    /// Drag/Drop Type (G5 Only)<br/>
    /// (none, draggable, drop target)
    /// </summary>
    [DefaultValue(DragDropType.None)]
    [JsonProperty("ddt", Order = 12, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DragDropType DragDropType { get; set; }

    /// <summary>
    /// Drop Group (G5 Only)
    /// </summary>
    [JsonProperty("ddg", Order = 13, NullValueHandling = NullValueHandling.Ignore)]
    public string DropGroup { get; set; }

    /// <summary>
    /// Touch Style  (bounding, passThru)
    /// </summary>
    [DefaultValue(TouchStyle.ActiveTouch)]
    [JsonProperty("hs", Order = 14, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public TouchStyle TouchStyle { get; set; }

    /// <summary>
    /// Border Style (-> TAB General)
    /// </summary>
    [JsonProperty("bs", Order = 15)]
    public string BorderStyle { get; set; }

    #region Multi State General

    /// <summary>
    /// MultiGeneral: State Count
    /// </summary>
    [JsonProperty("stateCount", Order = 15)]
    public int StateCount { get; set; }

    /// <summary>
    /// MultiGeneral: State Count
    /// </summary>
    [JsonProperty("rm", Order = 15)]
    public int rm { get; set; }

    /// <summary>
    /// MultiGeneral: Animate Time Up
    /// </summary>
    [JsonProperty("nu", Order = 15)]
    public int AnimateTimeUp { get; set; }

    /// <summary>
    /// MultiGeneral: Animate Time Down
    /// </summary>
    [JsonProperty("nd", Order = 15)]
    public int AnimateTimeDown { get; set; }
    
    /// <summary>
    /// MultiGeneral: Auto-Repeat
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ar", Order = 15, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool AutoRepeat { get; set; }

    /// <summary>
    /// MultiGeneral: ru
    /// </summary>
    [JsonProperty("ru", Order = 15)]
    public int ru { get; set; }

    /// <summary>
    /// MultiGeneral: rd
    /// </summary>
    [JsonProperty("rd", Order = 15)]
    public int rd { get; set; }
    
    #endregion

    /// <summary>
    /// Disabled
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("da", Order = 16, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Hidden
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("hd", Order = 17, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Hidden { get; set; }

    #region Bargraph

    /// <summary>
    /// Bargraph: Value Direction
    /// </summary>
    [DefaultValue(ValueDirection.Vertical)]
    [JsonProperty("dr", Order = 17, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public ValueDirection ValueDirection { get; set; }
    
    /// <summary>
    /// Multi-State-Bargraph: Touch Map
    /// </summary>
    [JsonProperty("tm", Order = 17, NullValueHandling = NullValueHandling.Ignore)]
    public string TouchMap { get; set; }

    /// <summary>
    /// Bargraph: Slider Name
    /// </summary>
    [JsonProperty("sd", Order = 17, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public SliderType SliderName { get; set; }

    /// <summary>
    /// Bargraph: Slider Color
    /// </summary>
    [JsonProperty("sc", Order = 17, NullValueHandling = NullValueHandling.Ignore)]
    public string SliderColor { get; set; }

    #endregion

    #region Joystick

    /// <summary>
    /// Joystick: Cursor Name
    /// </summary>
    [JsonProperty("cd", Order = 17, NullValueHandling = NullValueHandling.Ignore)]
    public string CursorName { get; set; }

    /// <summary>
    /// Joystick: Cursor Color
    /// </summary>
    [JsonProperty("cc", Order = 17, NullValueHandling = NullValueHandling.Ignore)]
    public string CursorColor { get; set; }

    #endregion

    #region TextInput

    /// <summary>
    /// TextInput: Password Character
    /// </summary>
    [JsonProperty("pc", Order = 17, NullValueHandling = NullValueHandling.Ignore)]
    public string PasswordCharacter { get; set; }

    /// <summary>
    /// TextInput: DisplayType
    /// </summary>
    [JsonProperty("dt", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DisplayType DisplayType { get; set; }

    /// <summary>
    /// TextInput: Max Text Length
    /// </summary>
    [JsonProperty("mt", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int MaxTextLength { get; set; }

    /// <summary>
    /// TextInput: Input Mask
    /// </summary>
    [JsonProperty("im", Order = 17, NullValueHandling = NullValueHandling.Ignore)]
    public string InputMask { get; set; }

    #endregion

    /// <summary>
    /// Touch Style  (bounding, passThru)
    /// </summary>
    [DefaultValue(PasswordProtection.None)]
    [JsonProperty("pp", Order = 18, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public PasswordProtection PasswordProtection { get; set; }

    // ============================================================================================
    // TAB: Programming
    // ============================================================================================

    /// <summary>
    /// Feedback
    /// </summary>
    [DefaultValue(FeedbackType.Channel)]
    [JsonProperty("fb", Order = 19, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public FeedbackType Feedback { get; set; }

    /// <summary>
    /// Address Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("ap", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AddressPort { get; set; }

    /// <summary>
    /// Address Code
    /// </summary>
    [JsonProperty("ad", Order = 21, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AddressCode { get; set; }

    /// <summary>
    /// Channel Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("cp", Order = 22, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ChannelPort { get; set; }

    /// <summary>
    /// Channel Code
    /// </summary>
    [JsonProperty("ch", Order = 23, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ChannelCode { get; set; }

    /// <summary>
    /// Range Type<br/>
    /// (sByte, sint, "slong"long, int)
    /// </summary>
    [DefaultValue(RangeType.Byte)]
    [JsonProperty("rt", Order = 24, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public RangeType RangeType { get; set; }

    /// <summary>
    /// Level Control Type
    /// </summary>
    [DefaultValue(LevelControlType.None)]
    [JsonProperty("vt", Order = 25, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public LevelControlType LevelControlType { get; set; }

    /// <summary>
    /// Level Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("lp", Order = 26, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelPort { get; set; }

    /// <summary>
    /// Level Code
    /// </summary>
    [JsonProperty("lv", Order = 27, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelCode { get; set; }

    /// <summary>
    /// Bargraph: Level Function
    /// </summary>
    [DefaultValue(LevelFunction.DisplayOnly)]
    [JsonProperty("lf", Order = 27, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public LevelFunction LevelFunction { get; set; }

    /// <summary>
    /// Level Control Value
    /// </summary>
    [JsonProperty("va", Order = 28, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelControlValue { get; set; }

    /// <summary>
    /// Range Low
    /// </summary>
    [JsonProperty("rl", Order = 29)]
    public int RangeLow { get; set; }

    /// <summary>
    /// Range High
    /// </summary>
    [JsonProperty("rh", Order = 30)]
    public int RangeHigh { get; set; }

    /// <summary>
    /// Bargraph: Range Drag Increment
    /// </summary>
    [JsonProperty("rn", Order = 30)]
    public int RangeDragIncrement { get; set; }

    /// <summary>
    /// Bargraph: Range Inverted
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ri", Order = 30, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool RangeInverted { get; set; }

    /// <summary>
    /// Range Time Up
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("lu", Order = 31, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RangeTimeUp { get; set; }

    /// <summary>
    /// Range Time Down
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("ld", Order = 32, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RangeTimeDown { get; set; }

    /// <summary>
    /// String Output Port (G4 Only)
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("so", Order = 33, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int StringOutputPort { get; set; }

    /// <summary>
    /// String Output (G4 Only)
    /// </summary>
    [JsonProperty("op", Order = 34, NullValueHandling = NullValueHandling.Ignore)]
    public string StringOutput { get; set; }

    /// <summary>
    /// Command Output Port (G4 Only)
    /// </summary>
    [DefaultValue(21)]
    [JsonProperty("co", Order = 35, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int CommandOutputPort { get; set; }

    /// <summary>
    /// Command Output (G4 Only)
    /// </summary>
    [JsonProperty("cm", Order = 36, NullValueHandling = NullValueHandling.Ignore)]
    public string CommandOutput { get; set; }

    /// <summary>
    /// Unknown (G4)
    /// </summary>
    [JsonProperty("ac", Order = 37)]
    public object ac { get; set; } // { "di": "0" },

    // ============================================================================================
    // TAB: States
    // ============================================================================================

    /// <summary>
    /// States
    /// </summary>
    [JsonProperty("sr", Order = 38)]
    public IList<State> States { get; set; }

    // ============================================================================================
    // TAB: Events (G5 Only)
    // ============================================================================================

    /// <summary>
    /// Events: Button Press
    /// </summary>
    [JsonProperty("ep", Order = 39)]
    public Events EventsButtonPress { get; set; }

    /// <summary>
    /// Events: Button Release
    /// </summary>
    [JsonProperty("er", Order = 40)]
    public Events EventsButtonRelease { get; set; }

    /// <summary>
    /// Events: Gesture Any
    /// </summary>
    [JsonProperty("ga", Order = 41)]
    public Events EventsGestureAny { get; set; }

    /// <summary>
    /// Events: Gesture Up
    /// </summary>
    [JsonProperty("gu", Order = 42)]
    public Events EventsGestureUp { get; set; }

    /// <summary>
    /// Events: Gesture Down
    /// </summary>
    [JsonProperty("gd", Order = 43)]
    public Events EventsGestureDown { get; set; }

    /// <summary>
    /// Events: Gesture Right
    /// </summary>
    [JsonProperty("gr", Order = 44)]
    public Events EventsGestureRight { get; set; }

    /// <summary>
    /// Events: Gesture Left
    /// </summary>
    [JsonProperty("gl", Order = 45)]
    public Events EventsGestureLeft { get; set; }

    /// <summary>
    /// Events: Gesture Double-Tap
    /// </summary>
    [JsonProperty("gt", Order = 46)]
    public Events EventsGestureDoubleTap { get; set; }

    /// <summary>
    /// Events: Gesture 2-Finger Up
    /// </summary>
    [JsonProperty("tu", Order = 47)]
    public Events EventsGesture2FingerUp { get; set; }

    /// <summary>
    /// Events: Gesture 2-Finger Down
    /// </summary>
    [JsonProperty("td", Order = 48)]
    public Events EventsGesture2FingerDown { get; set; }

    /// <summary>
    /// Events: Gesture 2-Finger Right
    /// </summary>
    [JsonProperty("tr", Order = 49)]
    public Events EventsGesture2FingerRight { get; set; }

    /// <summary>
    /// Events: Gesture 2-Finger Left
    /// </summary>
    [JsonProperty("tl", Order = 50)]
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
