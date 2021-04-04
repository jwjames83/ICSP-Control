using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model.ProjectProperties
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum FormatType
  {
    [EnumMember(Value = "xport-s")]
    XPort,

    [EnumMember(Value = "csv-headers")]
    CSVHeaders,

    [EnumMember(Value = "csv")]
    CSV,
  }
}