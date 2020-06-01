using ICSP.Core;

namespace ICSP.WebProxy.Converter
{
  public interface IMessageConverter
  {
    /// <summary>
    /// The source-device if an message of type ICSPMsg is to be created
    /// </summary>
    ushort Device { set; }

    /// <summary>
    /// The source-system if an message of type ICSPMsg is to be created
    /// </summary>
    ushort System { set; }

    /// <summary>
    /// The destination device if an message of type ICSPMsg is to be created
    /// </summary>
    AmxDevice Dest { set; }

    /// <summary>
    /// Converts the arguments of type ChannelEventArgs into a string, that is sent to the WebSocket.<br/>
    /// If the conversion is not supported, null can be returned.
    /// </summary>
    string FromChannelEvent(ChannelEventArgs e);

    /// <summary>
    /// Converts the arguments of type LevelEventArgs into a string, that is sent to the WebSocket.<br/>
    /// If the conversion is not supported, null can be returned.
    /// </summary>
    string FromLevelEvent(LevelEventArgs e);

    /// <summary>
    /// Converts the arguments of type CommandEventArgs into a string, that is sent to the WebSocket.<br/>
    /// If the conversion is not supported, null can be returned.
    /// </summary>
    string FromCommandEvent(CommandEventArgs e);

    /// <summary>
    /// Converts the arguments of type StringEventArgs into a string, that is sent to the WebSocket.<br/>
    /// If the conversion is not supported, null can be returned.
    /// </summary>
    string FromStringEvent(StringEventArgs e);

    /// <summary>
    /// Creates a object of type ICSPMsg, that is sent to the master when a device is online.<br/>
    /// If the conversion is not needed, null can be returned.
    /// </summary>
    ICSPMsg DeviceOnline();

    /// <summary>
    /// Creates a string, that is sent to the WebSocket, when a device goes offline.<br/>
    /// If the conversion is not needed, null can be returned.
    /// </summary>
    string DeviceOffline();

    /// <summary>
    /// Creates a object of type ICSPMsg, that is sent to the master for a WebSocket-Request.<br/>
    /// If the conversion is not needed, null can be returned.
    /// </summary>
    ICSPMsg ToDevMessage(string msg);
  }
}