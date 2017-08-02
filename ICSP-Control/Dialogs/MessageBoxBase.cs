using System.Windows.Forms;

namespace ICSPControl.Dialogs
{
  internal static class MessageBoxBase
  {
    internal static DialogResult Show(IWin32Window owner, string message, string caption, MessageBoxButtons mbButtons, MessageBoxIcon mbIcon, MessageBoxDefaultButton mbDefaultButton)
    {
      using (new MessageBoxWindowHook())
      {
        if (owner != null)
          return MessageBox.Show(owner, message, caption, mbButtons, mbIcon, mbDefaultButton);

        if (Application.OpenForms.Count > 0)
          return MessageBox.Show(Application.OpenForms[0], message, caption, mbButtons, mbIcon, mbDefaultButton);

        return MessageBox.Show(null, message, caption, mbButtons, mbIcon, mbDefaultButton);
      }
    }
  }
}