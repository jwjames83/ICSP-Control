using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum TouchStyle
  {
    [EnumMember(Value = null)]
    ActiveTouch,

    [EnumMember(Value = "bounding")]
    BoundingBox,

    [EnumMember(Value = "passThru")]
    PassThrough,
  }
}
