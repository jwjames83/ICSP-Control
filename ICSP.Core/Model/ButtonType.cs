using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum ButtonType
  {
    [EnumMember(Value = "general")]
    General,

    [EnumMember(Value = "multiGeneral")]
    MultiGeneral,

    [EnumMember(Value = "bargraph")]
    Bargraph,

    [EnumMember(Value = "multiBargraph")]
    MultiBargraph,

    [EnumMember(Value = "textArea")]
    TextArea,

    [EnumMember(Value = "subPageView")]
    SubPageView,

    [EnumMember(Value = "listView")]
    ListView,
  }
}
