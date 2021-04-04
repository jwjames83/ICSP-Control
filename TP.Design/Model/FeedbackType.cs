using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum FeedbackType
  {
    [EnumMember(Value = "none")]
    None,

    [EnumMember(Value = null)]
    Channel,

    [EnumMember(Value = "inverted")]
    InvertedChannel,

    [EnumMember(Value = "on")]
    AlwaysOn,

    [EnumMember(Value = "momentary")]
    Momentary,

    [EnumMember(Value = "blink")]
    Blink,
  }
}
