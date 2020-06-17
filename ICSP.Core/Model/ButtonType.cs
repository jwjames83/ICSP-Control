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
    MultiStateGeneral,

    [EnumMember(Value = "bargraph")]
    Bargraph,

    [EnumMember(Value = "multiBargraph")]
    MultiStateBargraph,

    // G4 Only
    [EnumMember(Value = "joystick")]
    Joystick,

    [EnumMember(Value = "textArea")]
    TextInput,

    // G4 Only
    [EnumMember(Value = "virtualPC")]
    ComputerControl,

    // G4 Only
    [EnumMember(Value = "annotation")]
    TakeNote,

    [EnumMember(Value = "subPageView")]
    SubPageView,

    [EnumMember(Value = "listView")]
    ListView,
  }
}
