using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model.ProjectProperties
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
