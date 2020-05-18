using System.Windows.Forms;

using ICSPControl.Environment;

namespace ICSPControl.Dialogs
{
  public static class OKCancelMessageBox
  {
    public static DialogResult Show(string message)
    {
      return MessageBoxBase.Show(null, message, ProgramProperties.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
    }

    public static DialogResult Show(string message, params object[] args)
    {
      return MessageBoxBase.Show(null, string.Format(message, args), ProgramProperties.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
    }

    public static DialogResult Show(IWin32Window owner, string message)
    {
      return MessageBoxBase.Show(owner, message, ProgramProperties.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
    }
  }
}
