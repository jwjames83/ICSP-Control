using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum LevelFunction
  {
    [EnumMember(Value = null)]
    DisplayOnly,

    [EnumMember(Value = "active")]
    Active,
    
    [EnumMember(Value = "center")]
    ActiveCentering,

    [EnumMember(Value = "drag")]
    Drag,

    [EnumMember(Value = "dragCenter")]
    DragCentering,
  }
}
