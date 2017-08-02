using System;
using System.Runtime.InteropServices;

namespace ICSPControl
{
  public class User32Wrappers
  {
    // Methods
    [DllImport("user32")]
    public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);

    [DllImport("user32")]
    public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, WS_EX dsNewLong);

    // Nested Types
    public enum GWL
    {
      ExStyle = -20
    }

    public enum LWA
    {
      Alpha = 2,
      ColorKey = 1
    }

    public enum WS_EX
    {
      Layered = 0x80000,
      Transparent = 0x20
    }
  }
}
