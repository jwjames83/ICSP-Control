using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TP.Design.Model
{
  public class Page
  {
    [JsonExtensionData]
    private readonly IDictionary<string, JToken> mAdditionalData;

    public Page()
    {
      mAdditionalData = new Dictionary<string, JToken>();
    }

    /// <summary>
    /// Page type ("page", "subpage")
    /// </summary>
    [JsonProperty("type", Order = 1)]
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
    [JsonProperty("name", Order = 4)]
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
    [JsonIgnore]
    public ActionEvents EventsGestureAny { get; set; }

    /// <summary>
    /// G5 Events: Gesture Up
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGestureUp { get; set; }

    /// <summary>
    /// G5 Events: Gesture Down
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGestureDown { get; set; }

    /// <summary>
    /// G5 Events: Gesture Right
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGestureRight { get; set; }

    /// <summary>
    /// G5 Events: Gesture Left
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGestureLeft { get; set; }

    /// <summary>
    /// G5 Events: Gesture Double-Tap
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGestureDoubleTap { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Up
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGesture2FingerUp { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Down
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGesture2FingerDown { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Right
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGesture2FingerRight { get; set; }

    /// <summary>
    /// G5 Events: Gesture 2-Finger Left
    /// </summary>
    [JsonIgnore]
    public ActionEvents EventsGesture2FingerLeft { get; set; }

    /// <summary>
    /// Buttons
    /// </summary>
    [JsonProperty("button", Order = 110)]
    public List<Button> Buttons { get; set; }

    /// <summary>
    /// States
    /// </summary>
    [JsonProperty("sr", Order = 111)]
    public List<State> States { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      try
      {
        // Gesture Any
        if(mAdditionalData.TryGetValue("gestureAny", out var gestureAny))
        {
          if(gestureAny["ga"] != null)
            EventsGestureAny = JsonConvert.DeserializeObject<ActionEvents>(gestureAny["ga"].ToString());
        }

        // Gesture Up
        if(mAdditionalData.TryGetValue("gestureUp", out var gestureUp))
        {
          if(gestureUp["gu"] != null)
            EventsGestureUp = JsonConvert.DeserializeObject<ActionEvents>(gestureUp["gu"].ToString());
        }

        // Gesture Down
        if(mAdditionalData.TryGetValue("gestureDown", out var gestureDown))
        {
          if(gestureDown["gd"] != null)
            EventsGestureDown = JsonConvert.DeserializeObject<ActionEvents>(gestureDown["gd"].ToString());
        }

        // Gesture Right
        if(mAdditionalData.TryGetValue("gestureRight", out var gestureRight))
        {
          if(gestureRight["gr"] != null)
            EventsGestureRight = JsonConvert.DeserializeObject<ActionEvents>(gestureRight["gr"].ToString());
        }

        // Gesture Left
        if(mAdditionalData.TryGetValue("gestureLeft", out var gestureLeft))
        {
          if(gestureLeft["gl"] != null)
            EventsGestureLeft = JsonConvert.DeserializeObject<ActionEvents>(gestureLeft["gl"].ToString());
        }

        // Gesture Double-Tap
        if(mAdditionalData.TryGetValue("gestureDblTap", out var gestureDblTap))
        {
          if(gestureDblTap["gt"] != null)
            EventsGestureDoubleTap = JsonConvert.DeserializeObject<ActionEvents>(gestureDblTap["gt"].ToString());
        }

        // Gesture 2-Finger Up
        if(mAdditionalData.TryGetValue("gesture2FUp", out var gesture2FUp))
        {
          if(gesture2FUp["tu"] != null)
            EventsGesture2FingerUp = JsonConvert.DeserializeObject<ActionEvents>(gesture2FUp["tu"].ToString());
        }

        // Gesture 2-Finger Down
        if(mAdditionalData.TryGetValue("gesture2FDn", out var gesture2FDn))
        {
          if(gesture2FDn["td"] != null)
            EventsGesture2FingerDown = JsonConvert.DeserializeObject<ActionEvents>(gesture2FDn["td"].ToString());
        }

        // Gesture 2-Finger Right
        if(mAdditionalData.TryGetValue("gesture2FRt", out var gesture2FRt))
        {
          if(gesture2FRt["tr"] != null)
            EventsGesture2FingerRight = JsonConvert.DeserializeObject<ActionEvents>(gesture2FRt["tr"].ToString());
        }

        // Gesture 2-Finger Left
        if(mAdditionalData.TryGetValue("gesture2FLt", out var gesture2FLt))
        {
          if(gesture2FLt["tl"] != null)
            EventsGesture2FingerLeft = JsonConvert.DeserializeObject<ActionEvents>(gesture2FLt["tl"].ToString());
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}
