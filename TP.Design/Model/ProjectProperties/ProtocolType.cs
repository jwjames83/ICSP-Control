using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model.ProjectProperties
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum ProtocolType
  {
    [EnumMember(Value = "HTTP")]
    HTTP,

    [EnumMember(Value = "HTTPS")]
    HTTPS,
  }
}
