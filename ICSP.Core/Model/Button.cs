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
    /// Touch Style
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
    [DefaultValue(2)]
    [JsonProperty("stateCount", Order = 16, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int StateCount { get; set; }

    /// <summary>
    /// MultiGeneral: Range Count
    /// </summary>
    [JsonProperty("rm", Order = 17, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(2)]
    public int RangeCount { get; set; }

    /// <summary>
    /// MultiGeneral: Animate Time Up
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("nu", Order = 18, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AnimateTimeUp { get; set; }

    /// <summary>
    /// MultiGeneral: Animate Time Down
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("nd", Order = 19, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AnimateTimeDown { get; set; }
    
    /// <summary>
    /// MultiGeneral: Auto-Repeat
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ar", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool AutoRepeat { get; set; }

    /// <summary>
    /// MultiGeneral: Animate Range Up
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("ru", Order = 21, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AnimateRangeUp { get; set; }

    /// <summary>
    /// MultiGeneral: Animate Range Down
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("rd", Order = 22, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AnimateRangeDown { get; set; }
    
    #endregion

    /// <summary>
    /// Disabled
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("da", Order = 23, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Hidden
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("hd", Order = 24, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Hidden { get; set; }

    #region Bargraph

    /// <summary>
    /// Bargraph: Value Direction
    /// </summary>
    [DefaultValue(ValueDirection.Vertical)]
    [JsonProperty("dr", Order = 25, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public ValueDirection ValueDirection { get; set; }
    
    /// <summary>
    /// Multi-State-Bargraph: Touch Map
    /// </summary>
    [JsonProperty("tm", Order = 26, NullValueHandling = NullValueHandling.Ignore)]
    public string TouchMap { get; set; }

    /// <summary>
    /// Bargraph: Slider Name
    /// </summary>
    [JsonProperty("sd", Order = 27, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public SliderType SliderName { get; set; }

    /// <summary>
    /// Bargraph: Slider Color
    /// </summary>
    [JsonProperty("sc", Order = 28, NullValueHandling = NullValueHandling.Ignore)]
    public string SliderColor { get; set; }

    #endregion

    #region Joystick

    /// <summary>
    /// Joystick: Cursor Name
    /// </summary>
    [JsonProperty("cd", Order = 29, NullValueHandling = NullValueHandling.Ignore)]
    public string CursorName { get; set; }

    /// <summary>
    /// Joystick: Cursor Color
    /// </summary>
    [JsonProperty("cc", Order = 30, NullValueHandling = NullValueHandling.Ignore)]
    public string CursorColor { get; set; }

    /// <summary>
    /// Joystick: Range Aux Inverted
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ji", Order = 31, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool RangeAuxInverted { get; set; }

    #endregion

    #region TextInput

    /// <summary>
    /// TextInput: Password Character
    /// </summary>
    [JsonProperty("pc", Order = 32, NullValueHandling = NullValueHandling.Ignore)]
    public string PasswordCharacter { get; set; }

    /// <summary>
    /// TextInput: DisplayType
    /// </summary>
    [JsonProperty("dt", Order = 33, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DisplayType DisplayType { get; set; }

    /// <summary>
    /// TextInput: Max Text Length
    /// </summary>
    [JsonProperty("mt", Order = 34, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int MaxTextLength { get; set; }

    /// <summary>
    /// TextInput: Input Mask
    /// </summary>
    [JsonProperty("im", Order = 35, NullValueHandling = NullValueHandling.Ignore)]
    public string InputMask { get; set; }

    #endregion

    #region Computer Control

    /// <summary>
    /// ComputerControl: Remote Host
    /// </summary>
    [JsonProperty("ho", Order = 36, NullValueHandling = NullValueHandling.Ignore)]
    public string RemoteHost { get; set; }

    /// <summary>
    /// ComputerControl: Remote Port
    /// </summary>
    [DefaultValue(5900)]
    [JsonProperty("rp", Order = 37, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RemotePort { get; set; }

    /// <summary>
    /// ComputerControl: Password
    /// </summary>
    [JsonProperty("pw", Order = 38, NullValueHandling = NullValueHandling.Ignore)]
    public object Password { get; set; }

    /// <summary>
    /// ComputerControl: Color Depth
    /// </summary>
    [JsonProperty("cl", Order = 39, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ColorDepth { get; set; }

    /// <summary>
    /// ComputerControl: Compression
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("cr", Order = 40, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Compression { get; set; }

    /// <summary>
    /// ComputerControl: Scale To Fit
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("sf", Order = 41, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool ScaleToFit { get; set; }

    /// <summary>
    /// ComputerControl: TakeNote Enabled
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ea", Order = 42, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool TakeNoteEnabled { get; set; }

    /// <summary>
    /// ComputerControl: TakeNote Host
    /// </summary>
    [JsonProperty("ha", Order = 43, NullValueHandling = NullValueHandling.Ignore)]
    public string TakeNoteHost { get; set; }

    /// <summary>
    /// ComputerControl: TakeNote Port
    /// </summary>
    [DefaultValue(1541)]
    [JsonProperty("pa", Order = 44, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int TakeNotePort { get; set; }

    #endregion

    #region Sub-Page View

    /// <summary>
    /// Sub-Page View: Sub Page Set (Index)
    /// </summary>
    [JsonProperty("st", Order = 45, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int SubPageSet { get; set; }

    /// <summary>
    /// Sub-Page View: Orientation
    /// </summary>
    [DefaultValue(Orientation.Horizontal)]
    [JsonProperty("on", Order = 46, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public Orientation Orientation { get; set; }

    /// <summary>
    /// Sub-Page View: Spacing (%)
    /// </summary>
    [JsonProperty("sa", Order = 47, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Spacing { get; set; }

    /// <summary>
    /// Sub-Page View: Anchor Position
    /// </summary>
    [DefaultValue(AnchorPosition.Center)]
    [JsonProperty("we", Order = 48, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public AnchorPosition AnchorPosition { get; set; }

    /// <summary>
    /// Sub-Page View: Wrap Sub-Pages
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ws", Order = 49, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool WrapSubPages { get; set; }

    /// <summary>
    /// Sub-Page View: Show Sub-Pages
    /// </summary>
    [DefaultValue(true)]
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("sw", Order = 50, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool ShowSubPages { get; set; }
   
    /// <summary>
    /// Sub-Page View: Allow Dynamic Reordering
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("dy", Order = 51, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool AllowDynamicReordering { get; set; }

    /// <summary>
    /// Sub-Page View: Reset View on Show
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("rs", Order = 52, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool ResetViewOnShow { get; set; }

    /// <summary>
    /// Sub-Page View: Scrollbar
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ba", Order = 53, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Scrollbar { get; set; }

    /// <summary>
    /// Sub-Page View: Scrollbar Offset
    /// </summary>
    [JsonProperty("bo", Order = 54, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ScrollbarOffset { get; set; }
    
    #endregion

    /// <summary>
    /// Password Protection
    /// </summary>
    [DefaultValue(PasswordProtection.None)]
    [JsonProperty("pp", Order = 55, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public PasswordProtection PasswordProtection { get; set; }

    // ============================================================================================
    // TAB: Programming
    // ============================================================================================

    /// <summary>
    /// Feedback
    /// </summary>
    [DefaultValue(FeedbackType.Channel)]
    [JsonProperty("fb", Order = 56, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public FeedbackType Feedback { get; set; }

    /// <summary>
    /// Address Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("ap", Order = 57, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AddressPort { get; set; }

    /// <summary>
    /// Address Code
    /// </summary>
    [JsonProperty("ad", Order = 58, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AddressCode { get; set; }

    /// <summary>
    /// Channel Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("cp", Order = 59, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ChannelPort { get; set; }

    /// <summary>
    /// Channel Code
    /// </summary>
    [JsonProperty("ch", Order = 60, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ChannelCode { get; set; }

    /// <summary>
    /// Range Type<br/>
    /// (sByte, sint, "slong"long, int)
    /// </summary>
    [DefaultValue(RangeType.Byte)]
    [JsonProperty("rt", Order = 61, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public RangeType RangeType { get; set; }

    /// <summary>
    /// Level Control Type
    /// </summary>
    [DefaultValue(LevelControlType.None)]
    [JsonProperty("vt", Order = 62, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public LevelControlType LevelControlType { get; set; }

    /// <summary>
    /// Level Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("lp", Order = 63, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelPort { get; set; }

    /// <summary>
    /// Level Code
    /// </summary>
    [JsonProperty("lv", Order = 64, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelCode { get; set; }

    /// <summary>
    /// Bargraph: Level Function
    /// </summary>
    [DefaultValue(LevelFunction.DisplayOnly)]
    [JsonProperty("lf", Order = 65, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public LevelFunction LevelFunction { get; set; }

    /// <summary>
    /// Level Control Value
    /// </summary>
    [JsonProperty("va", Order = 66, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int LevelControlValue { get; set; }

    /// <summary>
    /// Range Low
    /// </summary>
    [JsonProperty("rl", Order = 67)]
    public int RangeLow { get; set; }

    /// <summary>
    /// Range High
    /// </summary>
    [JsonProperty("rh", Order = 68)]
    public int RangeHigh { get; set; }

    /// <summary>
    /// Bargraph: Range Drag Increment
    /// </summary>
    [JsonProperty("rn", Order = 69, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RangeDragIncrement { get; set; }

    /// <summary>
    /// Bargraph: Range Inverted
    /// </summary>
    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("ri", Order = 70, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool RangeInverted { get; set; }

    /// <summary>
    /// Range Time Up
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("lu", Order = 71, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RangeTimeUp { get; set; }

    /// <summary>
    /// Range Time Down
    /// </summary>
    [DefaultValue(2)]
    [JsonProperty("ld", Order = 72, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RangeTimeDown { get; set; }

    /// <summary>
    /// String Output Port (G4 Only)
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("so", Order = 73, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int StringOutputPort { get; set; }

    /// <summary>
    /// String Output (G4 Only)
    /// </summary>
    [JsonProperty("op", Order = 74, NullValueHandling = NullValueHandling.Ignore)]
    public string StringOutput { get; set; }

    /// <summary>
    /// Command Output Port (G4 Only)
    /// </summary>
    [DefaultValue(21)]
    [JsonProperty("co", Order = 75, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int CommandOutputPort { get; set; }

    /// <summary>
    /// Command Output (G4 Only)
    /// </summary>
    [JsonProperty("cm", Order = 76, NullValueHandling = NullValueHandling.Ignore)]
    public string CommandOutput { get; set; }

    /// <summary>
    /// Page Flip Animation (G4 Only)
    /// </summary>
    [JsonProperty("ac", Order = 77, NullValueHandling = NullValueHandling.Ignore)]
    public PageFlipAnimation PageFlipAnimation { get; set; }
    
    /// <summary>
    /// Page Flip: Animation Time (tenths/sec) (G4 Only)
    /// </summary>
    [JsonProperty("at", Order = 78, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int PageFlipAnimationTime { get; set; }

    /// <summary>
    /// Page Flips (G4 Only)
    /// </summary>
    [JsonProperty("pf", Order = 79, NullValueHandling = NullValueHandling.Ignore)]
    public List<PageFlip> PageFlips { get; set; }
    
    // ============================================================================================
    // TAB: States
    // ============================================================================================

    /// <summary>
    /// States
    /// </summary>
    [JsonProperty("sr", Order = 80)]
    public List<State> States { get; set; }

    // ============================================================================================
    // TAB: Events (G5 Only)
    // ============================================================================================

    /// <summary>
    /// G5 Events: Button Press
    /// </summary>
    [JsonProperty("ep", Order = 81)]
    public ActionEvents EventsButtonPress { get; set; }

    /// <summary>
    /// G5 Events: Button Release
    /// </summary>
    [JsonProperty("er", Order = 82)]
    public ActionEvents EventsButtonRelease { get; set; }

    /// <summary>
    /// G5 Events: Gesture Any
    /// </summary>
    [JsonProperty("ga", Order = 83)]
    public ActionEvents EventsGestureAny { get; set; }

    /// <summary>
    /// G5 Events: Gesture Up
    /// </summary>
    [JsonProperty("gu", Order = 84)]
    public ActionEvents EventsGestureUp { get; set; }

    /// <summary>
    /// G5 Events: Gesture Down
    /// </summary>
    [JsonProperty("gd", Order = 85)]
    public ActionEvents EventsGestureDown { get; set; }

    /// <summary>
    /// G5 Events: Gesture Right
    /// </summary>
    [JsonProperty("gr", Order = 86)]
    public ActionEvents EventsGestureRight { get; set; }

    /// <summary>
    /// G5 Events: Gesture Left
    /// </summary>
    [JsonProperty("gl", Order = 87)]
    public ActionEvents EventsGestureLeft { get; set; }

    /// <summary>
    /// G5 Events: Gesture Double-Tap
    /// </summary>
    [JsonProperty("gt", Order = 88)]
    public ActionEvents EventsGestureDoubleTap { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Up
    /// </summary>
    [JsonProperty("tu", Order = 89)]
    public ActionEvents EventsGesture2FingerUp { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Down
    /// </summary>
    [JsonProperty("td", Order = 90)]
    public ActionEvents EventsGesture2FingerDown { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Right
    /// </summary>
    [JsonProperty("tr", Order = 91)]
    public ActionEvents EventsGesture2FingerRight { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Left
    /// </summary>
    [JsonProperty("tl", Order = 92)]
    public ActionEvents EventsGesture2FingerLeft { get; set; }

    /*
    <dst / ==> Events: 
    <dca / ==> Events: 
    <den / ==> Events: 
    <dex / ==> Events: 
    <ddr / ==> Events: 
    */
  }
}
