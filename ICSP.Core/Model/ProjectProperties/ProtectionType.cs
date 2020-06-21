using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model.ProjectProperties
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum ProtectionType
  {
    [EnumMember(Value = "none")]
    None,

    [EnumMember(Value = "readOnly")]
    ReadOnly,

    [EnumMember(Value = "locked")]
    Locked,
  }
}
