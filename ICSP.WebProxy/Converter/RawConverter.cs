using ICSP.Core;

using Microsoft.Extensions.Configuration;

namespace ICSP.WebProxy.Converter
{
  public class RawConverter : IMessageConverter
  {
    public ushort Device { get; set; }

    public ushort System { get; set; }

    public AmxDevice Dest { get; set; }

    public string FromChannelEvent(ChannelEventArgs e)
    {
      if(e.Enabled)
        return string.Concat("[", e.Device, "][Channel][On]:", e.Channel);
      else
        return string.Concat("[", e.Device, "][Channel][Off]:", e.Channel);
    }

    public string FromLevelEvent(LevelEventArgs e)
    {
      return string.Concat("[", e.Device, "][Level]:", e.Level, e.Value);
    }

    public string FromCommandEvent(CommandEventArgs e)
    {
      return string.Concat("[", e.Device, "][Command]:", e.Text);
    }

    public string FromStringEvent(StringEventArgs e)
    {
      return string.Concat("[", e.Device, "][String]:", e.Text);
    }

    public ICSPMsg DeviceOnline()
    {
      return null;
    }

    public string DeviceOffline()
    {
      return string.Concat("[", new AmxDevice(Device, 1, System), "][Offline]:");
    }

    public ICSPMsg ToDevMessage(string msg)
    {
      return null;
    }
  }
}
