using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum DragDropType
  {
    [EnumMember(Value = null)]
    None,

    [EnumMember(Value = "dr")]
    Draggable,

    [EnumMember(Value = "dt")]
    DropTarget,
  }
}
