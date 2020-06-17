using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum SliderType
  {
    [EnumMember(Value = null)]
    None,

    [EnumMember(Value = "Ball")]
    Ball,

    [EnumMember(Value = "Circle -L")]
    CircleLarge,

    [EnumMember(Value = "Circle -M")]
    CircleMedium,

    [EnumMember(Value = "Circle -S")]
    CircleSmall,

    [EnumMember(Value = "Precision")]
    Precision,

    [EnumMember(Value = "Rectangle -L")]
    RectangleLarge,

    [EnumMember(Value = "Rectangle -M")]
    RectangleMedium,

    [EnumMember(Value = "Rectangle -S")]
    RectangleSmall,

    [EnumMember(Value = "Smart Button Bubbled Large")]
    SmartButtonBubbledLarge,

    [EnumMember(Value = "Smart Button Bubbled Small")]
    SmartButtonBubbledSmall,

    [EnumMember(Value = "Windows")]
    Windows,

    [EnumMember(Value = "Windows Active")]
    WindowsActive,
  }
}
