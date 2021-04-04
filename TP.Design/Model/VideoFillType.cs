using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum VideoFillType
  {
    [EnumMember(Value = null)]
    None,

    [EnumMember(Value = "1")]
    VideoSlot1,

    [EnumMember(Value = "2")]
    VideoSlot2,

    [EnumMember(Value = "3")]
    VideoSlot3,

    [EnumMember(Value = "4")]
    VideoSlot4,

    [EnumMember(Value = "100")]
    StreamingVideo,

    [EnumMember(Value = "101")]
    MXA_MPL,
  }
}
