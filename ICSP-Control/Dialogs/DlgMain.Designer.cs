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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Knoten4", 2, 3);
      System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Knoten5", 2, 3);
      System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Knoten6", 4, 5);
      System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Knoten1", 0, 1, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
      System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Knoten2", 0, 1);
      System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Knoten3", 6, 7);
      System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("<Empty Device Tree>", 0, 1, new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6});
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
      this.cmd_ChannelOn = new System.Windows.Forms.Button();
      this.cmd_Disconnect = new System.Windows.Forms.Button();
      this.cmd_Connect = new System.Windows.Forms.Button();
      this.cmd_ChannelOff = new System.Windows.Forms.Button();
      this.cmd_SendCmd = new System.Windows.Forms.Button();
      this.cmd_SendString = new System.Windows.Forms.Button();
      this.txt_Text = new System.Windows.Forms.TextBox();
      this.lab_System = new System.Windows.Forms.Label();
      this.num_System = new System.Windows.Forms.NumericUpDown();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.lab_DevPort = new System.Windows.Forms.Label();
      this.lab_Device = new System.Windows.Forms.Label();
      this.num_Device = new System.Windows.Forms.NumericUpDown();
      this.num_DevPort = new System.Windows.Forms.NumericUpDown();
      this.cmd_RequestDeviceStatus = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.lab_Channel = new System.Windows.Forms.Label();
      this.num_Channel = new System.Windows.Forms.NumericUpDown();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.lab_LevelValue = new System.Windows.Forms.Label();
      this.num_LevelInput = new System.Windows.Forms.NumericUpDown();
      this.lab_LevelInput = new System.Windows.Forms.Label();
      this.num_LevelValue = new System.Windows.Forms.NumericUpDown();
      this.cmd_SendLevel = new System.Windows.Forms.Button();
      this.MainMenu = new System.Windows.Forms.MenuStrip();
      this.tsmi_Settings = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_CommunicationSetttings = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.tsmi_Exit = new System.Windows.Forms.ToolStripMenuItem();
      this.OnlineTree = new System.Windows.Forms.TreeView();
      this.ImageList = new System.Windows.Forms.ImageList(this.components);
      this.cm_OnlineTree = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.cmd_RefreshSystemOnlineTree = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.cmd_ShowDeviceProperties = new System.Windows.Forms.ToolStripMenuItem();
      this.label9 = new System.Windows.Forms.Label();
      this.cmd_ShowTraceWindow = new System.Windows.Forms.Button();
      this.cmd_CreatePhysicalDevice = new System.Windows.Forms.Button();
      this.cmd_ShowFeedbackTest = new System.Windows.Forms.Button();
      this.tsmi_Tools = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_Tools_OpenTmpFolder = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_Tools_InfoFileTransfer = new System.Windows.Forms.ToolStripMenuItem();
      this.MainStatusStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_System)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_Device)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).BeginInit();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_Channel)).BeginInit();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelValue)).BeginInit();
      this.MainMenu.SuspendLayout();
      this.cm_OnlineTree.SuspendLayout();
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
      this.MainStatusStrip.Location = new System.Drawing.Point(0, 558);
      this.MainStatusStrip.Name = "MainStatusStrip";
      this.MainStatusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
      this.MainStatusStrip.Size = new System.Drawing.Size(1521, 36);
      this.MainStatusStrip.SizingGrip = false;
      this.MainStatusStrip.TabIndex = 16;
      // 
      // tssl_ClientState
      // 
      this.tssl_ClientState.AutoSize = false;
      this.tssl_ClientState.BackColor = System.Drawing.Color.Red;
      this.tssl_ClientState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.tssl_ClientState.Name = "tssl_ClientState";
      this.tssl_ClientState.Size = new System.Drawing.Size(100, 29);
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
      this.tssl_Device.Size = new System.Drawing.Size(127, 29);
      this.tssl_Device.Text = "Device: 15000";
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
      this.tssl_MainFile.Size = new System.Drawing.Size(446, 29);
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
      // cmd_ChannelOn
      // 
      this.cmd_ChannelOn.Location = new System.Drawing.Point(120, 31);
      this.cmd_ChannelOn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ChannelOn.Name = "cmd_ChannelOn";
      this.cmd_ChannelOn.Size = new System.Drawing.Size(122, 34);
      this.cmd_ChannelOn.TabIndex = 2;
      this.cmd_ChannelOn.Text = "On";
      this.cmd_ChannelOn.UseVisualStyleBackColor = true;
      this.cmd_ChannelOn.Click += new System.EventHandler(this.OnCmdChannelOn);
      // 
      // cmd_Disconnect
      // 
      this.cmd_Disconnect.Location = new System.Drawing.Point(183, 49);
      this.cmd_Disconnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_Disconnect.Name = "cmd_Disconnect";
      this.cmd_Disconnect.Size = new System.Drawing.Size(156, 43);
      this.cmd_Disconnect.TabIndex = 5;
      this.cmd_Disconnect.Text = "Disconnect";
      // 
      // cmd_Connect
      // 
      this.cmd_Connect.Location = new System.Drawing.Point(18, 49);
      this.cmd_Connect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_Connect.Name = "cmd_Connect";
      this.cmd_Connect.Size = new System.Drawing.Size(156, 43);
      this.cmd_Connect.TabIndex = 4;
      this.cmd_Connect.Text = "Connect";
      // 
      // cmd_ChannelOff
      // 
      this.cmd_ChannelOff.Location = new System.Drawing.Point(120, 69);
      this.cmd_ChannelOff.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ChannelOff.Name = "cmd_ChannelOff";
      this.cmd_ChannelOff.Size = new System.Drawing.Size(122, 34);
      this.cmd_ChannelOff.TabIndex = 3;
      this.cmd_ChannelOff.Text = "Off";
      this.cmd_ChannelOff.UseVisualStyleBackColor = true;
      this.cmd_ChannelOff.Click += new System.EventHandler(this.OnCmdChannelOff);
      // 
      // cmd_SendCmd
      // 
      this.cmd_SendCmd.Location = new System.Drawing.Point(183, 274);
      this.cmd_SendCmd.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendCmd.Name = "cmd_SendCmd";
      this.cmd_SendCmd.Size = new System.Drawing.Size(156, 54);
      this.cmd_SendCmd.TabIndex = 14;
      this.cmd_SendCmd.Text = "Send Command";
      this.cmd_SendCmd.UseVisualStyleBackColor = true;
      this.cmd_SendCmd.Click += new System.EventHandler(this.OnCmdSendCmd);
      // 
      // cmd_SendString
      // 
      this.cmd_SendString.Location = new System.Drawing.Point(18, 274);
      this.cmd_SendString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendString.Name = "cmd_SendString";
      this.cmd_SendString.Size = new System.Drawing.Size(156, 54);
      this.cmd_SendString.TabIndex = 13;
      this.cmd_SendString.Text = "Send String";
      this.cmd_SendString.UseVisualStyleBackColor = true;
      this.cmd_SendString.Click += new System.EventHandler(this.OnCmdSendString);
      // 
      // txt_Text
      // 
      this.txt_Text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.txt_Text.Location = new System.Drawing.Point(18, 337);
      this.txt_Text.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_Text.Multiline = true;
      this.txt_Text.Name = "txt_Text";
      this.txt_Text.Size = new System.Drawing.Size(798, 202);
      this.txt_Text.TabIndex = 15;
      // 
      // lab_System
      // 
      this.lab_System.AutoSize = true;
      this.lab_System.Location = new System.Drawing.Point(16, 111);
      this.lab_System.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_System.Name = "lab_System";
      this.lab_System.Size = new System.Drawing.Size(62, 20);
      this.lab_System.TabIndex = 4;
      this.lab_System.Text = "System";
      // 
      // num_System
      // 
      this.num_System.Location = new System.Drawing.Point(105, 108);
      this.num_System.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_System.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_System.Name = "num_System";
      this.num_System.Size = new System.Drawing.Size(82, 26);
      this.num_System.TabIndex = 5;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.lab_DevPort);
      this.groupBox1.Controls.Add(this.lab_Device);
      this.groupBox1.Controls.Add(this.num_Device);
      this.groupBox1.Controls.Add(this.num_DevPort);
      this.groupBox1.Controls.Add(this.num_System);
      this.groupBox1.Controls.Add(this.lab_System);
      this.groupBox1.Controls.Add(this.cmd_RequestDeviceStatus);
      this.groupBox1.Location = new System.Drawing.Point(348, 42);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox1.Size = new System.Drawing.Size(206, 242);
      this.groupBox1.TabIndex = 6;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Device";
      // 
      // lab_DevPort
      // 
      this.lab_DevPort.AutoSize = true;
      this.lab_DevPort.Location = new System.Drawing.Point(16, 71);
      this.lab_DevPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_DevPort.Name = "lab_DevPort";
      this.lab_DevPort.Size = new System.Drawing.Size(38, 20);
      this.lab_DevPort.TabIndex = 2;
      this.lab_DevPort.Text = "Port";
      // 
      // lab_Device
      // 
      this.lab_Device.AutoSize = true;
      this.lab_Device.Location = new System.Drawing.Point(16, 31);
      this.lab_Device.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_Device.Name = "lab_Device";
      this.lab_Device.Size = new System.Drawing.Size(57, 20);
      this.lab_Device.TabIndex = 0;
      this.lab_Device.Text = "Device";
      // 
      // num_Device
      // 
      this.num_Device.Location = new System.Drawing.Point(105, 28);
      this.num_Device.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_Device.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_Device.Name = "num_Device";
      this.num_Device.Size = new System.Drawing.Size(82, 26);
      this.num_Device.TabIndex = 1;
      this.num_Device.Value = new decimal(new int[] {
            15000,
            0,
            0,
            0});
      // 
      // num_DevPort
      // 
      this.num_DevPort.Location = new System.Drawing.Point(105, 68);
      this.num_DevPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_DevPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_DevPort.Name = "num_DevPort";
      this.num_DevPort.Size = new System.Drawing.Size(82, 26);
      this.num_DevPort.TabIndex = 3;
      this.num_DevPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // cmd_RequestDeviceStatus
      // 
      this.cmd_RequestDeviceStatus.Location = new System.Drawing.Point(9, 152);
      this.cmd_RequestDeviceStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_RequestDeviceStatus.Name = "cmd_RequestDeviceStatus";
      this.cmd_RequestDeviceStatus.Size = new System.Drawing.Size(178, 75);
      this.cmd_RequestDeviceStatus.TabIndex = 10;
      this.cmd_RequestDeviceStatus.Text = "Request\r\nDevice Status";
      this.cmd_RequestDeviceStatus.UseVisualStyleBackColor = true;
      this.cmd_RequestDeviceStatus.Click += new System.EventHandler(this.OnCmdRequestDeviceStatus);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.lab_Channel);
      this.groupBox2.Controls.Add(this.num_Channel);
      this.groupBox2.Controls.Add(this.cmd_ChannelOn);
      this.groupBox2.Controls.Add(this.cmd_ChannelOff);
      this.groupBox2.Location = new System.Drawing.Point(562, 42);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox2.Size = new System.Drawing.Size(255, 115);
      this.groupBox2.TabIndex = 7;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Channel";
      // 
      // lab_Channel
      // 
      this.lab_Channel.AutoSize = true;
      this.lab_Channel.Location = new System.Drawing.Point(16, 32);
      this.lab_Channel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_Channel.Name = "lab_Channel";
      this.lab_Channel.Size = new System.Drawing.Size(68, 20);
      this.lab_Channel.TabIndex = 0;
      this.lab_Channel.Text = "Channel";
      // 
      // num_Channel
      // 
      this.num_Channel.Location = new System.Drawing.Point(21, 69);
      this.num_Channel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_Channel.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
      this.num_Channel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.num_Channel.Name = "num_Channel";
      this.num_Channel.Size = new System.Drawing.Size(90, 26);
      this.num_Channel.TabIndex = 1;
      this.num_Channel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.lab_LevelValue);
      this.groupBox3.Controls.Add(this.num_LevelInput);
      this.groupBox3.Controls.Add(this.lab_LevelInput);
      this.groupBox3.Controls.Add(this.num_LevelValue);
      this.groupBox3.Controls.Add(this.cmd_SendLevel);
      this.groupBox3.Location = new System.Drawing.Point(562, 166);
      this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox3.Size = new System.Drawing.Size(255, 117);
      this.groupBox3.TabIndex = 8;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Level";
      // 
      // lab_LevelValue
      // 
      this.lab_LevelValue.AutoSize = true;
      this.lab_LevelValue.Location = new System.Drawing.Point(10, 72);
      this.lab_LevelValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_LevelValue.Name = "lab_LevelValue";
      this.lab_LevelValue.Size = new System.Drawing.Size(50, 20);
      this.lab_LevelValue.TabIndex = 2;
      this.lab_LevelValue.Text = "Value";
      // 
      // num_LevelInput
      // 
      this.num_LevelInput.Location = new System.Drawing.Point(69, 28);
      this.num_LevelInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_LevelInput.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
      this.num_LevelInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.num_LevelInput.Name = "num_LevelInput";
      this.num_LevelInput.Size = new System.Drawing.Size(81, 26);
      this.num_LevelInput.TabIndex = 1;
      this.num_LevelInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // lab_LevelInput
      // 
      this.lab_LevelInput.AutoSize = true;
      this.lab_LevelInput.Location = new System.Drawing.Point(10, 32);
      this.lab_LevelInput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_LevelInput.Name = "lab_LevelInput";
      this.lab_LevelInput.Size = new System.Drawing.Size(46, 20);
      this.lab_LevelInput.TabIndex = 0;
      this.lab_LevelInput.Text = "Level";
      // 
      // num_LevelValue
      // 
      this.num_LevelValue.Location = new System.Drawing.Point(70, 68);
      this.num_LevelValue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_LevelValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_LevelValue.Name = "num_LevelValue";
      this.num_LevelValue.Size = new System.Drawing.Size(81, 26);
      this.num_LevelValue.TabIndex = 3;
      this.num_LevelValue.Value = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      // 
      // cmd_SendLevel
      // 
      this.cmd_SendLevel.Location = new System.Drawing.Point(160, 28);
      this.cmd_SendLevel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendLevel.Name = "cmd_SendLevel";
      this.cmd_SendLevel.Size = new System.Drawing.Size(81, 75);
      this.cmd_SendLevel.TabIndex = 4;
      this.cmd_SendLevel.Text = "Send";
      this.cmd_SendLevel.UseVisualStyleBackColor = true;
      this.cmd_SendLevel.Click += new System.EventHandler(this.OnCmdSendLevel);
      // 
      // MainMenu
      // 
      this.MainMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.MainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Settings,
            this.tsmi_Tools});
      this.MainMenu.Location = new System.Drawing.Point(0, 0);
      this.MainMenu.Name = "MainMenu";
      this.MainMenu.Size = new System.Drawing.Size(1521, 33);
      this.MainMenu.TabIndex = 25;
      // 
      // tsmi_Settings
      // 
      this.tsmi_Settings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_CommunicationSetttings,
            this.toolStripMenuItem1,
            this.tsmi_Exit});
      this.tsmi_Settings.Name = "tsmi_Settings";
      this.tsmi_Settings.Size = new System.Drawing.Size(92, 29);
      this.tsmi_Settings.Text = "&Settings";
      // 
      // tsmi_CommunicationSetttings
      // 
      this.tsmi_CommunicationSetttings.Name = "tsmi_CommunicationSetttings";
      this.tsmi_CommunicationSetttings.Size = new System.Drawing.Size(321, 34);
      this.tsmi_CommunicationSetttings.Text = "&Communication Settings...";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(318, 6);
      // 
      // tsmi_Exit
      // 
      this.tsmi_Exit.Name = "tsmi_Exit";
      this.tsmi_Exit.Size = new System.Drawing.Size(321, 34);
      this.tsmi_Exit.Text = "&Exit";
      // 
      // OnlineTree
      // 
      this.OnlineTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.OnlineTree.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
      this.OnlineTree.ImageIndex = 0;
      this.OnlineTree.ImageList = this.ImageList;
      this.OnlineTree.ItemHeight = 20;
      this.OnlineTree.Location = new System.Drawing.Point(826, 74);
      this.OnlineTree.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.OnlineTree.Name = "OnlineTree";
      treeNode1.ImageIndex = 2;
      treeNode1.Name = "Knoten4";
      treeNode1.SelectedImageIndex = 3;
      treeNode1.Text = "Knoten4";
      treeNode2.ImageIndex = 2;
      treeNode2.Name = "Knoten5";
      treeNode2.SelectedImageIndex = 3;
      treeNode2.Text = "Knoten5";
      treeNode3.ImageIndex = 4;
      treeNode3.Name = "Knoten6";
      treeNode3.SelectedImageIndex = 5;
      treeNode3.Text = "Knoten6";
      treeNode4.ImageIndex = 0;
      treeNode4.Name = "Knoten1";
      treeNode4.SelectedImageIndex = 1;
      treeNode4.Text = "Knoten1";
      treeNode5.ImageIndex = 0;
      treeNode5.Name = "Knoten2";
      treeNode5.SelectedImageIndex = 1;
      treeNode5.Text = "Knoten2";
      treeNode6.ImageIndex = 6;
      treeNode6.Name = "Knoten3";
      treeNode6.SelectedImageIndex = 7;
      treeNode6.Text = "Knoten3";
      treeNode7.ImageIndex = 0;
      treeNode7.Name = "Root";
      treeNode7.SelectedImageIndex = 1;
      treeNode7.Text = "<Empty Device Tree>";
      this.OnlineTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode7});
      this.OnlineTree.SelectedImageIndex = 0;
      this.OnlineTree.Size = new System.Drawing.Size(674, 466);
      this.OnlineTree.TabIndex = 26;
      // 
      // ImageList
      // 
      this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
      this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
      this.ImageList.Images.SetKeyName(0, "AMXDeviceDefault");
      this.ImageList.Images.SetKeyName(1, "AMXDeviceSelected");
      this.ImageList.Images.SetKeyName(2, "OIDDeviceDefault");
      this.ImageList.Images.SetKeyName(3, "OIDDeviceSelected");
      this.ImageList.Images.SetKeyName(4, "IODeviceDefault");
      this.ImageList.Images.SetKeyName(5, "IODeviceSelected");
      this.ImageList.Images.SetKeyName(6, "VirtualDeviceDefault");
      this.ImageList.Images.SetKeyName(7, "VirtualDeviceSelected");
      this.ImageList.Images.SetKeyName(8, "CloudDefault");
      this.ImageList.Images.SetKeyName(9, "CloudSelected");
      // 
      // cm_OnlineTree
      // 
      this.cm_OnlineTree.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.cm_OnlineTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmd_RefreshSystemOnlineTree,
            this.toolStripMenuItem2,
            this.cmd_ShowDeviceProperties});
      this.cm_OnlineTree.Name = "cm_OnlineTree";
      this.cm_OnlineTree.Size = new System.Drawing.Size(297, 74);
      // 
      // cmd_RefreshSystemOnlineTree
      // 
      this.cmd_RefreshSystemOnlineTree.Name = "cmd_RefreshSystemOnlineTree";
      this.cmd_RefreshSystemOnlineTree.Size = new System.Drawing.Size(296, 32);
      this.cmd_RefreshSystemOnlineTree.Text = "Refresh System Online Tree";
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(293, 6);
      // 
      // cmd_ShowDeviceProperties
      // 
      this.cmd_ShowDeviceProperties.Name = "cmd_ShowDeviceProperties";
      this.cmd_ShowDeviceProperties.Size = new System.Drawing.Size(296, 32);
      this.cmd_ShowDeviceProperties.Text = "Show Device Properties";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(826, 49);
      this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(90, 20);
      this.label9.TabIndex = 27;
      this.label9.Text = "Online Tree";
      // 
      // cmd_ShowTraceWindow
      // 
      this.cmd_ShowTraceWindow.Location = new System.Drawing.Point(18, 154);
      this.cmd_ShowTraceWindow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ShowTraceWindow.Name = "cmd_ShowTraceWindow";
      this.cmd_ShowTraceWindow.Size = new System.Drawing.Size(321, 43);
      this.cmd_ShowTraceWindow.TabIndex = 29;
      this.cmd_ShowTraceWindow.Text = "Show Trace Window";
      this.cmd_ShowTraceWindow.UseVisualStyleBackColor = true;
      // 
      // cmd_CreatePhysicalDevice
      // 
      this.cmd_CreatePhysicalDevice.Location = new System.Drawing.Point(18, 102);
      this.cmd_CreatePhysicalDevice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_CreatePhysicalDevice.Name = "cmd_CreatePhysicalDevice";
      this.cmd_CreatePhysicalDevice.Size = new System.Drawing.Size(321, 43);
      this.cmd_CreatePhysicalDevice.TabIndex = 30;
      this.cmd_CreatePhysicalDevice.Text = "Settings => Create Physical Device";
      this.cmd_CreatePhysicalDevice.UseVisualStyleBackColor = true;
      // 
      // cmd_ShowFeedbackTest
      // 
      this.cmd_ShowFeedbackTest.Location = new System.Drawing.Point(18, 206);
      this.cmd_ShowFeedbackTest.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ShowFeedbackTest.Name = "cmd_ShowFeedbackTest";
      this.cmd_ShowFeedbackTest.Size = new System.Drawing.Size(321, 43);
      this.cmd_ShowFeedbackTest.TabIndex = 31;
      this.cmd_ShowFeedbackTest.Text = "Feedback Test";
      this.cmd_ShowFeedbackTest.UseVisualStyleBackColor = true;
      // 
      // tsmi_Tools
      // 
      this.tsmi_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Tools_InfoFileTransfer,
            this.tsmi_Tools_OpenTmpFolder});
      this.tsmi_Tools.Name = "tsmi_Tools";
      this.tsmi_Tools.Size = new System.Drawing.Size(69, 29);
      this.tsmi_Tools.Text = "&Tools";
      // 
      // tsmi_Tools_OpenTmpFolder
      // 
      this.tsmi_Tools_OpenTmpFolder.Image = global::ICSPControl.Properties.Resources.file_config;
      this.tsmi_Tools_OpenTmpFolder.Name = "tsmi_Tools_OpenTmpFolder";
      this.tsmi_Tools_OpenTmpFolder.Size = new System.Drawing.Size(288, 34);
      this.tsmi_Tools_OpenTmpFolder.Text = "Open G5 Temp Folder";
      // 
      // tsmi_Tools_InfoFileTransfer
      // 
      this.tsmi_Tools_InfoFileTransfer.Image = global::ICSPControl.Properties.Resources.realvista_general_info_32;
      this.tsmi_Tools_InfoFileTransfer.Name = "tsmi_Tools_InfoFileTransfer";
      this.tsmi_Tools_InfoFileTransfer.Size = new System.Drawing.Size(288, 34);
      this.tsmi_Tools_InfoFileTransfer.Text = "&Info File Transfer";
      // 
      // DlgMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1521, 594);
      this.Controls.Add(this.cmd_ShowFeedbackTest);
      this.Controls.Add(this.cmd_CreatePhysicalDevice);
      this.Controls.Add(this.cmd_ShowTraceWindow);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.OnlineTree);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.txt_Text);
      this.Controls.Add(this.cmd_SendCmd);
      this.Controls.Add(this.cmd_SendString);
      this.Controls.Add(this.cmd_Disconnect);
      this.Controls.Add(this.cmd_Connect);
      this.Controls.Add(this.MainStatusStrip);
      this.Controls.Add(this.MainMenu);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MaximizeBox = false;
      this.MinimumSize = new System.Drawing.Size(1534, 624);
      this.Name = "DlgMain";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "ICSP-Control";
      this.MainStatusStrip.ResumeLayout(false);
      this.MainStatusStrip.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_System)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_Device)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_Channel)).EndInit();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelValue)).EndInit();
      this.MainMenu.ResumeLayout(false);
      this.MainMenu.PerformLayout();
      this.cm_OnlineTree.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
    private System.Windows.Forms.StatusStrip MainStatusStrip;
    private System.Windows.Forms.ToolStripStatusLabel tssl_Host;
    private System.Windows.Forms.ToolStripStatusLabel tssl_Port;
    private System.Windows.Forms.ToolStripStatusLabel tssl_ClientState;
    private System.Windows.Forms.Button cmd_ChannelOn;
    private System.Windows.Forms.Button cmd_Disconnect;
    private System.Windows.Forms.Button cmd_Connect;
    private System.Windows.Forms.Button cmd_ChannelOff;
    private System.Windows.Forms.Button cmd_SendCmd;
    private System.Windows.Forms.Button cmd_SendString;
    private System.Windows.Forms.TextBox txt_Text;
    private System.Windows.Forms.Label lab_System;
    private System.Windows.Forms.NumericUpDown num_System;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label lab_DevPort;
    private System.Windows.Forms.Label lab_Device;
    private System.Windows.Forms.NumericUpDown num_Device;
    private System.Windows.Forms.NumericUpDown num_DevPort;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label lab_Channel;
    private System.Windows.Forms.NumericUpDown num_Channel;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label lab_LevelValue;
    private System.Windows.Forms.NumericUpDown num_LevelInput;
    private System.Windows.Forms.Label lab_LevelInput;
    private System.Windows.Forms.NumericUpDown num_LevelValue;
    private System.Windows.Forms.Button cmd_SendLevel;
    private System.Windows.Forms.ToolStripStatusLabel tssl_Blink;
    private System.Windows.Forms.Button cmd_RequestDeviceStatus;
    private System.Windows.Forms.MenuStrip MainMenu;
    private System.Windows.Forms.ToolStripMenuItem tsmi_Settings;
    private System.Windows.Forms.ToolStripMenuItem tsmi_CommunicationSetttings;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem tsmi_Exit;
    private System.Windows.Forms.ToolStripStatusLabel tssl_Device;
    private System.Windows.Forms.TreeView OnlineTree;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.ImageList ImageList;
    private System.Windows.Forms.ContextMenuStrip cm_OnlineTree;
    private System.Windows.Forms.ToolStripMenuItem cmd_RefreshSystemOnlineTree;
    private System.Windows.Forms.Button cmd_ShowTraceWindow;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem cmd_ShowDeviceProperties;
    private System.Windows.Forms.ToolStripStatusLabel tssl_CurrentSystem;
    private System.Windows.Forms.ToolStripStatusLabel tssl_DynamicDevice;
    private System.Windows.Forms.Button cmd_CreatePhysicalDevice;
    private System.Windows.Forms.ToolStripStatusLabel tssl_ProgramName;
    private System.Windows.Forms.ToolStripStatusLabel tssl_MainFile;
    private System.Windows.Forms.Button cmd_ShowFeedbackTest;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools_InfoFileTransfer;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools_OpenTmpFolder;
    }
}

