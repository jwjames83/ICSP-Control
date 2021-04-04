using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum Orientation
  {
    [EnumMember(Value = null)]
    Horizontal,

    [EnumMember(Value = "vert")]
    Vertical,
  }
}
