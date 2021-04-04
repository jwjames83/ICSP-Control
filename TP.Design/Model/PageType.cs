using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum PageType
  {
    [EnumMember(Value = "page")]
    Page,

    [EnumMember(Value = "subpage")]
    SubPage
  }
}
