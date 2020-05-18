using System;
using System.Drawing;
using System.Windows.Forms;

using ICSPControl.Environment;

namespace ICSPControl.Dialogs
{
  public partial class ErrorDialog : Form
  {
    private string mMessage;

    public ErrorDialog()
    {
      InitializeComponent();

      Text = ProgramProperties.Title;

      rtb_Message.ReadOnly = true;
      rtb_Message.BackColor = Color.White;

      cmd_Clipboard.Click += OnClipboard_Click;
    }

    private void OnClipboard_Click(object sender, EventArgs e)
    {
      try
      {
        if (mMessage != null)
          Clipboard.SetText(mMessage);
        else
          Clipboard.SetText("Kein Text vorhanden.");
      }
      catch { }
    }

    protected override void OnLoad(EventArgs e)
    {
      if (!string.IsNullOrEmpty(mMessage))
      {
        var lFontSize = Graphics.FromHwnd(rtb_Message.Handle).MeasureString(mMessage, rtb_Message.Font);
        var lSize = base.Size;
        var lClientSize = rtb_Message.ClientSize;

        var lNewSize = new Size((lSize.Width + ((int)lFontSize.Width)) - lClientSize.Width, lSize.Height);

        base.Size = lNewSize;

        rtb_Message.Text = mMessage;
      }

      base.BringToFront();

      base.OnLoad(e);
    }

    public static void Show(Exception ex)
    {
      Show(ex.StackTrace);
    }

    public static void Show(string sMessage)
    {
      try
      {
        new ErrorDialog { Message = sMessage }.ShowDialog();
      }
      catch { }
    }

    public static void Show(Control parent, string sMessage)
    {
      try
      {
        new ErrorDialog { Message = sMessage }.ShowDialog(parent);
      }
      catch { }
    }

    public string Message
    {
      get
      {
        return mMessage;
      }
      set
      {
        mMessage = value;
      }
    }
  }
}