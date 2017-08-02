namespace ICSPControl.Dialogs
{
  partial class ErrorDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.cmd_Clipboard = new System.Windows.Forms.Button();
      this.cmd_OK = new System.Windows.Forms.Button();
      this.rtb_Message = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // cmd_Clipboard
      // 
      this.cmd_Clipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cmd_Clipboard.Location = new System.Drawing.Point(540, 409);
      this.cmd_Clipboard.Name = "cmd_Clipboard";
      this.cmd_Clipboard.Size = new System.Drawing.Size(75, 23);
      this.cmd_Clipboard.TabIndex = 2;
      this.cmd_Clipboard.Text = "&Kopieren";
      this.cmd_Clipboard.UseVisualStyleBackColor = true;
      // 
      // cmd_OK
      // 
      this.cmd_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cmd_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cmd_OK.Location = new System.Drawing.Point(459, 409);
      this.cmd_OK.Name = "cmd_OK";
      this.cmd_OK.Size = new System.Drawing.Size(75, 23);
      this.cmd_OK.TabIndex = 1;
      this.cmd_OK.Text = "OK";
      this.cmd_OK.UseVisualStyleBackColor = true;
      // 
      // rtb_Message
      // 
      this.rtb_Message.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rtb_Message.Location = new System.Drawing.Point(0, 0);
      this.rtb_Message.Name = "rtb_Message";
      this.rtb_Message.Size = new System.Drawing.Size(629, 403);
      this.rtb_Message.TabIndex = 0;
      this.rtb_Message.Text = "";
      // 
      // ErrorDialog
      // 
      this.AcceptButton = this.cmd_OK;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.CancelButton = this.cmd_OK;
      this.ClientSize = new System.Drawing.Size(627, 444);
      this.ControlBox = false;
      this.Controls.Add(this.rtb_Message);
      this.Controls.Add(this.cmd_OK);
      this.Controls.Add(this.cmd_Clipboard);
      this.Name = "ErrorDialog";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "PDAT Error";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button cmd_Clipboard;
    private System.Windows.Forms.Button cmd_OK;
    private System.Windows.Forms.RichTextBox rtb_Message;
  }
}