namespace ICSPControl.Dialogs
{
  partial class DlgNotifications
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
      if(disposing && (components != null))
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
      this.cmd_StartStopLog = new System.Windows.Forms.Button();
      this.txt_Text = new System.Windows.Forms.TextBox();
      this.cmd_ClearLog = new System.Windows.Forms.Button();
      this.ckb_ShowPing = new System.Windows.Forms.CheckBox();
      this.ckb_ShowBlink = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // cmd_StartStopLog
      // 
      this.cmd_StartStopLog.Location = new System.Drawing.Point(12, 12);
      this.cmd_StartStopLog.Name = "cmd_StartStopLog";
      this.cmd_StartStopLog.Size = new System.Drawing.Size(116, 23);
      this.cmd_StartStopLog.TabIndex = 31;
      this.cmd_StartStopLog.Text = "Stop Log";
      this.cmd_StartStopLog.UseVisualStyleBackColor = true;
      // 
      // txt_Text
      // 
      this.txt_Text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_Text.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_Text.Location = new System.Drawing.Point(12, 41);
      this.txt_Text.Multiline = true;
      this.txt_Text.Name = "txt_Text";
      this.txt_Text.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txt_Text.Size = new System.Drawing.Size(896, 408);
      this.txt_Text.TabIndex = 30;
      this.txt_Text.WordWrap = false;
      // 
      // cmd_ClearLog
      // 
      this.cmd_ClearLog.Location = new System.Drawing.Point(134, 12);
      this.cmd_ClearLog.Name = "cmd_ClearLog";
      this.cmd_ClearLog.Size = new System.Drawing.Size(116, 23);
      this.cmd_ClearLog.TabIndex = 32;
      this.cmd_ClearLog.Text = "Clear Log";
      this.cmd_ClearLog.UseVisualStyleBackColor = true;
      // 
      // ckb_ShowPing
      // 
      this.ckb_ShowPing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ckb_ShowPing.AutoSize = true;
      this.ckb_ShowPing.Location = new System.Drawing.Point(652, 18);
      this.ckb_ShowPing.Name = "ckb_ShowPing";
      this.ckb_ShowPing.Size = new System.Drawing.Size(123, 17);
      this.ckb_ShowPing.TabIndex = 33;
      this.ckb_ShowPing.Text = "Show Ping Message";
      this.ckb_ShowPing.UseVisualStyleBackColor = true;
      // 
      // ckb_ShowBlink
      // 
      this.ckb_ShowBlink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ckb_ShowBlink.AutoSize = true;
      this.ckb_ShowBlink.Location = new System.Drawing.Point(781, 18);
      this.ckb_ShowBlink.Name = "ckb_ShowBlink";
      this.ckb_ShowBlink.Size = new System.Drawing.Size(125, 17);
      this.ckb_ShowBlink.TabIndex = 34;
      this.ckb_ShowBlink.Text = "Show Blink Message";
      this.ckb_ShowBlink.UseVisualStyleBackColor = true;
      // 
      // DlgNotifications
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(920, 461);
      this.Controls.Add(this.ckb_ShowBlink);
      this.Controls.Add(this.ckb_ShowPing);
      this.Controls.Add(this.cmd_ClearLog);
      this.Controls.Add(this.cmd_StartStopLog);
      this.Controls.Add(this.txt_Text);
      this.Name = "DlgNotifications";
      this.Text = "DlgNotifications";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button cmd_StartStopLog;
    private System.Windows.Forms.TextBox txt_Text;
    private System.Windows.Forms.Button cmd_ClearLog;
    private System.Windows.Forms.CheckBox ckb_ShowPing;
    private System.Windows.Forms.CheckBox ckb_ShowBlink;
  }
}