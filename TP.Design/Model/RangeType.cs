using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  /// (sByte, sint, "slong"long, int)
  [JsonConverter(typeof(StringEnumConverter))]
  public enum RangeType
  {
    /// <summary>
    /// 0:255 (System.Byte)
    /// </summary>
    [EnumMember(Value = null)]
    Byte,

    /// <summary>
    /// 0:65535 (System.UInt16)
    /// </summary>
    [EnumMember(Value = "int")]
    UInt16,

    /// <summary>
    /// 0:4294967295 (System.UInt32)
    /// </summary>
    [EnumMember(Value = "long")]
    UInt32,

    /// <summary>
    /// -128:127 (System.SByte)
    /// </summary>
    [EnumMember(Value = "sByte")]
    SByte,

    /// <summary>
    /// -32768: 32767 (System.Int16)
    /// </summary>
    [EnumMember(Value = "sint")]
    Int16,

    /// <summary>
    /// -999999999:999999999 (System.Int32)
    /// </summary>
    [EnumMember(Value = "slong")]
    Int32,
  }
}
