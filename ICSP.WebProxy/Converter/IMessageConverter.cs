using ICSP.Core;
using ICSP.Core.Manager.DeviceManager;

namespace ICSP.WebProxy.Converter
{
  public interface IMessageConverter
  {
    string FromChannelEvent(ChannelEventArgs e);

    string FromLevelEvent(LevelEventArgs e);

    string FromCommandEvent(CommandEventArgs e);

    string FromStringEvent(StringEventArgs e);
    
    ICSPMsg ToDevMessage(string msg);
  }
}