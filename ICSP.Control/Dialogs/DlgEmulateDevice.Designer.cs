namespace ICSPControl.Dialogs
{
  partial class DlgEmulateDevice
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
      System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Knoten4", 2, 3);
      System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Knoten5", 2, 3);
      System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Knoten6", 4, 5);
      System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Knoten1", 0, 1, new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10});
      System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Knoten2", 0, 1);
      System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Knoten3", 6, 7);
      System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("<Empty Device Tree>", 0, 1, new System.Windows.Forms.TreeNode[] {
            treeNode11,
            treeNode12,
            treeNode13});
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgEmulateDevice));
      this.cmd_ChannelOn = new System.Windows.Forms.Button();
      this.cmd_ChannelOff = new System.Windows.Forms.Button();
      this.cmd_SendCommand = new System.Windows.Forms.Button();
      this.cmd_SendString = new System.Windows.Forms.Button();
      this.txt_Text = new System.Windows.Forms.TextBox();
      this.lab_System = new System.Windows.Forms.Label();
      this.num_System = new System.Windows.Forms.NumericUpDown();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.lab_DevPort = new System.Windows.Forms.Label();
      this.lab_Device = new System.Windows.Forms.Label();
      this.num_Device = new System.Windows.Forms.NumericUpDown();
      this.num_DevPort = new System.Windows.Forms.NumericUpDown();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.cmd_ChannelPush = new System.Windows.Forms.Button();
      this.lab_Channel = new System.Windows.Forms.Label();
      this.num_Channel = new System.Windows.Forms.NumericUpDown();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.lab_LevelValue = new System.Windows.Forms.Label();
      this.num_LevelInput = new System.Windows.Forms.NumericUpDown();
      this.lab_LevelInput = new System.Windows.Forms.Label();
      this.num_LevelValue = new System.Windows.Forms.NumericUpDown();
      this.cmd_SendLevel = new System.Windows.Forms.Button();
      this.OnlineTree = new System.Windows.Forms.TreeView();
      this.ImageList = new System.Windows.Forms.ImageList(this.components);
      this.cm_OnlineTree = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.cmd_RefreshSystemOnlineTree = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.cmd_ShowDeviceProperties = new System.Windows.Forms.ToolStripMenuItem();
      this.label9 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.num_System)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_Device)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).BeginInit();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_Channel)).BeginInit();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelValue)).BeginInit();
      this.cm_OnlineTree.SuspendLayout();
      this.SuspendLayout();
      // 
      // cmd_ChannelOn
      // 
      this.cmd_ChannelOn.Location = new System.Drawing.Point(15, 66);
      this.cmd_ChannelOn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ChannelOn.Name = "cmd_ChannelOn";
      this.cmd_ChannelOn.Size = new System.Drawing.Size(175, 34);
      this.cmd_ChannelOn.TabIndex = 2;
      this.cmd_ChannelOn.Text = "On";
      this.cmd_ChannelOn.UseVisualStyleBackColor = true;
      // 
      // cmd_ChannelOff
      // 
      this.cmd_ChannelOff.Location = new System.Drawing.Point(15, 107);
      this.cmd_ChannelOff.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ChannelOff.Name = "cmd_ChannelOff";
      this.cmd_ChannelOff.Size = new System.Drawing.Size(175, 34);
      this.cmd_ChannelOff.TabIndex = 3;
      this.cmd_ChannelOff.Text = "Off";
      this.cmd_ChannelOff.UseVisualStyleBackColor = true;
      // 
      // cmd_SendCommand
      // 
      this.cmd_SendCommand.Location = new System.Drawing.Point(240, 220);
      this.cmd_SendCommand.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendCommand.Name = "cmd_SendCommand";
      this.cmd_SendCommand.Size = new System.Drawing.Size(210, 54);
      this.cmd_SendCommand.TabIndex = 4;
      this.cmd_SendCommand.Text = "Send Command";
      this.cmd_SendCommand.UseVisualStyleBackColor = true;
      // 
      // cmd_SendString
      // 
      this.cmd_SendString.Location = new System.Drawing.Point(15, 220);
      this.cmd_SendString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendString.Name = "cmd_SendString";
      this.cmd_SendString.Size = new System.Drawing.Size(210, 54);
      this.cmd_SendString.TabIndex = 3;
      this.cmd_SendString.Text = "Send String";
      this.cmd_SendString.UseVisualStyleBackColor = true;
      // 
      // txt_Text
      // 
      this.txt_Text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_Text.Location = new System.Drawing.Point(15, 290);
      this.txt_Text.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_Text.Multiline = true;
      this.txt_Text.Name = "txt_Text";
      this.txt_Text.Size = new System.Drawing.Size(660, 290);
      this.txt_Text.TabIndex = 5;
      // 
      // lab_System
      // 
      this.lab_System.AutoSize = true;
      this.lab_System.Location = new System.Drawing.Point(15, 111);
      this.lab_System.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_System.Name = "lab_System";
      this.lab_System.Size = new System.Drawing.Size(62, 20);
      this.lab_System.TabIndex = 4;
      this.lab_System.Text = "System";
      // 
      // num_System
      // 
      this.num_System.Location = new System.Drawing.Point(100, 108);
      this.num_System.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_System.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_System.Name = "num_System";
      this.num_System.Size = new System.Drawing.Size(90, 26);
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
      this.groupBox1.Location = new System.Drawing.Point(15, 15);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox1.Size = new System.Drawing.Size(210, 195);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Device To Emulate";
      // 
      // lab_DevPort
      // 
      this.lab_DevPort.AutoSize = true;
      this.lab_DevPort.Location = new System.Drawing.Point(15, 71);
      this.lab_DevPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_DevPort.Name = "lab_DevPort";
      this.lab_DevPort.Size = new System.Drawing.Size(38, 20);
      this.lab_DevPort.TabIndex = 2;
      this.lab_DevPort.Text = "Port";
      // 
      // lab_Device
      // 
      this.lab_Device.AutoSize = true;
      this.lab_Device.Location = new System.Drawing.Point(15, 32);
      this.lab_Device.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_Device.Name = "lab_Device";
      this.lab_Device.Size = new System.Drawing.Size(57, 20);
      this.lab_Device.TabIndex = 0;
      this.lab_Device.Text = "Device";
      // 
      // num_Device
      // 
      this.num_Device.Location = new System.Drawing.Point(100, 30);
      this.num_Device.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_Device.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_Device.Name = "num_Device";
      this.num_Device.Size = new System.Drawing.Size(90, 26);
      this.num_Device.TabIndex = 1;
      this.num_Device.Value = new decimal(new int[] {
            15000,
            0,
            0,
            0});
      // 
      // num_DevPort
      // 
      this.num_DevPort.Location = new System.Drawing.Point(100, 68);
      this.num_DevPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_DevPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_DevPort.Name = "num_DevPort";
      this.num_DevPort.Size = new System.Drawing.Size(90, 26);
      this.num_DevPort.TabIndex = 3;
      this.num_DevPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.cmd_ChannelPush);
      this.groupBox2.Controls.Add(this.lab_Channel);
      this.groupBox2.Controls.Add(this.num_Channel);
      this.groupBox2.Controls.Add(this.cmd_ChannelOn);
      this.groupBox2.Controls.Add(this.cmd_ChannelOff);
      this.groupBox2.Location = new System.Drawing.Point(240, 15);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox2.Size = new System.Drawing.Size(210, 195);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Channel";
      // 
      // cmd_ChannelPush
      // 
      this.cmd_ChannelPush.Location = new System.Drawing.Point(15, 148);
      this.cmd_ChannelPush.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ChannelPush.Name = "cmd_ChannelPush";
      this.cmd_ChannelPush.Size = new System.Drawing.Size(175, 34);
      this.cmd_ChannelPush.TabIndex = 4;
      this.cmd_ChannelPush.Text = "Push";
      this.cmd_ChannelPush.UseVisualStyleBackColor = true;
      // 
      // lab_Channel
      // 
      this.lab_Channel.AutoSize = true;
      this.lab_Channel.Location = new System.Drawing.Point(15, 32);
      this.lab_Channel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_Channel.Name = "lab_Channel";
      this.lab_Channel.Size = new System.Drawing.Size(68, 20);
      this.lab_Channel.TabIndex = 0;
      this.lab_Channel.Text = "Channel";
      // 
      // num_Channel
      // 
      this.num_Channel.Location = new System.Drawing.Point(100, 30);
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
      this.groupBox3.Location = new System.Drawing.Point(465, 14);
      this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox3.Size = new System.Drawing.Size(210, 195);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Level";
      // 
      // lab_LevelValue
      // 
      this.lab_LevelValue.AutoSize = true;
      this.lab_LevelValue.Location = new System.Drawing.Point(15, 72);
      this.lab_LevelValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_LevelValue.Name = "lab_LevelValue";
      this.lab_LevelValue.Size = new System.Drawing.Size(50, 20);
      this.lab_LevelValue.TabIndex = 2;
      this.lab_LevelValue.Text = "Value";
      // 
      // num_LevelInput
      // 
      this.num_LevelInput.Location = new System.Drawing.Point(100, 30);
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
      this.num_LevelInput.Size = new System.Drawing.Size(90, 26);
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
      this.lab_LevelInput.Location = new System.Drawing.Point(15, 32);
      this.lab_LevelInput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_LevelInput.Name = "lab_LevelInput";
      this.lab_LevelInput.Size = new System.Drawing.Size(46, 20);
      this.lab_LevelInput.TabIndex = 0;
      this.lab_LevelInput.Text = "Level";
      // 
      // num_LevelValue
      // 
      this.num_LevelValue.Location = new System.Drawing.Point(100, 68);
      this.num_LevelValue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_LevelValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_LevelValue.Name = "num_LevelValue";
      this.num_LevelValue.Size = new System.Drawing.Size(90, 26);
      this.num_LevelValue.TabIndex = 3;
      this.num_LevelValue.Value = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      // 
      // cmd_SendLevel
      // 
      this.cmd_SendLevel.Location = new System.Drawing.Point(15, 107);
      this.cmd_SendLevel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendLevel.Name = "cmd_SendLevel";
      this.cmd_SendLevel.Size = new System.Drawing.Size(175, 76);
      this.cmd_SendLevel.TabIndex = 4;
      this.cmd_SendLevel.Text = "Send";
      this.cmd_SendLevel.UseVisualStyleBackColor = true;
      // 
      // OnlineTree
      // 
      this.OnlineTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.OnlineTree.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
      this.OnlineTree.ImageIndex = 0;
      this.OnlineTree.ImageList = this.ImageList;
      this.OnlineTree.ItemHeight = 20;
      this.OnlineTree.Location = new System.Drawing.Point(687, 39);
      this.OnlineTree.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.OnlineTree.Name = "OnlineTree";
      treeNode8.ImageIndex = 2;
      treeNode8.Name = "Knoten4";
      treeNode8.SelectedImageIndex = 3;
      treeNode8.Text = "Knoten4";
      treeNode9.ImageIndex = 2;
      treeNode9.Name = "Knoten5";
      treeNode9.SelectedImageIndex = 3;
      treeNode9.Text = "Knoten5";
      treeNode10.ImageIndex = 4;
      treeNode10.Name = "Knoten6";
      treeNode10.SelectedImageIndex = 5;
      treeNode10.Text = "Knoten6";
      treeNode11.ImageIndex = 0;
      treeNode11.Name = "Knoten1";
      treeNode11.SelectedImageIndex = 1;
      treeNode11.Text = "Knoten1";
      treeNode12.ImageIndex = 0;
      treeNode12.Name = "Knoten2";
      treeNode12.SelectedImageIndex = 1;
      treeNode12.Text = "Knoten2";
      treeNode13.ImageIndex = 6;
      treeNode13.Name = "Knoten3";
      treeNode13.SelectedImageIndex = 7;
      treeNode13.Text = "Knoten3";
      treeNode14.ImageIndex = 0;
      treeNode14.Name = "Root";
      treeNode14.SelectedImageIndex = 1;
      treeNode14.Text = "<Empty Device Tree>";
      this.OnlineTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode14});
      this.OnlineTree.SelectedImageIndex = 0;
      this.OnlineTree.Size = new System.Drawing.Size(598, 541);
      this.OnlineTree.TabIndex = 7;
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
      this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(683, 15);
      this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(90, 20);
      this.label9.TabIndex = 6;
      this.label9.Text = "Online Tree";
      // 
      // DlgEmulateDevice
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1298, 594);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.OnlineTree);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.txt_Text);
      this.Controls.Add(this.cmd_SendCommand);
      this.Controls.Add(this.cmd_SendString);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "DlgEmulateDevice";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "Emulate a Device";
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
      this.cm_OnlineTree.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
    private System.Windows.Forms.Button cmd_ChannelOn;
    private System.Windows.Forms.Button cmd_ChannelOff;
    private System.Windows.Forms.Button cmd_SendCommand;
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
    private System.Windows.Forms.TreeView OnlineTree;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.ImageList ImageList;
    private System.Windows.Forms.ContextMenuStrip cm_OnlineTree;
    private System.Windows.Forms.ToolStripMenuItem cmd_RefreshSystemOnlineTree;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem cmd_ShowDeviceProperties;
        private System.Windows.Forms.Button cmd_ChannelPush;
    }
}

