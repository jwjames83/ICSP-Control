using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Newtonsoft.Json;

using TP.Design.Model;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlPage
  {
    /// <summary>
    /// Page type ("page", "subpage")
    /// </summary>
    [JsonIgnore]
    public PageType Type { get; set; }

    // SubPage:
    // [JsonProperty("popupType", Order = 2)]

    /// <summary>
    /// PageID
    /// </summary>
    [JsonProperty("pageID", Order = 3)]
    public int PageID { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonIgnore]
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
    public string Description { get; set; }

    // SubPage:
    // [JsonProperty("left", Order = 6)]
    // [JsonProperty("top", Order = 7)]

    /// <summary>
    /// Width
    /// </summary>
    [JsonProperty("width", Order = 8)]
    public int Width { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    [JsonProperty("height", Order = 9)]
    public int Height { get; set; }

    // SubPage:
    // [JsonProperty("resetPos", Order = 10, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("group", Order = 11, NullValueHandling = NullValueHandling.Ignore)]
    // [JsonProperty("timeout", Order = 12, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("modal", Order = 13, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("showEffect", Order = 14, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("showLocX", Order = 15, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("showLocY", Order = 16, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("showTime", Order = 17, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("hideEffect", Order = 18, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("hideLocX", Order = 19, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("hideLocY", Order = 20, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("hideTime", Order = 2§, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("collapseDirection", Order = 21, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("collapseOffset", Order = 22, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    // [JsonProperty("collapseShowOpen", Order = 24, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

    /// <summary>
    /// Address Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("addressPort", Order = 25, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AddressPort { get; set; }

    /// <summary>
    /// Address Code
    /// </summary>
    [JsonProperty("address", Order = 26, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int AddressCode { get; set; }
    /// <summary>
    /// Channel Port
    /// </summary>
    [DefaultValue(1)]
    [JsonProperty("channelPort", Order = 27, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ChannelPort { get; set; }

    /// <summary>
    /// Channel Code
    /// </summary>
    [JsonProperty("channel", Order = 28, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int ChannelCode { get; set; }

    // ============================================================================================
    // TAB: Events (G5 Only)
    // ============================================================================================

    /// <summary>
    /// G5 Events: Show Page
    /// </summary>
    [JsonProperty("eventShow", Order = 29, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsShowPage { get; set; }

    /// <summary>
    /// G5 Events: Hide Page
    /// </summary>
    [JsonProperty("eventHide", Order = 30, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsHidePage { get; set; }

    /// <summary>
    /// G5 Events: Gesture Any
    /// </summary>
    [JsonProperty("gestureAny", Order = 31, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGestureAny { get; set; }

    /// <summary>
    /// G5 Events: Gesture Up
    /// </summary>
    [JsonProperty("gestureUp", Order = 32, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGestureUp { get; set; }

    /// <summary>
    /// G5 Events: Gesture Down
    /// </summary>
    [JsonProperty("gestureDown", Order = 33, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGestureDown { get; set; }

    /// <summary>
    /// G5 Events: Gesture Right
    /// </summary>
    [JsonProperty("gestureRight", Order = 34, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGestureRight { get; set; }

    /// <summary>
    /// G5 Events: Gesture Left
    /// </summary>
    [JsonProperty("gestureLeft", Order = 35, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGestureLeft { get; set; }

    /// <summary>
    /// G5 Events: Gesture Double-Tap
    /// </summary>
    [JsonProperty("gestureDblTap", Order = 36, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGestureDoubleTap { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Up
    /// </summary>
    [JsonProperty("gesture2FUp", Order = 37, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGesture2FingerUp { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Down
    /// </summary>
    [JsonProperty("gesture2FDn", Order = 38, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGesture2FingerDown { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Right
    /// </summary>
    [JsonProperty("gesture2FRt", Order = 39, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGesture2FingerRight { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Left
    /// </summary>
    [JsonProperty("gesture2FLt", Order = 40, NullValueHandling = NullValueHandling.Ignore)]
    public ActionEvents EventsGesture2FingerLeft { get; set; }

    /// <summary>
    /// Buttons
    /// </summary>
    [JsonProperty("buttons", Order = 110)]
    public List<WebControlButton> Buttons { get; set; }

    [JsonProperty("states", Order = 111)]
    public Dictionary<int, WebControlState> States { get; set; }

    public static implicit operator WebControlPage(Page page)
    {
      return new WebControlPage()
      {
        Type = page.Type,
        PageID = page.PageID,
        Name = page.Name,
        Description = page.Description,
        Width = page.Width,
        Height = page.Height,
        AddressPort = page.AddressPort,
        AddressCode = page.AddressCode,
        ChannelPort = page.ChannelPort,
        ChannelCode = page.ChannelCode,
        EventsShowPage = page.EventsShowPage,
        EventsHidePage = page.EventsHidePage,
        EventsGestureAny = page.EventsGestureAny,
        EventsGestureUp = page.EventsGestureUp,
        EventsGestureDown = page.EventsGestureDown,
        EventsGestureRight = page.EventsGestureRight,
        EventsGestureLeft = page.EventsGestureLeft,
        EventsGestureDoubleTap = page.EventsGestureDoubleTap,
        EventsGesture2FingerUp = page.EventsGesture2FingerUp,
        EventsGesture2FingerDown = page.EventsGesture2FingerDown,
        EventsGesture2FingerRight = page.EventsGesture2FingerRight,
        EventsGesture2FingerLeft = page.EventsGesture2FingerLeft,
        Buttons = page.Buttons?.Select(s => (WebControlButton)s)?.ToList() ?? new List<WebControlButton>(),
        States = page.States?.ToDictionary(k => k.Number, e => (WebControlState)e) ?? new Dictionary<int, WebControlState>(),
      };
    }

    public static implicit operator Page(WebControlPage page)
    {
      return new Page()
      {
        Type = page.Type,
        PageID = page.PageID,
        Name = page.Name,
        Description = page.Description,
        Width = page.Width,
        Height = page.Height,
        AddressPort = page.AddressPort,
        AddressCode = page.AddressCode,
        ChannelPort = page.ChannelPort,
        ChannelCode = page.ChannelCode,
        EventsShowPage = page.EventsShowPage,
        EventsHidePage = page.EventsHidePage,
        EventsGestureAny = page.EventsGestureAny,
        EventsGestureUp = page.EventsGestureUp,
        EventsGestureDown = page.EventsGestureDown,
        EventsGestureRight = page.EventsGestureRight,
        EventsGestureLeft = page.EventsGestureLeft,
        EventsGestureDoubleTap = page.EventsGestureDoubleTap,
        EventsGesture2FingerUp = page.EventsGesture2FingerUp,
        EventsGesture2FingerDown = page.EventsGesture2FingerDown,
        EventsGesture2FingerRight = page.EventsGesture2FingerRight,
        EventsGesture2FingerLeft = page.EventsGesture2FingerLeft,
        Buttons = page.Buttons?.Select(s => (Button)s)?.ToList(),
        States = page.States?.Values?.Select(s => (State)s)?.ToList(),
      };
    }
  }
}
