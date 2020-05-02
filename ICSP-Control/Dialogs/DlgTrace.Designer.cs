namespace ICSPControl.Dialogs
{
  partial class DlgTrace
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgTrace));
      this.cmd_StartStopTrace = new System.Windows.Forms.Button();
      this.txt_Log = new System.Windows.Forms.TextBox();
      this.cmd_ClearLog = new System.Windows.Forms.Button();
      this.ckb_PingMessage = new System.Windows.Forms.CheckBox();
      this.grp_EventFilter = new System.Windows.Forms.GroupBox();
      this.ckb_Others = new System.Windows.Forms.CheckBox();
      this.ckb_LevelEvent = new System.Windows.Forms.CheckBox();
      this.ckb_CommandEvent = new System.Windows.Forms.CheckBox();
      this.ckb_StringEvent = new System.Windows.Forms.CheckBox();
      this.ckb_DynamicDeviceCreated = new System.Windows.Forms.CheckBox();
      this.ckb_DeviceInfo = new System.Windows.Forms.CheckBox();
      this.ckb_PortCount = new System.Windows.Forms.CheckBox();
      this.ckb_ChannelEvent = new System.Windows.Forms.CheckBox();
      this.ckb_BlinkMessage = new System.Windows.Forms.CheckBox();
      this.grp_EventFilter.SuspendLayout();
      this.SuspendLayout();
      // 
      // cmd_StartStopTrace
      // 
      this.cmd_StartStopTrace.Location = new System.Drawing.Point(18, 18);
      this.cmd_StartStopTrace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_StartStopTrace.Name = "cmd_StartStopTrace";
      this.cmd_StartStopTrace.Size = new System.Drawing.Size(174, 35);
      this.cmd_StartStopTrace.TabIndex = 0;
      this.cmd_StartStopTrace.Text = "Stop Trace";
      this.cmd_StartStopTrace.UseVisualStyleBackColor = true;
      // 
      // txt_Log
      // 
      this.txt_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_Log.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_Log.Location = new System.Drawing.Point(398, 18);
      this.txt_Log.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_Log.Multiline = true;
      this.txt_Log.Name = "txt_Log";
      this.txt_Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txt_Log.Size = new System.Drawing.Size(1478, 985);
      this.txt_Log.TabIndex = 3;
      this.txt_Log.WordWrap = false;
      // 
      // cmd_ClearLog
      // 
      this.cmd_ClearLog.Location = new System.Drawing.Point(201, 18);
      this.cmd_ClearLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ClearLog.Name = "cmd_ClearLog";
      this.cmd_ClearLog.Size = new System.Drawing.Size(174, 35);
      this.cmd_ClearLog.TabIndex = 1;
      this.cmd_ClearLog.Text = "Clear Log";
      this.cmd_ClearLog.UseVisualStyleBackColor = true;
      // 
      // ckb_PingMessage
      // 
      this.ckb_PingMessage.Location = new System.Drawing.Point(17, 34);
      this.ckb_PingMessage.Name = "ckb_PingMessage";
      this.ckb_PingMessage.Size = new System.Drawing.Size(300, 29);
      this.ckb_PingMessage.TabIndex = 0;
      this.ckb_PingMessage.Text = "Ping Message";
      this.ckb_PingMessage.UseVisualStyleBackColor = true;
      // 
      // grp_EventFilter
      // 
      this.grp_EventFilter.Controls.Add(this.ckb_Others);
      this.grp_EventFilter.Controls.Add(this.ckb_LevelEvent);
      this.grp_EventFilter.Controls.Add(this.ckb_CommandEvent);
      this.grp_EventFilter.Controls.Add(this.ckb_StringEvent);
      this.grp_EventFilter.Controls.Add(this.ckb_DynamicDeviceCreated);
      this.grp_EventFilter.Controls.Add(this.ckb_DeviceInfo);
      this.grp_EventFilter.Controls.Add(this.ckb_PortCount);
      this.grp_EventFilter.Controls.Add(this.ckb_ChannelEvent);
      this.grp_EventFilter.Controls.Add(this.ckb_BlinkMessage);
      this.grp_EventFilter.Controls.Add(this.ckb_PingMessage);
      this.grp_EventFilter.Location = new System.Drawing.Point(18, 81);
      this.grp_EventFilter.Name = "grp_EventFilter";
      this.grp_EventFilter.Size = new System.Drawing.Size(352, 396);
      this.grp_EventFilter.TabIndex = 2;
      this.grp_EventFilter.TabStop = false;
      this.grp_EventFilter.Text = "Event Filter";
      // 
      // ckb_Others
      // 
      this.ckb_Others.Checked = true;
      this.ckb_Others.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_Others.Location = new System.Drawing.Point(17, 349);
      this.ckb_Others.Name = "ckb_Others";
      this.ckb_Others.Size = new System.Drawing.Size(300, 29);
      this.ckb_Others.TabIndex = 9;
      this.ckb_Others.Text = "Others";
      this.ckb_Others.UseVisualStyleBackColor = true;
      // 
      // ckb_LevelEvent
      // 
      this.ckb_LevelEvent.Checked = true;
      this.ckb_LevelEvent.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_LevelEvent.Location = new System.Drawing.Point(17, 314);
      this.ckb_LevelEvent.Name = "ckb_LevelEvent";
      this.ckb_LevelEvent.Size = new System.Drawing.Size(300, 29);
      this.ckb_LevelEvent.TabIndex = 8;
      this.ckb_LevelEvent.Text = "Level Event";
      this.ckb_LevelEvent.UseVisualStyleBackColor = true;
      // 
      // ckb_CommandEvent
      // 
      this.ckb_CommandEvent.Location = new System.Drawing.Point(17, 279);
      this.ckb_CommandEvent.Name = "ckb_CommandEvent";
      this.ckb_CommandEvent.Size = new System.Drawing.Size(300, 29);
      this.ckb_CommandEvent.TabIndex = 7;
      this.ckb_CommandEvent.Text = "Command Event";
      this.ckb_CommandEvent.UseVisualStyleBackColor = true;
      // 
      // ckb_StringEvent
      // 
      this.ckb_StringEvent.Checked = true;
      this.ckb_StringEvent.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_StringEvent.Location = new System.Drawing.Point(17, 244);
      this.ckb_StringEvent.Name = "ckb_StringEvent";
      this.ckb_StringEvent.Size = new System.Drawing.Size(300, 29);
      this.ckb_StringEvent.TabIndex = 6;
      this.ckb_StringEvent.Text = "String Event";
      this.ckb_StringEvent.UseVisualStyleBackColor = true;
      // 
      // ckb_DynamicDeviceCreated
      // 
      this.ckb_DynamicDeviceCreated.Checked = true;
      this.ckb_DynamicDeviceCreated.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_DynamicDeviceCreated.Location = new System.Drawing.Point(17, 174);
      this.ckb_DynamicDeviceCreated.Name = "ckb_DynamicDeviceCreated";
      this.ckb_DynamicDeviceCreated.Size = new System.Drawing.Size(300, 29);
      this.ckb_DynamicDeviceCreated.TabIndex = 4;
      this.ckb_DynamicDeviceCreated.Text = "Dynamic Device Created";
      this.ckb_DynamicDeviceCreated.UseVisualStyleBackColor = true;
      // 
      // ckb_DeviceInfo
      // 
      this.ckb_DeviceInfo.Checked = true;
      this.ckb_DeviceInfo.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_DeviceInfo.Location = new System.Drawing.Point(17, 104);
      this.ckb_DeviceInfo.Name = "ckb_DeviceInfo";
      this.ckb_DeviceInfo.Size = new System.Drawing.Size(300, 29);
      this.ckb_DeviceInfo.TabIndex = 2;
      this.ckb_DeviceInfo.Text = "DeviceInfo";
      this.ckb_DeviceInfo.UseVisualStyleBackColor = true;
      // 
      // ckb_PortCount
      // 
      this.ckb_PortCount.Checked = true;
      this.ckb_PortCount.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_PortCount.Location = new System.Drawing.Point(17, 139);
      this.ckb_PortCount.Name = "ckb_PortCount";
      this.ckb_PortCount.Size = new System.Drawing.Size(300, 29);
      this.ckb_PortCount.TabIndex = 3;
      this.ckb_PortCount.Text = "Port Count";
      this.ckb_PortCount.UseVisualStyleBackColor = true;
      // 
      // ckb_ChannelEvent
      // 
      this.ckb_ChannelEvent.Checked = true;
      this.ckb_ChannelEvent.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_ChannelEvent.Location = new System.Drawing.Point(17, 209);
      this.ckb_ChannelEvent.Name = "ckb_ChannelEvent";
      this.ckb_ChannelEvent.Size = new System.Drawing.Size(300, 29);
      this.ckb_ChannelEvent.TabIndex = 5;
      this.ckb_ChannelEvent.Text = "Channel Event";
      this.ckb_ChannelEvent.UseVisualStyleBackColor = true;
      // 
      // ckb_BlinkMessage
      // 
      this.ckb_BlinkMessage.Location = new System.Drawing.Point(17, 69);
      this.ckb_BlinkMessage.Name = "ckb_BlinkMessage";
      this.ckb_BlinkMessage.Size = new System.Drawing.Size(300, 29);
      this.ckb_BlinkMessage.TabIndex = 1;
      this.ckb_BlinkMessage.Text = "Blink Message";
      this.ckb_BlinkMessage.UseVisualStyleBackColor = true;
      // 
      // DlgTrace
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1898, 1024);
      this.Controls.Add(this.grp_EventFilter);
      this.Controls.Add(this.cmd_ClearLog);
      this.Controls.Add(this.cmd_StartStopTrace);
      this.Controls.Add(this.txt_Log);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "DlgTrace";
      this.Text = "ICSP Trace Window";
      this.grp_EventFilter.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button cmd_StartStopTrace;
    private System.Windows.Forms.TextBox txt_Log;
    private System.Windows.Forms.Button cmd_ClearLog;
    private System.Windows.Forms.CheckBox ckb_PingMessage;
    private System.Windows.Forms.GroupBox grp_EventFilter;
    private System.Windows.Forms.CheckBox ckb_DynamicDeviceCreated;
    private System.Windows.Forms.CheckBox ckb_DeviceInfo;
    private System.Windows.Forms.CheckBox ckb_PortCount;
    private System.Windows.Forms.CheckBox ckb_ChannelEvent;
    private System.Windows.Forms.CheckBox ckb_BlinkMessage;
    private System.Windows.Forms.CheckBox ckb_LevelEvent;
    private System.Windows.Forms.CheckBox ckb_CommandEvent;
    private System.Windows.Forms.CheckBox ckb_StringEvent;
    private System.Windows.Forms.CheckBox ckb_Others;
  }
}