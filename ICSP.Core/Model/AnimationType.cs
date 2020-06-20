using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model
{
  /// </summary>
  [JsonConverter(typeof(StringEnumConverter))]
  public enum AnimationType
  {
    [EnumMember(Value = null)]
    None,

    [EnumMember(Value = "slide")]
    Slide,

    [EnumMember(Value = "sldBounce")]
    SlideBounce,

    [EnumMember(Value = "blkGlass")]
    BlackGlass,

    [EnumMember(Value = "fade")]
    Fade,

    [EnumMember(Value = "doorFade")]
    DoorFade,

    [EnumMember(Value = "cntrDrFade")]
    CenterDoorFade,

    [EnumMember(Value = "pgCurl")]
    PageCurl,

    [EnumMember(Value = "zoomIn")]
    ZoomIn,

    [EnumMember(Value = "zoomOut")]
    ZoomOut,

    [EnumMember(Value = "implode")]
    Implode,

    [EnumMember(Value = "explode")]
    Explode,

    [EnumMember(Value = "spinIn")]
    SpinIn,

    [EnumMember(Value = "spinOut")]
    SpinOut,

    [EnumMember(Value = "flipCard")]
    FlipCard,

    [EnumMember(Value = "tumble")]
    Tumble,

    [EnumMember(Value = "slats")]
    Slats,

    [EnumMember(Value = "squish")]
    Squish,

    [EnumMember(Value = "wipe")]
    Wipe,

    [EnumMember(Value = "waves")]
    Waves,
  }
}