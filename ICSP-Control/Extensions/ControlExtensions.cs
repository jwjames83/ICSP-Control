using System;
using System.Windows.Forms;

namespace ICSPControl.Extensions
{
  public static class ControlExtensions
  {
    public static void InvokeIfRequired<T>(this T control, Action<T> action) where T : Control
    {
      if(control != null)
      {
        if(control.InvokeRequired)
          control.BeginInvoke(new Action(() => action(control)));
        else
          action(control);
      }
    }
  }
}
