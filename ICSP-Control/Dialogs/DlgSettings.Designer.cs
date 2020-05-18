namespace ICSPControl.Dialogs
{
  partial class DlgSettings
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
      this.lab_Port = new System.Windows.Forms.Label();
      this.lab_Host = new System.Windows.Forms.Label();
      this.txt_Host = new System.Windows.Forms.TextBox();
      this.num_Port = new System.Windows.Forms.NumericUpDown();
      this.num_PhysicalDeviceNumber = new System.Windows.Forms.NumericUpDown();
      this.cmd_Abort = new System.Windows.Forms.Button();
      this.cmd_OK = new System.Windows.Forms.Button();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.ckb_PhysicalDeviceUseCustomDeviceId = new System.Windows.Forms.CheckBox();
      this.cbo_PanelType = new System.Windows.Forms.ComboBox();
      this.ckb_PhysicalDeviceAutoCreate = new System.Windows.Forms.CheckBox();
      this.label10 = new System.Windows.Forms.Label();
      this.num_PhysicalDevicePortCount = new System.Windows.Forms.NumericUpDown();
      this.label8 = new System.Windows.Forms.Label();
      this.num_PhysicalDeviceFirmwareId = new System.Windows.Forms.NumericUpDown();
      this.label7 = new System.Windows.Forms.Label();
      this.num_PhysicalDeviceDeviceId = new System.Windows.Forms.NumericUpDown();
      this.label6 = new System.Windows.Forms.Label();
      this.num_PhysicalDeviceManufactureId = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      this.txt_PhysicalDeviceSerialNumber = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txt_PhysicalDeviceManufacturer = new System.Windows.Forms.TextBox();
      this.txt_PhysicalDeviceName = new System.Windows.Forms.TextBox();
      this.txt_PhysicalDeviceVersion = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.ckb_AutoConnect = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label9 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.num_Port)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDeviceNumber)).BeginInit();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDevicePortCount)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDeviceFirmwareId)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDeviceDeviceId)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDeviceManufactureId)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // lab_Port
      // 
      this.lab_Port.AutoSize = true;
      this.lab_Port.Location = new System.Drawing.Point(20, 72);
      this.lab_Port.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_Port.Name = "lab_Port";
      this.lab_Port.Size = new System.Drawing.Size(38, 20);
      this.lab_Port.TabIndex = 2;
      this.lab_Port.Text = "Port";
      // 
      // lab_Host
      // 
      this.lab_Host.AutoSize = true;
      this.lab_Host.Location = new System.Drawing.Point(20, 34);
      this.lab_Host.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_Host.Name = "lab_Host";
      this.lab_Host.Size = new System.Drawing.Size(43, 20);
      this.lab_Host.TabIndex = 0;
      this.lab_Host.Text = "Host";
      // 
      // txt_Host
      // 
      this.txt_Host.Location = new System.Drawing.Point(72, 29);
      this.txt_Host.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_Host.Name = "txt_Host";
      this.txt_Host.Size = new System.Drawing.Size(665, 26);
      this.txt_Host.TabIndex = 1;
      // 
      // num_Port
      // 
      this.num_Port.Location = new System.Drawing.Point(72, 69);
      this.num_Port.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_Port.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.num_Port.Name = "num_Port";
      this.num_Port.Size = new System.Drawing.Size(100, 26);
      this.num_Port.TabIndex = 3;
      this.num_Port.Value = new decimal(new int[] {
            1319,
            0,
            0,
            0});
      // 
      // num_PhysicalDeviceNumber
      // 
      this.num_PhysicalDeviceNumber.Location = new System.Drawing.Point(86, 29);
      this.num_PhysicalDeviceNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_PhysicalDeviceNumber.Maximum = new decimal(new int[] {
            65534,
            0,
            0,
            0});
      this.num_PhysicalDeviceNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.num_PhysicalDeviceNumber.Name = "num_PhysicalDeviceNumber";
      this.num_PhysicalDeviceNumber.Size = new System.Drawing.Size(78, 26);
      this.num_PhysicalDeviceNumber.TabIndex = 1;
      this.num_PhysicalDeviceNumber.Value = new decimal(new int[] {
            15000,
            0,
            0,
            0});
      // 
      // cmd_Abort
      // 
      this.cmd_Abort.DialogResult = System.Windows.Forms.DialogResult.Abort;
      this.cmd_Abort.Location = new System.Drawing.Point(628, 536);
      this.cmd_Abort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_Abort.Name = "cmd_Abort";
      this.cmd_Abort.Size = new System.Drawing.Size(153, 35);
      this.cmd_Abort.TabIndex = 3;
      this.cmd_Abort.Text = "Abort";
      // 
      // cmd_OK
      // 
      this.cmd_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.cmd_OK.Location = new System.Drawing.Point(449, 536);
      this.cmd_OK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_OK.Name = "cmd_OK";
      this.cmd_OK.Size = new System.Drawing.Size(170, 35);
      this.cmd_OK.TabIndex = 2;
      this.cmd_OK.Text = "OK";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.ckb_PhysicalDeviceUseCustomDeviceId);
      this.groupBox4.Controls.Add(this.cbo_PanelType);
      this.groupBox4.Controls.Add(this.ckb_PhysicalDeviceAutoCreate);
      this.groupBox4.Controls.Add(this.label10);
      this.groupBox4.Controls.Add(this.num_PhysicalDevicePortCount);
      this.groupBox4.Controls.Add(this.label8);
      this.groupBox4.Controls.Add(this.num_PhysicalDeviceFirmwareId);
      this.groupBox4.Controls.Add(this.label7);
      this.groupBox4.Controls.Add(this.num_PhysicalDeviceDeviceId);
      this.groupBox4.Controls.Add(this.label6);
      this.groupBox4.Controls.Add(this.num_PhysicalDeviceManufactureId);
      this.groupBox4.Controls.Add(this.label4);
      this.groupBox4.Controls.Add(this.txt_PhysicalDeviceSerialNumber);
      this.groupBox4.Controls.Add(this.label1);
      this.groupBox4.Controls.Add(this.label3);
      this.groupBox4.Controls.Add(this.label2);
      this.groupBox4.Controls.Add(this.txt_PhysicalDeviceManufacturer);
      this.groupBox4.Controls.Add(this.txt_PhysicalDeviceName);
      this.groupBox4.Controls.Add(this.num_PhysicalDeviceNumber);
      this.groupBox4.Controls.Add(this.txt_PhysicalDeviceVersion);
      this.groupBox4.Controls.Add(this.label5);
      this.groupBox4.Location = new System.Drawing.Point(26, 146);
      this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox4.Size = new System.Drawing.Size(755, 355);
      this.groupBox4.TabIndex = 1;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Physical Device";
      // 
      // ckb_PhysicalDeviceUseCustomDeviceId
      // 
      this.ckb_PhysicalDeviceUseCustomDeviceId.AutoSize = true;
      this.ckb_PhysicalDeviceUseCustomDeviceId.Location = new System.Drawing.Point(561, 266);
      this.ckb_PhysicalDeviceUseCustomDeviceId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.ckb_PhysicalDeviceUseCustomDeviceId.Name = "ckb_PhysicalDeviceUseCustomDeviceId";
      this.ckb_PhysicalDeviceUseCustomDeviceId.Size = new System.Drawing.Size(90, 24);
      this.ckb_PhysicalDeviceUseCustomDeviceId.TabIndex = 17;
      this.ckb_PhysicalDeviceUseCustomDeviceId.Text = "Custom";
      this.ckb_PhysicalDeviceUseCustomDeviceId.UseVisualStyleBackColor = true;
      // 
      // cbo_PanelType
      // 
      this.cbo_PanelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbo_PanelType.FormattingEnabled = true;
      this.cbo_PanelType.Location = new System.Drawing.Point(262, 267);
      this.cbo_PanelType.Name = "cbo_PanelType";
      this.cbo_PanelType.Size = new System.Drawing.Size(266, 28);
      this.cbo_PanelType.TabIndex = 16;
      // 
      // ckb_PhysicalDeviceAutoCreate
      // 
      this.ckb_PhysicalDeviceAutoCreate.AutoSize = true;
      this.ckb_PhysicalDeviceAutoCreate.Location = new System.Drawing.Point(262, 31);
      this.ckb_PhysicalDeviceAutoCreate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.ckb_PhysicalDeviceAutoCreate.Name = "ckb_PhysicalDeviceAutoCreate";
      this.ckb_PhysicalDeviceAutoCreate.Size = new System.Drawing.Size(299, 24);
      this.ckb_PhysicalDeviceAutoCreate.TabIndex = 4;
      this.ckb_PhysicalDeviceAutoCreate.Text = "Automatically create when connected";
      this.ckb_PhysicalDeviceAutoCreate.UseVisualStyleBackColor = true;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(10, 72);
      this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(85, 20);
      this.label10.TabIndex = 2;
      this.label10.Text = "Port Count";
      // 
      // num_PhysicalDevicePortCount
      // 
      this.num_PhysicalDevicePortCount.Location = new System.Drawing.Point(105, 69);
      this.num_PhysicalDevicePortCount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_PhysicalDevicePortCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.num_PhysicalDevicePortCount.Name = "num_PhysicalDevicePortCount";
      this.num_PhysicalDevicePortCount.Size = new System.Drawing.Size(58, 26);
      this.num_PhysicalDevicePortCount.TabIndex = 3;
      this.num_PhysicalDevicePortCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(159, 311);
      this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(95, 20);
      this.label8.TabIndex = 19;
      this.label8.Text = "Firmware ID";
      // 
      // num_PhysicalDeviceFirmwareId
      // 
      this.num_PhysicalDeviceFirmwareId.Location = new System.Drawing.Point(262, 308);
      this.num_PhysicalDeviceFirmwareId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_PhysicalDeviceFirmwareId.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_PhysicalDeviceFirmwareId.Name = "num_PhysicalDeviceFirmwareId";
      this.num_PhysicalDeviceFirmwareId.Size = new System.Drawing.Size(78, 26);
      this.num_PhysicalDeviceFirmwareId.TabIndex = 20;
      this.num_PhysicalDeviceFirmwareId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(171, 271);
      this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(78, 20);
      this.label7.TabIndex = 15;
      this.label7.Text = "Device ID";
      // 
      // num_PhysicalDeviceDeviceId
      // 
      this.num_PhysicalDeviceDeviceId.Location = new System.Drawing.Point(659, 265);
      this.num_PhysicalDeviceDeviceId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_PhysicalDeviceDeviceId.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_PhysicalDeviceDeviceId.Name = "num_PhysicalDeviceDeviceId";
      this.num_PhysicalDeviceDeviceId.Size = new System.Drawing.Size(78, 26);
      this.num_PhysicalDeviceDeviceId.TabIndex = 18;
      this.num_PhysicalDeviceDeviceId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(132, 231);
      this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(120, 20);
      this.label6.TabIndex = 13;
      this.label6.Text = "Manufacture ID";
      // 
      // num_PhysicalDeviceManufactureId
      // 
      this.num_PhysicalDeviceManufactureId.Location = new System.Drawing.Point(262, 228);
      this.num_PhysicalDeviceManufactureId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_PhysicalDeviceManufactureId.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.num_PhysicalDeviceManufactureId.Name = "num_PhysicalDeviceManufactureId";
      this.num_PhysicalDeviceManufactureId.Size = new System.Drawing.Size(78, 26);
      this.num_PhysicalDeviceManufactureId.TabIndex = 14;
      this.num_PhysicalDeviceManufactureId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(30, 192);
      this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(221, 20);
      this.label4.TabIndex = 11;
      this.label4.Text = "Serial Number (Max: 16 chars)";
      this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // txt_PhysicalDeviceSerialNumber
      // 
      this.txt_PhysicalDeviceSerialNumber.Location = new System.Drawing.Point(262, 188);
      this.txt_PhysicalDeviceSerialNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_PhysicalDeviceSerialNumber.MaxLength = 16;
      this.txt_PhysicalDeviceSerialNumber.Name = "txt_PhysicalDeviceSerialNumber";
      this.txt_PhysicalDeviceSerialNumber.Size = new System.Drawing.Size(475, 26);
      this.txt_PhysicalDeviceSerialNumber.TabIndex = 12;
      this.txt_PhysicalDeviceSerialNumber.Text = "0000000000000000";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(10, 32);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(57, 20);
      this.label1.TabIndex = 0;
      this.label1.Text = "Device";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(148, 152);
      this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(104, 20);
      this.label3.TabIndex = 9;
      this.label3.Text = "Manufacturer";
      this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(201, 112);
      this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(51, 20);
      this.label2.TabIndex = 7;
      this.label2.Text = "Name";
      this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // txt_PhysicalDeviceManufacturer
      // 
      this.txt_PhysicalDeviceManufacturer.Location = new System.Drawing.Point(262, 148);
      this.txt_PhysicalDeviceManufacturer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_PhysicalDeviceManufacturer.Name = "txt_PhysicalDeviceManufacturer";
      this.txt_PhysicalDeviceManufacturer.Size = new System.Drawing.Size(475, 26);
      this.txt_PhysicalDeviceManufacturer.TabIndex = 10;
      this.txt_PhysicalDeviceManufacturer.Text = "ICSP Manufacturer";
      // 
      // txt_PhysicalDeviceName
      // 
      this.txt_PhysicalDeviceName.Location = new System.Drawing.Point(262, 108);
      this.txt_PhysicalDeviceName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_PhysicalDeviceName.Name = "txt_PhysicalDeviceName";
      this.txt_PhysicalDeviceName.Size = new System.Drawing.Size(475, 26);
      this.txt_PhysicalDeviceName.TabIndex = 8;
      this.txt_PhysicalDeviceName.Text = "ICSP Windows";
      // 
      // txt_PhysicalDeviceVersion
      // 
      this.txt_PhysicalDeviceVersion.Location = new System.Drawing.Point(262, 68);
      this.txt_PhysicalDeviceVersion.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_PhysicalDeviceVersion.Name = "txt_PhysicalDeviceVersion";
      this.txt_PhysicalDeviceVersion.Size = new System.Drawing.Size(475, 26);
      this.txt_PhysicalDeviceVersion.TabIndex = 6;
      this.txt_PhysicalDeviceVersion.Text = "v1.00.00";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(190, 72);
      this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(63, 20);
      this.label5.TabIndex = 5;
      this.label5.Text = "Version";
      this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // ckb_AutoConnect
      // 
      this.ckb_AutoConnect.AutoSize = true;
      this.ckb_AutoConnect.Location = new System.Drawing.Point(182, 74);
      this.ckb_AutoConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.ckb_AutoConnect.Name = "ckb_AutoConnect";
      this.ckb_AutoConnect.Size = new System.Drawing.Size(133, 24);
      this.ckb_AutoConnect.TabIndex = 4;
      this.ckb_AutoConnect.Text = "Auto Connect";
      this.ckb_AutoConnect.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label9);
      this.groupBox1.Controls.Add(this.lab_Host);
      this.groupBox1.Controls.Add(this.ckb_AutoConnect);
      this.groupBox1.Controls.Add(this.num_Port);
      this.groupBox1.Controls.Add(this.txt_Host);
      this.groupBox1.Controls.Add(this.lab_Port);
      this.groupBox1.Location = new System.Drawing.Point(26, 18);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox1.Size = new System.Drawing.Size(755, 118);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Connection";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.ForeColor = System.Drawing.Color.Red;
      this.label9.Location = new System.Drawing.Point(443, 75);
      this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(294, 20);
      this.label9.TabIndex = 5;
      this.label9.Text = "Info: SSL/TLS connection not supported";
      this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // DlgSettings
      // 
      this.AcceptButton = this.cmd_OK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cmd_Abort;
      this.ClientSize = new System.Drawing.Size(794, 585);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.cmd_Abort);
      this.Controls.Add(this.cmd_OK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DlgSettings";
      this.Text = "ICSP Settings";
      ((System.ComponentModel.ISupportInitialize)(this.num_Port)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDeviceNumber)).EndInit();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDevicePortCount)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDeviceFirmwareId)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDeviceDeviceId)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_PhysicalDeviceManufactureId)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lab_Port;
    private System.Windows.Forms.Label lab_Host;
    private System.Windows.Forms.TextBox txt_Host;
    private System.Windows.Forms.NumericUpDown num_Port;
    private System.Windows.Forms.NumericUpDown num_PhysicalDeviceNumber;
    private System.Windows.Forms.Button cmd_Abort;
    private System.Windows.Forms.Button cmd_OK;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txt_PhysicalDeviceManufacturer;
    private System.Windows.Forms.TextBox txt_PhysicalDeviceName;
    private System.Windows.Forms.TextBox txt_PhysicalDeviceVersion;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.CheckBox ckb_AutoConnect;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txt_PhysicalDeviceSerialNumber;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.NumericUpDown num_PhysicalDeviceFirmwareId;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.NumericUpDown num_PhysicalDeviceDeviceId;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown num_PhysicalDeviceManufactureId;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.NumericUpDown num_PhysicalDevicePortCount;
    private System.Windows.Forms.CheckBox ckb_PhysicalDeviceAutoCreate;
        private System.Windows.Forms.CheckBox ckb_PhysicalDeviceUseCustomDeviceId;
        private System.Windows.Forms.ComboBox cbo_PanelType;
    }
}