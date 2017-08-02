using System;
using System.Runtime.InteropServices;

namespace ICSPControl.Dialogs
{
  internal class MessageBoxWindowHook : IDisposable
  {
    public enum HookType
    {
      WH_CBT = 5
    }

    private IntPtr mHook = IntPtr.Zero;

    private const int HCBT_ACTIVATE = 5;
    private const int ID_BUT_ABORT = 3;
    private const int ID_BUT_CANCEL = 2;
    private const int ID_BUT_IGNORE = 5;
    private const int ID_BUT_NO = 7;
    private const int ID_BUT_OK = 1;
    private const int ID_BUT_RETRY = 4;
    private const int ID_BUT_YES = 6;

    private delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool SetDlgItemTextW(IntPtr hDlg, int nIDDlgItem, [MarshalAs(UnmanagedType.LPWStr)] string lpString);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, int dwThreadId);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    public MessageBoxWindowHook()
    {
      HookDialog();
    }

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    public void Dispose()
    {
      UnHookDialog();
    }

    private void HookDialog()
    {
      UnHookDialog();

#pragma warning disable 0618

      mHook = SetWindowsHookEx(HookType.WH_CBT, new HookProc(HookProcedure), IntPtr.Zero, AppDomain.GetCurrentThreadId());

#pragma warning restore 0618

    }

    private void UnHookDialog()
    {
      if (!mHook.Equals(IntPtr.Zero))
      {
        UnhookWindowsHookEx(mHook);
        mHook = IntPtr.Zero;
      }
    }

    private IntPtr HookProcedure(int code, IntPtr wParam, IntPtr lParam)
    {
      if (code == 5)
      {
        SetDlgItemTextW(wParam, 1, "OK");
        SetDlgItemTextW(wParam, 2, "Abbrechen");
        SetDlgItemTextW(wParam, 3, "Abbrechen");
        SetDlgItemTextW(wParam, 4, "Wiederholen");
        SetDlgItemTextW(wParam, 5, "Ignorieren");
        SetDlgItemTextW(wParam, 6, "Ja");
        SetDlgItemTextW(wParam, 7, "Nein");

        UnHookDialog();
      }

      return CallNextHookEx(mHook, code, wParam, lParam);
    }
  }
}