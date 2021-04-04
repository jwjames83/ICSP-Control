using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum LevelControlType
  {
    [EnumMember(Value = null)]
    None,

    [EnumMember(Value = "abs")]
    Absolute,

    [EnumMember(Value = "rel")]
    Relative,
  }
}
