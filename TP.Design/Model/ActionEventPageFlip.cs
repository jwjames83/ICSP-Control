﻿using System.ComponentModel;

using Newtonsoft.Json;

namespace TP.Design.Model
{
  public class ActionEventPageFlip
  {
    /// <summary>
    /// Item
    /// </summary>
    [JsonProperty("item", Order = 1)]
    public int Item { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type", Order = 2)]
    public PageFlipType Type { get; set; }

    /// <summary>
    /// Animation
    /// </summary>
    [DefaultValue(AnimationType.None)]
    [JsonProperty("ani", Order = 3, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public AnimationType Animation { get; set; }

    /// <summary>
    /// Animation: Origin
    /// </summary>
    [JsonProperty("origin", Order = 4, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public AnimationDirection Origin { get; set; }

    /// <summary>
    /// Animation: Duration
    /// </summary>
    [DefaultValue(15)]
    [JsonProperty("dur", Order = 5, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Duration { get; set; }
    
    /// <summary>
    /// Value
    /// </summary>
    [JsonProperty("#text", Order = 6, NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; set; }

    public static explicit operator ActionEventPageFlip(PageFlip pageFlip)
    {
      return new ActionEventPageFlip()
      {
        Type = pageFlip.Type,
        Value = pageFlip.Value,
      };
    }

    public static implicit operator PageFlip(ActionEventPageFlip pageFlip)
    {
      return new PageFlip()
      {
        Type = pageFlip.Type,
        Value = pageFlip.Value,
      };
    }
  }
}
