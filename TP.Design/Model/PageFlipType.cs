using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TP.Design.Model
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum PageFlipType
  {
    /// <summary>
    /// This selection populates the Target list box with a list of standard pages in your project.<br/>
    /// Select a target page for the page flip. Multiple Standard page flips are allowed.
    /// </summary>
    [EnumMember(Value = "stan")]
    StandardPage,

    /// <summary>
    /// This selection sets the page flip to go to the previous page (relative to the order of existing page flips) when the button is touched.
    /// </summary>
    [EnumMember(Value = "prev")]
    PreviousPage,

    /// <summary>
    /// This selection populates the Target list box with a list of popup pages in your project.<br/>
    /// Select a target popup page for the page flip (to show when the button is touched).
    /// </summary>
    [EnumMember(Value = "sShow")]
    ShowPopup,

    /// <summary>
    /// This selection populates the Target list box with a list of popup pages in your project.<br/>
    /// Select a target popup page for the page flip (to hide when the button is touched).
    /// </summary>
    [EnumMember(Value = "sHide")]
    HidePopup,

    /// <summary>
    /// This selection populates the Target list box with a list of popup pages in your project.<br/>
    /// Select a target popup page for the page flip (to toggle hide/show when the button is touched).
    /// </summary>
    [EnumMember(Value = "sToggle")]
    TogglePopup,

    /// <summary>
    /// This selection populates the Target list box with a list of popup page groups in your project.<br/>
    /// Select a target popup page group for the page flip (to hide when the button is touched).
    /// </summary>
    [EnumMember(Value = "scGroup")]
    HidePopupGroup,

    /// <summary>
    /// This selection populates the Target list box with a list of standard pages in your project.<br/>
    /// Select the page that you want to hide the Popups on when the button is touched.
    /// </summary>
    [EnumMember(Value = "scPage")]
    HidePopupsOnPage,

    /// <summary>
    /// This selection sets the page flip to clear all popup pages when the button is touched.
    /// </summary>
    [EnumMember(Value = "scPanel")]
    HideAllPopups,

    /// <summary>
    /// (G4 Only)<br/>
    /// This selection flips to the page specified, but does not retain the source page in the page flip stack (which is used to execute previous page flips).<br/>
    /// Use this option when you do not wish to return to the page that initiated the page flip when a subsequent previous page flip action is performed.<br/>
    /// This allows the user start on page one, flip to page two, flip to page three, then with a page flip go directly back to page one.
    /// </summary>
    [EnumMember(Value = "forget")]
    PageFlipForget,

    /// <summary>
    /// This selection populates the Target list box with a list of standard pages in your project.<br/>
    /// Select a target page for the page flip. Multiple Standard page flips are allowed.<br/>
    /// (MVP-9000i only)
    /// </summary>
    [EnumMember(Value = "stanAni")]
    StandardAnimated,

    /// <summary>
    /// This selection sets the page flip to go to the previous page (relative to the order of existing page flips) when the button is touched.<br/>
    /// (MVP-9000i only)
    /// </summary>
    [EnumMember(Value = "prevAni")]
    PreviousAnimated,
  }
}