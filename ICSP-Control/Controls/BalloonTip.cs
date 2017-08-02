using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ICSPControl.Controls
{
  internal static class User32
  {
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr LPVOIDlpParam);
  }

  public class BalloonTip
  {
    private System.Timers.Timer mTimer = new System.Timers.Timer();
    private SemaphoreSlim mSemaphore = new SemaphoreSlim(1);
    private IntPtr mHWnd;

    private Control mControl;

    public enum Icon
    {
      None,
      Info,
      Warning,
      Error
    }

    public BalloonTip(Control control, string text)
    {
      if(control == null)
        throw new ArgumentNullException(nameof(control));

      mControl = control;

      Show(string.Empty, text);
    }

    public BalloonTip(Control control, string title, string text, Icon icon = 0, double timeOut = 0, bool focus = false)
    {
      if(control == null)
        throw new ArgumentNullException(nameof(control));

      mControl = control;

      Show(title, text, icon, timeOut, focus);
    }

    public BalloonTip(Control control, string title, string text, Icon icon = 0, double timeOut = 0, bool focus = false, short posX = 0, short posY = 0)
    {
      if(control == null)
        throw new ArgumentNullException(nameof(control));

      mControl = control;

      Show(title, text, icon, timeOut, focus, posX, posY);
    }

    private void Show(string title, string text, Icon icon = 0, double timeOut = 0, bool focus = false, short posX = 0, short posY = 0)
    {
      if(posX == 0 && posY == 0)
      {
        posX = (short)(mControl.RectangleToScreen(mControl.ClientRectangle).Left + mControl.Width / 2);
        posY = (short)(mControl.RectangleToScreen(mControl.ClientRectangle).Top + mControl.Height / 2);
      }

      var lToolInfo = new TOOLINFO();

      lToolInfo.cbSize = (uint)Marshal.SizeOf(lToolInfo);
      lToolInfo.uFlags = 0x20; // TTF_TRACK
      lToolInfo.lpszText = text;

      var lPtrToolInfo = Marshal.AllocCoTaskMem(Marshal.SizeOf(lToolInfo));
      Marshal.StructureToPtr(lToolInfo, lPtrToolInfo, false);

      var lBuffer = Encoding.UTF8.GetBytes(title + '\0');
      // lBuffer = lBuffer.Concat(new byte[] { 0 }).ToArray();

      var lPtrTitle = Marshal.AllocCoTaskMem(lBuffer.Length);
      Marshal.Copy(lBuffer, 0, lPtrTitle, lBuffer.Length);

      /*
      Styles:
      TTS_ALWAYSTIP      := 0x01
      TTS_NOPREFIX       := 0x02
      TTS_NOANIMATE      := 0x10
      TTS_NOFADE         := 0x20
      TTS_BALLOON        := 0x40
      TTS_CLOSE          := 0x80
      TTS_USEVISUALSTYLE := 0x100; >= Vista: use themed hyperlinks
      */

      // No Close Button
      var lStyle = (uint)(0x01 | 0x02 | 0x40);

      mHWnd = User32.CreateWindowEx(0x8, "tooltips_class32", "", lStyle,
        0, 0, 0, 0, mControl.Parent.Handle, (IntPtr)0, (IntPtr)0, (IntPtr)0);

      User32.SendMessage(mHWnd, 1028, (IntPtr)0, lPtrToolInfo); // TTM_ADDTOOL
      User32.SendMessage(mHWnd, 1042, (IntPtr)0, (IntPtr)((ushort)posX | ((ushort)posY << 16))); // TTM_TRACKPOSITION
      // User32.SendMessage(hWnd, 1044, (IntPtr)0xffff, (IntPtr)0); // TTM_SETTIPTEXTCOLOR
      User32.SendMessage(mHWnd, 1056, (IntPtr)icon, lPtrTitle); // TTM_SETTITLE 0:None, 1:Info, 2:Warning, 3:Error, >3:assumed to be an hIcon. ; 1057 for Unicode
      User32.SendMessage(mHWnd, 1048, (IntPtr)0, (IntPtr)500); // TTM_SETMAXTIPWIDTH
      User32.SendMessage(mHWnd, 0x40c, (IntPtr)0, lPtrToolInfo); // TTM_UPDATETIPTEXT; 0x439 for Unicode
      User32.SendMessage(mHWnd, 1041, (IntPtr)1, lPtrToolInfo); // TTM_TRACKACTIVATE

      Marshal.FreeCoTaskMem(lPtrTitle);
      Marshal.DestroyStructure(lPtrToolInfo, typeof(TOOLINFO));
      Marshal.FreeCoTaskMem(lPtrToolInfo);

      if(focus)
        mControl.Focus();

      // uncomment bellow to make balloon close when user changes focus,
      // starts typing, resizes/moves parent window, minimizes parent window, etc
      // adjust which control events to subscribe to depending on the control over which the balloon tip is shown

      //
      mControl.Click += OnControlEvent;
      mControl.Leave += OnControlEvent;
      mControl.TextChanged += OnControlEvent;
      mControl.LocationChanged += OnControlEvent;
      mControl.SizeChanged += OnControlEvent;
      mControl.VisibleChanged += OnControlEvent;

      var lParent = mControl.Parent;

      while(lParent != null)
      {
        lParent.VisibleChanged += OnParentVisibleChanged;
        lParent = lParent.Parent;
      }

      mControl.TopLevelControl.LocationChanged += OnFormLocationChanged;
      ((Form)mControl.TopLevelControl).Deactivate += OnFormDeactivate;
      //

      mTimer.AutoReset = false;
      mTimer.Elapsed += OnTimerElapsed;

      if(timeOut > 0)
      {
        mTimer.Interval = timeOut;
        mTimer.Start();
      }
    }

    private void OnParentVisibleChanged(object sender, EventArgs e)
    {
      Close();
    }

    private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      Close();
    }

    private void OnControlEvent(object sender, EventArgs e)
    {
      Close();
    }

    private void OnFormLocationChanged(object sender, EventArgs e)
    {
      Close();
    }

    private void OnFormDeactivate(object sender, EventArgs e)
    {
      Close();
    }

    public void Close()
    {
      // Ensures one time only execution
      if(!mSemaphore.Wait(0))
        return;

      mTimer.Elapsed -= OnTimerElapsed;
      mTimer.Close();

      mControl.Click -= OnControlEvent;
      mControl.Leave -= OnControlEvent;
      mControl.TextChanged -= OnControlEvent;
      mControl.LocationChanged -= OnControlEvent;
      mControl.SizeChanged -= OnControlEvent;
      mControl.VisibleChanged -= OnControlEvent;

      var lParent = mControl.Parent;

      while(lParent != null)
      {
        lParent.VisibleChanged -= OnParentVisibleChanged;
        lParent = lParent.Parent;
      }

      mControl.TopLevelControl.LocationChanged -= OnFormLocationChanged;
      ((Form)mControl.TopLevelControl).Deactivate -= OnFormDeactivate;

      mControl = null;

      User32.SendMessage(mHWnd, 0x0010, (IntPtr)0, (IntPtr)0); // WM_CLOSE
      // User32.SendMessage(hWnd, 0x0002, (IntPtr)0, (IntPtr)0); // WM_DESTROY
      // User32.SendMessage(hWnd, 0x0082, (IntPtr)0, (IntPtr)0); // WM_NCDESTROY
    }

    [StructLayout(LayoutKind.Sequential)]
    struct TOOLINFO
    {
      public uint cbSize;
      public uint uFlags;
      public IntPtr hwnd;
      public IntPtr uId;
      public RECT rect;
      public IntPtr hinst;
      [MarshalAs(UnmanagedType.LPStr)]
      public string lpszText;
      public IntPtr lParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }
  }
}
