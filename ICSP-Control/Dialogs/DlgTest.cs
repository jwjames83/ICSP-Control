using System.Collections.Generic;
using System.Windows.Forms;

using ICSP;
using TpControls;

namespace ICSPControl.Dialogs
{
  public partial class DlgTest : Form
  {
    private ICSPManager mICSPManager;

    public DlgTest(ICSPManager manager)
    {
      InitializeComponent();

      mICSPManager = manager;

      if(mICSPManager != null)
      {
        foreach(var lButton in GetControlsOfType<TpButton>(this))
          lButton.SetManager(mICSPManager);
      }

      cmd_Close.Click += delegate { Close(); };
    }

    private static IEnumerable<T> GetControlsOfType<T>(Control root) where T : Control
    {
      var t = root as T;

      if(t != null)
        yield return t;

      foreach(Control c in root.Controls)
        foreach(var i in GetControlsOfType<T>(c))
          yield return i;
    }
  }
}
