using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum PopupType
  {
    [EnumMember(Value = "popup")]
    Popup,

    [EnumMember(Value = "subpage")]
    SubPage,

    [EnumMember(Value = "appwindow")]
    AppWindow,
  }
}
