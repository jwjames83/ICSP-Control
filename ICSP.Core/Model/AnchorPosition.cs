using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model
{  
  [JsonConverter(typeof(StringEnumConverter))]
  public enum AnchorPosition
  {
    [EnumMember(Value = null)]
    Center,

    [EnumMember(Value = "l/t")]
    LeftOrTop,

    [EnumMember(Value = "r/b")]
    RightOrBottom,
  }
}
