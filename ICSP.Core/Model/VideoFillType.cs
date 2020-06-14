using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum VideoFillType
  {
    [EnumMember(Value = null)]
    None,

    [EnumMember(Value = "100")]
    StreamingVideo,

    [EnumMember(Value = "101")]
    MXA_MPL,
  }
}
