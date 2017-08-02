using System.Windows.Forms;

using ICSPControl.Environment;

namespace ICSPControl.Dialogs
{
  public static class InfoMessageBox
  {
    public static void Show(string sMessage)
    {
      MessageBoxBase.Show(null, sMessage, ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
    }

    public static void Show(string sMessage, params object[] args)
    {
      MessageBoxBase.Show(null, string.Format(sMessage, args), ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
    }

    public static void Show(IWin32Window owner, string sMessage)
    {
      MessageBoxBase.Show(owner, sMessage, ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
    }
  }
}