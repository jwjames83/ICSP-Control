namespace ICSPControl.Dialogs
{
  partial class DlgMain
  {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgMain));
      this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
      this.tssl_ClientState = new System.Windows.Forms.ToolStripStatusLabel();
      this.tssl_Host = new System.Windows.Forms.ToolStripStatusLabel();
      this.tssl_Port = new System.Windows.Forms.ToolStripStatusLabel();
      this.tssl_CurrentSystem = new System.Windows.Forms.ToolStripStatusLabel();
      this.tssl_DynamicDevice = new System.Windows.Forms.ToolStripStatusLabel();
      this.tssl_Device = new System.Windows.Forms.ToolStripStatusLabel();
      this.tssl_ProgramName = new System.Windows.Forms.ToolStripStatusLabel();
      this.tssl_MainFile = new System.Windows.Forms.ToolStripStatusLabel();
      this.tssl_Blink = new System.Windows.Forms.ToolStripStatusLabel();
      this.MainMenu = new System.Windows.Forms.MenuStrip();
      this.tsmi_File = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_File_Connect = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_File_Disconnect = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_File_CommunicationSetttings = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.tsmi_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_Tools = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_Tools_FileTransfer = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_Tools_ControlDevice = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_Tools_DeviceNotifications = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      this.tsmi_Tools_InfoFileTransfer = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_Tools_OpenTmpFolder = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_Window = new System.Windows.Forms.ToolStripMenuItem();
      this.MainToolStrip = new System.Windows.Forms.ToolStrip();
      this.tsb_Connect = new System.Windows.Forms.ToolStripButton();
      this.tsb_Disconnect = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.tsb_FileTransfer = new System.Windows.Forms.ToolStripButton();
      this.tsb_ControlDevice = new System.Windows.Forms.ToolStripButton();
      this.tsb_DeviceNotifications = new System.Windows.Forms.ToolStripButton();
      this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
      this.MainStatusStrip.SuspendLayout();
      this.MainMenu.SuspendLayout();
      this.MainToolStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // MainStatusStrip
      // 
      this.MainStatusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssl_ClientState,
            this.tssl_Host,
            this.tssl_Port,
            this.tssl_CurrentSystem,
            this.tssl_DynamicDevice,
            this.tssl_Device,
            this.tssl_ProgramName,
            this.tssl_MainFile,
            this.tssl_Blink});
      this.MainStatusStrip.Location = new System.Drawing.Point(0, 961);
      this.MainStatusStrip.Name = "MainStatusStrip";
      this.MainStatusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
      this.MainStatusStrip.Size = new System.Drawing.Size(1725, 36);
      this.MainStatusStrip.SizingGrip = false;
      this.MainStatusStrip.TabIndex = 3;
      // 
      // tssl_ClientState
      // 
      this.tssl_ClientState.AutoSize = false;
      this.tssl_ClientState.BackColor = System.Drawing.Color.Red;
      this.tssl_ClientState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.tssl_ClientState.Name = "tssl_ClientState";
      this.tssl_ClientState.Size = new System.Drawing.Size(140, 29);
      this.tssl_ClientState.Text = "Not Connected";
      this.tssl_ClientState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tssl_Host
      // 
      this.tssl_Host.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.tssl_Host.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.tssl_Host.Name = "tssl_Host";
      this.tssl_Host.Size = new System.Drawing.Size(195, 29);
      this.tssl_Host.Text = "Host: 192.168.200.255";
      this.tssl_Host.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tssl_Port
      // 
      this.tssl_Port.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.tssl_Port.Name = "tssl_Port";
      this.tssl_Port.Size = new System.Drawing.Size(97, 29);
      this.tssl_Port.Text = "Port: 1319";
      this.tssl_Port.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tssl_CurrentSystem
      // 
      this.tssl_CurrentSystem.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.tssl_CurrentSystem.Name = "tssl_CurrentSystem";
      this.tssl_CurrentSystem.Size = new System.Drawing.Size(155, 29);
      this.tssl_CurrentSystem.Text = "Current System: 0";
      this.tssl_CurrentSystem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tssl_DynamicDevice
      // 
      this.tssl_DynamicDevice.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.tssl_DynamicDevice.Name = "tssl_DynamicDevice";
      this.tssl_DynamicDevice.Size = new System.Drawing.Size(201, 29);
      this.tssl_DynamicDevice.Text = "Dynamic Device: 00000";
      this.tssl_DynamicDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tssl_Device
      // 
      this.tssl_Device.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.tssl_Device.Name = "tssl_Device";
      this.tssl_Device.Size = new System.Drawing.Size(194, 29);
      this.tssl_Device.Text = "Physical Device: 15000";
      this.tssl_Device.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tssl_ProgramName
      // 
      this.tssl_ProgramName.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.tssl_ProgramName.Name = "tssl_ProgramName";
      this.tssl_ProgramName.Size = new System.Drawing.Size(146, 29);
      this.tssl_ProgramName.Text = "Program Name: ";
      this.tssl_ProgramName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tssl_MainFile
      // 
      this.tssl_MainFile.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.tssl_MainFile.Name = "tssl_MainFile";
      this.tssl_MainFile.Size = new System.Drawing.Size(543, 29);
      this.tssl_MainFile.Spring = true;
      this.tssl_MainFile.Text = "Main File: ";
      this.tssl_MainFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tssl_Blink
      // 
      this.tssl_Blink.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.tssl_Blink.Name = "tssl_Blink";
      this.tssl_Blink.Size = new System.Drawing.Size(31, 29);
      this.tssl_Blink.Text = "   ";
      // 
      // MainMenu
      // 
      this.MainMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.MainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_File,
            this.tsmi_Tools,
            this.tsmi_Window});
      this.MainMenu.Location = new System.Drawing.Point(0, 0);
      this.MainMenu.MdiWindowListItem = this.tsmi_Window;
      this.MainMenu.Name = "MainMenu";
      this.MainMenu.Size = new System.Drawing.Size(1725, 33);
      this.MainMenu.TabIndex = 0;
      // 
      // tsmi_File
      // 
      this.tsmi_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_File_Connect,
            this.tsmi_File_Disconnect,
            this.tsmi_File_CommunicationSetttings,
            this.toolStripMenuItem1,
            this.tsmi_File_Exit});
      this.tsmi_File.Name = "tsmi_File";
      this.tsmi_File.Size = new System.Drawing.Size(54, 29);
      this.tsmi_File.Text = "&File";
      // 
      // tsmi_File_Connect
      // 
      this.tsmi_File_Connect.Image = global::ICSP.Control.Properties.Resources.TpDesignConnect;
      this.tsmi_File_Connect.Name = "tsmi_File_Connect";
      this.tsmi_File_Connect.Size = new System.Drawing.Size(321, 34);
      this.tsmi_File_Connect.Text = "Connect";
      // 
      // tsmi_File_Disconnect
      // 
      this.tsmi_File_Disconnect.Enabled = false;
      this.tsmi_File_Disconnect.Image = global::ICSP.Control.Properties.Resources.TpDesignDisconnect;
      this.tsmi_File_Disconnect.Name = "tsmi_File_Disconnect";
      this.tsmi_File_Disconnect.Size = new System.Drawing.Size(321, 34);
      this.tsmi_File_Disconnect.Text = "Disconnect";
      // 
      // tsmi_File_CommunicationSetttings
      // 
      this.tsmi_File_CommunicationSetttings.Name = "tsmi_File_CommunicationSetttings";
      this.tsmi_File_CommunicationSetttings.Size = new System.Drawing.Size(321, 34);
      this.tsmi_File_CommunicationSetttings.Text = "&Communication Settings...";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(318, 6);
      // 
      // tsmi_File_Exit
      // 
      this.tsmi_File_Exit.Image = global::ICSP.Control.Properties.Resources.Exit;
      this.tsmi_File_Exit.Name = "tsmi_File_Exit";
      this.tsmi_File_Exit.Size = new System.Drawing.Size(321, 34);
      this.tsmi_File_Exit.Text = "&Exit";
      // 
      // tsmi_Tools
      // 
      this.tsmi_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Tools_FileTransfer,
            this.tsmi_Tools_ControlDevice,
            this.tsmi_Tools_DeviceNotifications,
            this.toolStripMenuItem3,
            this.tsmi_Tools_InfoFileTransfer,
            this.tsmi_Tools_OpenTmpFolder});
      this.tsmi_Tools.Name = "tsmi_Tools";
      this.tsmi_Tools.Size = new System.Drawing.Size(69, 29);
      this.tsmi_Tools.Text = "&Tools";
      // 
      // tsmi_Tools_FileTransfer
      // 
      this.tsmi_Tools_FileTransfer.Image = global::ICSP.Control.Properties.Resources.TpDesignSendToPanel;
      this.tsmi_Tools_FileTransfer.Name = "tsmi_Tools_FileTransfer";
      this.tsmi_Tools_FileTransfer.Size = new System.Drawing.Size(288, 34);
      this.tsmi_Tools_FileTransfer.Text = "&FileTransfer";
      // 
      // tsmi_Tools_ControlDevice
      // 
      this.tsmi_Tools_ControlDevice.Image = global::ICSP.Control.Properties.Resources.NetLinxControlDevice;
      this.tsmi_Tools_ControlDevice.Name = "tsmi_Tools_ControlDevice";
      this.tsmi_Tools_ControlDevice.Size = new System.Drawing.Size(288, 34);
      this.tsmi_Tools_ControlDevice.Text = "&Control a Device";
      // 
      // tsmi_Tools_DeviceNotifications
      // 
      this.tsmi_Tools_DeviceNotifications.Image = global::ICSP.Control.Properties.Resources.NetLinxDeviceNotifications;
      this.tsmi_Tools_DeviceNotifications.Name = "tsmi_Tools_DeviceNotifications";
      this.tsmi_Tools_DeviceNotifications.Size = new System.Drawing.Size(288, 34);
      this.tsmi_Tools_DeviceNotifications.Text = "&Device Notifications";
      // 
      // toolStripMenuItem3
      // 
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(285, 6);
      // 
      // tsmi_Tools_InfoFileTransfer
      // 
      this.tsmi_Tools_InfoFileTransfer.Image = global::ICSP.Control.Properties.Resources.Info;
      this.tsmi_Tools_InfoFileTransfer.Name = "tsmi_Tools_InfoFileTransfer";
      this.tsmi_Tools_InfoFileTransfer.Size = new System.Drawing.Size(288, 34);
      this.tsmi_Tools_InfoFileTransfer.Text = "&Info File Transfer ...";
      // 
      // tsmi_Tools_OpenTmpFolder
      // 
      this.tsmi_Tools_OpenTmpFolder.Image = global::ICSP.Control.Properties.Resources.OpenFolder;
      this.tsmi_Tools_OpenTmpFolder.Name = "tsmi_Tools_OpenTmpFolder";
      this.tsmi_Tools_OpenTmpFolder.Size = new System.Drawing.Size(288, 34);
      this.tsmi_Tools_OpenTmpFolder.Text = "&Open G5 Temp Folder";
      // 
      // tsmi_Window
      // 
      this.tsmi_Window.Name = "tsmi_Window";
      this.tsmi_Window.Size = new System.Drawing.Size(94, 29);
      this.tsmi_Window.Text = "&Window";
      // 
      // MainToolStrip
      // 
      this.MainToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_Connect,
            this.tsb_Disconnect,
            this.toolStripSeparator1,
            this.tsb_FileTransfer,
            this.tsb_ControlDevice,
            this.tsb_DeviceNotifications});
      this.MainToolStrip.Location = new System.Drawing.Point(0, 33);
      this.MainToolStrip.Name = "MainToolStrip";
      this.MainToolStrip.Size = new System.Drawing.Size(1725, 34);
      this.MainToolStrip.TabIndex = 1;
      // 
      // tsb_Connect
      // 
      this.tsb_Connect.Image = global::ICSP.Control.Properties.Resources.TpDesignConnect;
      this.tsb_Connect.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsb_Connect.Name = "tsb_Connect";
      this.tsb_Connect.Size = new System.Drawing.Size(105, 29);
      this.tsb_Connect.Text = "Connect";
      // 
      // tsb_Disconnect
      // 
      this.tsb_Disconnect.Enabled = false;
      this.tsb_Disconnect.Image = global::ICSP.Control.Properties.Resources.TpDesignDisconnect;
      this.tsb_Disconnect.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsb_Disconnect.Name = "tsb_Disconnect";
      this.tsb_Disconnect.Size = new System.Drawing.Size(127, 29);
      this.tsb_Disconnect.Text = "Disconnect";
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 34);
      // 
      // tsb_FileTransfer
      // 
      this.tsb_FileTransfer.Image = global::ICSP.Control.Properties.Resources.TpDesignSendToPanel;
      this.tsb_FileTransfer.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsb_FileTransfer.Name = "tsb_FileTransfer";
      this.tsb_FileTransfer.Size = new System.Drawing.Size(127, 29);
      this.tsb_FileTransfer.Text = "FileTransfer";
      // 
      // tsb_ControlDevice
      // 
      this.tsb_ControlDevice.Image = global::ICSP.Control.Properties.Resources.NetLinxControlDevice;
      this.tsb_ControlDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsb_ControlDevice.Name = "tsb_ControlDevice";
      this.tsb_ControlDevice.Size = new System.Drawing.Size(170, 29);
      this.tsb_ControlDevice.Text = "Control a Device";
      // 
      // tsb_DeviceNotifications
      // 
      this.tsb_DeviceNotifications.Image = global::ICSP.Control.Properties.Resources.NetLinxDeviceNotifications;
      this.tsb_DeviceNotifications.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsb_DeviceNotifications.Name = "tsb_DeviceNotifications";
      this.tsb_DeviceNotifications.Size = new System.Drawing.Size(197, 29);
      this.tsb_DeviceNotifications.Text = "Device Notifications";
      // 
      // DockPanel
      // 
      this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.DockPanel.DockBackColor = System.Drawing.SystemColors.AppWorkspace;
      this.DockPanel.Location = new System.Drawing.Point(0, 67);
      this.DockPanel.Name = "DockPanel";
      this.DockPanel.Size = new System.Drawing.Size(1725, 894);
      this.DockPanel.TabIndex = 2;
      // 
      // DlgMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1725, 997);
      this.Controls.Add(this.DockPanel);
      this.Controls.Add(this.MainToolStrip);
      this.Controls.Add(this.MainStatusStrip);
      this.Controls.Add(this.MainMenu);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.IsMdiContainer = true;
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MinimumSize = new System.Drawing.Size(1534, 624);
      this.Name = "DlgMain";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "ICSP-Control";
      this.MainStatusStrip.ResumeLayout(false);
      this.MainStatusStrip.PerformLayout();
      this.MainMenu.ResumeLayout(false);
      this.MainMenu.PerformLayout();
      this.MainToolStrip.ResumeLayout(false);
      this.MainToolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

    private System.Windows.Forms.StatusStrip MainStatusStrip;
    private System.Windows.Forms.ToolStripStatusLabel tssl_Host;
    private System.Windows.Forms.ToolStripStatusLabel tssl_Port;
    private System.Windows.Forms.ToolStripStatusLabel tssl_ClientState;
    private System.Windows.Forms.ToolStripStatusLabel tssl_Blink;
    private System.Windows.Forms.MenuStrip MainMenu;
    private System.Windows.Forms.ToolStripMenuItem tsmi_File;
    private System.Windows.Forms.ToolStripMenuItem tsmi_File_CommunicationSetttings;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem tsmi_File_Exit;
    private System.Windows.Forms.ToolStripStatusLabel tssl_Device;
    private System.Windows.Forms.ToolStripStatusLabel tssl_CurrentSystem;
    private System.Windows.Forms.ToolStripStatusLabel tssl_DynamicDevice;
    private System.Windows.Forms.ToolStripStatusLabel tssl_ProgramName;
    private System.Windows.Forms.ToolStripStatusLabel tssl_MainFile;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools_InfoFileTransfer;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools_OpenTmpFolder;
        private System.Windows.Forms.ToolStrip MainToolStrip;
        private System.Windows.Forms.ToolStripButton tsb_ControlDevice;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Window;
        private WeifenLuo.WinFormsUI.Docking.DockPanel DockPanel;
        private System.Windows.Forms.ToolStripButton tsb_DeviceNotifications;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools_FileTransfer;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools_ControlDevice;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools_DeviceNotifications;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripButton tsb_FileTransfer;
        private System.Windows.Forms.ToolStripButton tsb_Connect;
        private System.Windows.Forms.ToolStripButton tsb_Disconnect;
        private System.Windows.Forms.ToolStripMenuItem tsmi_File_Connect;
        private System.Windows.Forms.ToolStripMenuItem tsmi_File_Disconnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

