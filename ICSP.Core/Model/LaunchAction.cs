using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ICSP.Core.Model
{
  /// <summary>
  /// Launch Actions allow you to show/hide Application Windows based on a Page or Button Event.
  /// </summary>
  [JsonConverter(typeof(StringEnumConverter))]
  public enum LaunchAction
  {
    [EnumMember(Value = null)]
    None,

    /// <summary>
    /// Select a target Application Window for the launch action to open
    /// </summary>
    [EnumMember(Value = "show")]
    Show,

    /// <summary>
    /// Select a target Application Window for the launch action to close.
    /// </summary>
    [EnumMember(Value = "close")]
    Close,

    /// <summary>
    /// This selection closes all open Application Windows.
    /// </summary>
    [EnumMember(Value = "close_all")]
    CloseAll,

    /// <summary>
    /// This selection displays Application Status information on the panel.
    /// </summary>
    [EnumMember(Value = "status_show")]
    StatusShow,

    /// <summary>
    /// This selection hides Application Status information on the panel.
    /// </summary>
    [EnumMember(Value = "status_hide")]
    StatusHide,
  }
}