namespace ICSPControl.Dialogs
{
  partial class DlgTest
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
      TpControls.Channel channel7 = new TpControls.Channel();
      TpControls.Channel channel8 = new TpControls.Channel();
      TpControls.Channel channel9 = new TpControls.Channel();
      TpControls.Channel channel10 = new TpControls.Channel();
      TpControls.Channel channel11 = new TpControls.Channel();
      TpControls.Channel channel12 = new TpControls.Channel();
      this.cmd_Close = new System.Windows.Forms.Button();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.tpButton1 = new TpControls.TpButton();
      this.tpButton2 = new TpControls.TpButton();
      this.label6 = new System.Windows.Forms.Label();
      this.tpButton3 = new TpControls.TpButton();
      this.label5 = new System.Windows.Forms.Label();
      this.tpButton4 = new TpControls.TpButton();
      this.tpb_Feedback_2 = new TpControls.TpButton();
      this.tpb_Feedback_1 = new TpControls.TpButton();
      this.groupBox5.SuspendLayout();
      this.SuspendLayout();
      // 
      // cmd_Close
      // 
      this.cmd_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cmd_Close.Location = new System.Drawing.Point(200, 122);
      this.cmd_Close.Name = "cmd_Close";
      this.cmd_Close.Size = new System.Drawing.Size(113, 23);
      this.cmd_Close.TabIndex = 2;
      this.cmd_Close.Text = "Close";
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.label8);
      this.groupBox5.Controls.Add(this.label7);
      this.groupBox5.Controls.Add(this.tpButton1);
      this.groupBox5.Controls.Add(this.tpButton2);
      this.groupBox5.Controls.Add(this.label6);
      this.groupBox5.Controls.Add(this.tpButton3);
      this.groupBox5.Controls.Add(this.label5);
      this.groupBox5.Controls.Add(this.tpButton4);
      this.groupBox5.Controls.Add(this.tpb_Feedback_2);
      this.groupBox5.Controls.Add(this.tpb_Feedback_1);
      this.groupBox5.Location = new System.Drawing.Point(12, 12);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(300, 104);
      this.groupBox5.TabIndex = 0;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Feedback Test (15000)";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(145, 21);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(35, 13);
      this.label8.TabIndex = 1;
      this.label8.Text = "Port 2";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(74, 21);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(35, 13);
      this.label7.TabIndex = 0;
      this.label7.Text = "Port 1";
      // 
      // tpButton1
      // 
      channel7.ChannelCode = 2;
      channel7.ChannelPort = 2;
      channel7.Feedback = TpControls.FeedbackType.Channel;
      this.tpButton1.Channel = channel7;
      this.tpButton1.Location = new System.Drawing.Point(149, 68);
      this.tpButton1.Margin = new System.Windows.Forms.Padding(2);
      this.tpButton1.Name = "tpButton1";
      this.tpButton1.Size = new System.Drawing.Size(67, 24);
      this.tpButton1.State = TpControls.StateType.On;
      this.tpButton1.StateOff.Text = "Disabled";
      this.tpButton1.StateOn.BitmapJustification = System.Drawing.ContentAlignment.BottomRight;
      this.tpButton1.StateOn.BorderColor = System.Drawing.Color.Empty;
      this.tpButton1.StateOn.FillColor = System.Drawing.Color.DarkOrange;
      this.tpButton1.StateOn.Text = "Enabled";
      this.tpButton1.StateOn.TextColor = System.Drawing.Color.Yellow;
      this.tpButton1.StateOn.TextJustification = null;
      this.tpButton1.TabIndex = 8;
      // 
      // tpButton2
      // 
      channel8.ChannelCode = 1;
      channel8.ChannelPort = 2;
      channel8.Feedback = TpControls.FeedbackType.Channel;
      this.tpButton2.Channel = channel8;
      this.tpButton2.Location = new System.Drawing.Point(149, 38);
      this.tpButton2.Margin = new System.Windows.Forms.Padding(2);
      this.tpButton2.Name = "tpButton2";
      this.tpButton2.Size = new System.Drawing.Size(67, 24);
      this.tpButton2.State = TpControls.StateType.On;
      this.tpButton2.StateOff.Text = "Disabled";
      this.tpButton2.StateOn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.tpButton2.StateOn.FillColor = System.Drawing.Color.Red;
      this.tpButton2.StateOn.Text = "Enabled";
      this.tpButton2.StateOn.TextColor = System.Drawing.Color.Yellow;
      this.tpButton2.TabIndex = 4;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 73);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(55, 13);
      this.label6.TabIndex = 6;
      this.label6.Text = "Channel 2";
      // 
      // tpButton3
      // 
      channel9.ChannelCode = 2;
      channel9.ChannelPort = 2;
      channel9.Feedback = TpControls.FeedbackType.AllwaysOn;
      this.tpButton3.Channel = channel9;
      this.tpButton3.Location = new System.Drawing.Point(221, 68);
      this.tpButton3.Margin = new System.Windows.Forms.Padding(2);
      this.tpButton3.Name = "tpButton3";
      this.tpButton3.Size = new System.Drawing.Size(67, 24);
      this.tpButton3.StateOff.Text = "Off";
      this.tpButton3.StateOn.BitmapJustification = System.Drawing.ContentAlignment.BottomRight;
      this.tpButton3.StateOn.FillColor = System.Drawing.Color.DarkOrange;
      this.tpButton3.StateOn.Text = "AllwaysOn";
      this.tpButton3.StateOn.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
      this.tpButton3.StateOn.TextJustification = null;
      this.tpButton3.TabIndex = 9;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 45);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(55, 13);
      this.label5.TabIndex = 2;
      this.label5.Text = "Channel 1";
      // 
      // tpButton4
      // 
      channel10.ChannelCode = 1;
      channel10.ChannelPort = 2;
      channel10.Feedback = TpControls.FeedbackType.Momentary;
      this.tpButton4.Channel = channel10;
      this.tpButton4.Location = new System.Drawing.Point(221, 38);
      this.tpButton4.Margin = new System.Windows.Forms.Padding(2);
      this.tpButton4.Name = "tpButton4";
      this.tpButton4.Size = new System.Drawing.Size(67, 24);
      this.tpButton4.State = TpControls.StateType.On;
      this.tpButton4.StateOff.Text = "Momentary";
      this.tpButton4.StateOn.FillColor = System.Drawing.Color.Gold;
      this.tpButton4.StateOn.Text = "Momentary";
      this.tpButton4.StateOn.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
      this.tpButton4.TabIndex = 5;
      // 
      // tpb_Feedback_2
      // 
      channel11.ChannelCode = 2;
      channel11.Feedback = TpControls.FeedbackType.Channel;
      this.tpb_Feedback_2.Channel = channel11;
      this.tpb_Feedback_2.Location = new System.Drawing.Point(77, 68);
      this.tpb_Feedback_2.Margin = new System.Windows.Forms.Padding(2);
      this.tpb_Feedback_2.Name = "tpb_Feedback_2";
      this.tpb_Feedback_2.Size = new System.Drawing.Size(67, 24);
      this.tpb_Feedback_2.State = TpControls.StateType.On;
      this.tpb_Feedback_2.StateOff.Text = "Off";
      this.tpb_Feedback_2.StateOn.BorderColor = System.Drawing.Color.Empty;
      this.tpb_Feedback_2.StateOn.FillColor = System.Drawing.Color.DarkOrange;
      this.tpb_Feedback_2.StateOn.Text = "On";
      this.tpb_Feedback_2.StateOn.TextColor = System.Drawing.Color.Empty;
      this.tpb_Feedback_2.TabIndex = 7;
      // 
      // tpb_Feedback_1
      // 
      channel12.ChannelCode = 1;
      channel12.Feedback = TpControls.FeedbackType.Channel;
      this.tpb_Feedback_1.Channel = channel12;
      this.tpb_Feedback_1.Location = new System.Drawing.Point(77, 38);
      this.tpb_Feedback_1.Margin = new System.Windows.Forms.Padding(2);
      this.tpb_Feedback_1.Name = "tpb_Feedback_1";
      this.tpb_Feedback_1.Size = new System.Drawing.Size(67, 24);
      this.tpb_Feedback_1.State = TpControls.StateType.On;
      this.tpb_Feedback_1.StateOff.Text = "Off";
      this.tpb_Feedback_1.StateOn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.tpb_Feedback_1.StateOn.FillColor = System.Drawing.Color.Red;
      this.tpb_Feedback_1.StateOn.Text = "On";
      this.tpb_Feedback_1.StateOn.TextColor = System.Drawing.Color.Empty;
      this.tpb_Feedback_1.TabIndex = 3;
      // 
      // DlgTest
      // 
      this.AcceptButton = this.cmd_Close;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cmd_Close;
      this.ClientSize = new System.Drawing.Size(325, 154);
      this.Controls.Add(this.groupBox5);
      this.Controls.Add(this.cmd_Close);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DlgTest";
      this.Text = "Feedback Test";
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button cmd_Close;
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label7;
    private TpControls.TpButton tpButton1;
    private TpControls.TpButton tpButton2;
    private System.Windows.Forms.Label label6;
    private TpControls.TpButton tpButton3;
    private System.Windows.Forms.Label label5;
    private TpControls.TpButton tpButton4;
    private TpControls.TpButton tpb_Feedback_2;
    private TpControls.TpButton tpb_Feedback_1;
  }
}