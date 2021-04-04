using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum DisplayType
  {
    [EnumMember(Value = null)]
    SingleLine,

    [EnumMember(Value = "multiple")]
    MultipleLines,
  }
}
