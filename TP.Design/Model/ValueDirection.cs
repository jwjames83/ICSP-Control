﻿using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum ValueDirection
  {
    [EnumMember(Value = "vertical")]
    Vertical,

    [EnumMember(Value = "horizontal")]
    Horizontal,

    // Multi-State-Bargraph only
    [EnumMember(Value = "touchMap")]
    TouchMap,
  }
}
