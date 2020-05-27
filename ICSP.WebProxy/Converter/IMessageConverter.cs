using ICSP.Core;

namespace ICSP.WebProxy.Converter
{
  public interface IMessageConverter
  {
    public ushort Device { set; }

    public ushort System { set; }

    AmxDevice Dest { set; }

    string FromChannelEvent(ChannelEventArgs e);

    string FromLevelEvent(LevelEventArgs e);

    string FromCommandEvent(CommandEventArgs e);

    string FromStringEvent(StringEventArgs e);
    
    ICSPMsg ToDevMessage(string msg);
  }
}