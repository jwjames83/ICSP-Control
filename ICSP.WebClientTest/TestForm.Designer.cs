namespace ICSP.WebClientTest
{
  partial class TestForm
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
      if(disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.cmd_Open1 = new System.Windows.Forms.Button();
      this.txt_Url1 = new System.Windows.Forms.TextBox();
      this.cmd_Send1 = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.label9 = new System.Windows.Forms.Label();
      this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
      this.cmd_SendString = new System.Windows.Forms.Button();
      this.cmd_SendCmd = new System.Windows.Forms.Button();
      this.txt_Text = new System.Windows.Forms.TextBox();
      this.cmd_Close1 = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.label8 = new System.Windows.Forms.Label();
      this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
      this.lab_LevelValue = new System.Windows.Forms.Label();
      this.num_LevelInput = new System.Windows.Forms.NumericUpDown();
      this.lab_LevelInput = new System.Windows.Forms.Label();
      this.num_LevelValue = new System.Windows.Forms.NumericUpDown();
      this.cmd_SendLevel = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.label7 = new System.Windows.Forms.Label();
      this.num_ChannelPort1 = new System.Windows.Forms.NumericUpDown();
      this.cmd_Push1 = new System.Windows.Forms.Button();
      this.lab_Channel = new System.Windows.Forms.Label();
      this.num_ChannelChannel1 = new System.Windows.Forms.NumericUpDown();
      this.cmd_ChannelOn = new System.Windows.Forms.Button();
      this.cmd_ChannelOff = new System.Windows.Forms.Button();
      this.txt_Data1 = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txt_Send1 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.cmd_Close2 = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.txt_Data2 = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.txt_Send2 = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.cmd_Open2 = new System.Windows.Forms.Button();
      this.txt_Url2 = new System.Windows.Forms.TextBox();
      this.cmd_Send2 = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.groupBox5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelValue)).BeginInit();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_ChannelPort1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_ChannelChannel1)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // cmd_Open1
      // 
      this.cmd_Open1.Location = new System.Drawing.Point(16, 34);
      this.cmd_Open1.Name = "cmd_Open1";
      this.cmd_Open1.Size = new System.Drawing.Size(219, 58);
      this.cmd_Open1.TabIndex = 0;
      this.cmd_Open1.Text = "Open";
      this.cmd_Open1.UseVisualStyleBackColor = true;
      // 
      // txt_Url1
      // 
      this.txt_Url1.Location = new System.Drawing.Point(79, 119);
      this.txt_Url1.Name = "txt_Url1";
      this.txt_Url1.Size = new System.Drawing.Size(683, 26);
      this.txt_Url1.TabIndex = 1;
      this.txt_Url1.Text = "ws://localhost:5000/AnyPath?AnyParameter=AnyValue";
      // 
      // cmd_Send1
      // 
      this.cmd_Send1.Location = new System.Drawing.Point(487, 34);
      this.cmd_Send1.Name = "cmd_Send1";
      this.cmd_Send1.Size = new System.Drawing.Size(219, 58);
      this.cmd_Send1.TabIndex = 4;
      this.cmd_Send1.Text = "Send";
      this.cmd_Send1.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.groupBox5);
      this.groupBox1.Controls.Add(this.cmd_Close1);
      this.groupBox1.Controls.Add(this.groupBox3);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.groupBox4);
      this.groupBox1.Controls.Add(this.txt_Data1);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.txt_Send1);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.cmd_Open1);
      this.groupBox1.Controls.Add(this.txt_Url1);
      this.groupBox1.Controls.Add(this.cmd_Send1);
      this.groupBox1.Location = new System.Drawing.Point(22, 25);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(812, 1051);
      this.groupBox1.TabIndex = 6;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "WebSocket Client 1";
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.label9);
      this.groupBox5.Controls.Add(this.numericUpDown3);
      this.groupBox5.Controls.Add(this.cmd_SendString);
      this.groupBox5.Controls.Add(this.cmd_SendCmd);
      this.groupBox5.Controls.Add(this.txt_Text);
      this.groupBox5.Location = new System.Drawing.Point(16, 566);
      this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox5.Size = new System.Drawing.Size(763, 471);
      this.groupBox5.TabIndex = 22;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Messages(s) to Send";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(609, 46);
      this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(38, 20);
      this.label9.TabIndex = 28;
      this.label9.Text = "Port";
      // 
      // numericUpDown3
      // 
      this.numericUpDown3.Location = new System.Drawing.Point(656, 44);
      this.numericUpDown3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.numericUpDown3.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
      this.numericUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDown3.Name = "numericUpDown3";
      this.numericUpDown3.Size = new System.Drawing.Size(90, 26);
      this.numericUpDown3.TabIndex = 27;
      this.numericUpDown3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // cmd_SendString
      // 
      this.cmd_SendString.Location = new System.Drawing.Point(23, 29);
      this.cmd_SendString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendString.Name = "cmd_SendString";
      this.cmd_SendString.Size = new System.Drawing.Size(156, 54);
      this.cmd_SendString.TabIndex = 19;
      this.cmd_SendString.Text = "Send String";
      this.cmd_SendString.UseVisualStyleBackColor = true;
      // 
      // cmd_SendCmd
      // 
      this.cmd_SendCmd.Location = new System.Drawing.Point(189, 29);
      this.cmd_SendCmd.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendCmd.Name = "cmd_SendCmd";
      this.cmd_SendCmd.Size = new System.Drawing.Size(156, 54);
      this.cmd_SendCmd.TabIndex = 20;
      this.cmd_SendCmd.Text = "Send Command";
      this.cmd_SendCmd.UseVisualStyleBackColor = true;
      // 
      // txt_Text
      // 
      this.txt_Text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.txt_Text.Location = new System.Drawing.Point(23, 94);
      this.txt_Text.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txt_Text.Multiline = true;
      this.txt_Text.Name = "txt_Text";
      this.txt_Text.Size = new System.Drawing.Size(723, 353);
      this.txt_Text.TabIndex = 21;
      // 
      // cmd_Close1
      // 
      this.cmd_Close1.Location = new System.Drawing.Point(255, 34);
      this.cmd_Close1.Name = "cmd_Close1";
      this.cmd_Close1.Size = new System.Drawing.Size(219, 58);
      this.cmd_Close1.TabIndex = 10;
      this.cmd_Close1.Text = "Close";
      this.cmd_Close1.UseVisualStyleBackColor = true;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.label8);
      this.groupBox3.Controls.Add(this.numericUpDown2);
      this.groupBox3.Controls.Add(this.lab_LevelValue);
      this.groupBox3.Controls.Add(this.num_LevelInput);
      this.groupBox3.Controls.Add(this.lab_LevelInput);
      this.groupBox3.Controls.Add(this.num_LevelValue);
      this.groupBox3.Controls.Add(this.cmd_SendLevel);
      this.groupBox3.Location = new System.Drawing.Point(507, 320);
      this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox3.Size = new System.Drawing.Size(255, 219);
      this.groupBox3.TabIndex = 18;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Level";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(17, 32);
      this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(38, 20);
      this.label8.TabIndex = 26;
      this.label8.Text = "Port";
      // 
      // numericUpDown2
      // 
      this.numericUpDown2.Location = new System.Drawing.Point(17, 66);
      this.numericUpDown2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.numericUpDown2.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
      this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDown2.Name = "numericUpDown2";
      this.numericUpDown2.Size = new System.Drawing.Size(90, 26);
      this.numericUpDown2.TabIndex = 25;
      this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // lab_LevelValue
      // 
      this.lab_LevelValue.AutoSize = true;
      this.lab_LevelValue.Location = new System.Drawing.Point(5, 174);
      this.lab_LevelValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_LevelValue.Name = "lab_LevelValue";
      this.lab_LevelValue.Size = new System.Drawing.Size(50, 20);
      this.lab_LevelValue.TabIndex = 2;
      this.lab_LevelValue.Text = "Value";
      // 
      // num_LevelInput
      // 
      this.num_LevelInput.Location = new System.Drawing.Point(64, 130);
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
      this.lab_LevelInput.Location = new System.Drawing.Point(5, 134);
      this.lab_LevelInput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_LevelInput.Name = "lab_LevelInput";
      this.lab_LevelInput.Size = new System.Drawing.Size(46, 20);
      this.lab_LevelInput.TabIndex = 0;
      this.lab_LevelInput.Text = "Level";
      // 
      // num_LevelValue
      // 
      this.num_LevelValue.Location = new System.Drawing.Point(65, 170);
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
      this.cmd_SendLevel.Location = new System.Drawing.Point(155, 130);
      this.cmd_SendLevel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_SendLevel.Name = "cmd_SendLevel";
      this.cmd_SendLevel.Size = new System.Drawing.Size(81, 75);
      this.cmd_SendLevel.TabIndex = 4;
      this.cmd_SendLevel.Text = "Send";
      this.cmd_SendLevel.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 186);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(44, 20);
      this.label3.TabIndex = 9;
      this.label3.Text = "Data";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.label7);
      this.groupBox4.Controls.Add(this.num_ChannelPort1);
      this.groupBox4.Controls.Add(this.cmd_Push1);
      this.groupBox4.Controls.Add(this.lab_Channel);
      this.groupBox4.Controls.Add(this.num_ChannelChannel1);
      this.groupBox4.Controls.Add(this.cmd_ChannelOn);
      this.groupBox4.Controls.Add(this.cmd_ChannelOff);
      this.groupBox4.Location = new System.Drawing.Point(16, 320);
      this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox4.Size = new System.Drawing.Size(386, 219);
      this.groupBox4.TabIndex = 17;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Channel";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(8, 32);
      this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(38, 20);
      this.label7.TabIndex = 24;
      this.label7.Text = "Port";
      // 
      // num_ChannelPort1
      // 
      this.num_ChannelPort1.Location = new System.Drawing.Point(8, 66);
      this.num_ChannelPort1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_ChannelPort1.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
      this.num_ChannelPort1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.num_ChannelPort1.Name = "num_ChannelPort1";
      this.num_ChannelPort1.Size = new System.Drawing.Size(90, 26);
      this.num_ChannelPort1.TabIndex = 23;
      this.num_ChannelPort1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // cmd_Push1
      // 
      this.cmd_Push1.Location = new System.Drawing.Point(242, 131);
      this.cmd_Push1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_Push1.Name = "cmd_Push1";
      this.cmd_Push1.Size = new System.Drawing.Size(122, 72);
      this.cmd_Push1.TabIndex = 4;
      this.cmd_Push1.Text = "Push";
      this.cmd_Push1.UseVisualStyleBackColor = true;
      // 
      // lab_Channel
      // 
      this.lab_Channel.AutoSize = true;
      this.lab_Channel.Location = new System.Drawing.Point(8, 132);
      this.lab_Channel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lab_Channel.Name = "lab_Channel";
      this.lab_Channel.Size = new System.Drawing.Size(68, 20);
      this.lab_Channel.TabIndex = 0;
      this.lab_Channel.Text = "Channel";
      // 
      // num_ChannelChannel1
      // 
      this.num_ChannelChannel1.Location = new System.Drawing.Point(13, 169);
      this.num_ChannelChannel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.num_ChannelChannel1.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
      this.num_ChannelChannel1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.num_ChannelChannel1.Name = "num_ChannelChannel1";
      this.num_ChannelChannel1.Size = new System.Drawing.Size(90, 26);
      this.num_ChannelChannel1.TabIndex = 1;
      this.num_ChannelChannel1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // cmd_ChannelOn
      // 
      this.cmd_ChannelOn.Location = new System.Drawing.Point(112, 131);
      this.cmd_ChannelOn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ChannelOn.Name = "cmd_ChannelOn";
      this.cmd_ChannelOn.Size = new System.Drawing.Size(122, 34);
      this.cmd_ChannelOn.TabIndex = 2;
      this.cmd_ChannelOn.Text = "On";
      this.cmd_ChannelOn.UseVisualStyleBackColor = true;
      // 
      // cmd_ChannelOff
      // 
      this.cmd_ChannelOff.Location = new System.Drawing.Point(112, 169);
      this.cmd_ChannelOff.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_ChannelOff.Name = "cmd_ChannelOff";
      this.cmd_ChannelOff.Size = new System.Drawing.Size(122, 34);
      this.cmd_ChannelOff.TabIndex = 3;
      this.cmd_ChannelOff.Text = "Off";
      this.cmd_ChannelOff.UseVisualStyleBackColor = true;
      // 
      // txt_Data1
      // 
      this.txt_Data1.Location = new System.Drawing.Point(79, 183);
      this.txt_Data1.Multiline = true;
      this.txt_Data1.Name = "txt_Data1";
      this.txt_Data1.Size = new System.Drawing.Size(683, 109);
      this.txt_Data1.TabIndex = 8;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 154);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(47, 20);
      this.label2.TabIndex = 7;
      this.label2.Text = "Send";
      // 
      // txt_Send1
      // 
      this.txt_Send1.Location = new System.Drawing.Point(79, 151);
      this.txt_Send1.Name = "txt_Send1";
      this.txt_Send1.Size = new System.Drawing.Size(683, 26);
      this.txt_Send1.TabIndex = 6;
      this.txt_Send1.Text = "PUSH:1:255;";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 122);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(29, 20);
      this.label1.TabIndex = 5;
      this.label1.Text = "Url";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.cmd_Close2);
      this.groupBox2.Controls.Add(this.label4);
      this.groupBox2.Controls.Add(this.txt_Data2);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.txt_Send2);
      this.groupBox2.Controls.Add(this.label6);
      this.groupBox2.Controls.Add(this.cmd_Open2);
      this.groupBox2.Controls.Add(this.txt_Url2);
      this.groupBox2.Controls.Add(this.cmd_Send2);
      this.groupBox2.Location = new System.Drawing.Point(840, 25);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(786, 312);
      this.groupBox2.TabIndex = 11;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "WebSocket Client 2";
      // 
      // cmd_Close2
      // 
      this.cmd_Close2.Location = new System.Drawing.Point(255, 34);
      this.cmd_Close2.Name = "cmd_Close2";
      this.cmd_Close2.Size = new System.Drawing.Size(219, 58);
      this.cmd_Close2.TabIndex = 10;
      this.cmd_Close2.Text = "Close";
      this.cmd_Close2.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 186);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(44, 20);
      this.label4.TabIndex = 9;
      this.label4.Text = "Data";
      // 
      // txt_Data2
      // 
      this.txt_Data2.Location = new System.Drawing.Point(79, 183);
      this.txt_Data2.Multiline = true;
      this.txt_Data2.Name = "txt_Data2";
      this.txt_Data2.Size = new System.Drawing.Size(683, 109);
      this.txt_Data2.TabIndex = 8;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(12, 154);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(47, 20);
      this.label5.TabIndex = 7;
      this.label5.Text = "Send";
      // 
      // txt_Send2
      // 
      this.txt_Send2.Location = new System.Drawing.Point(79, 151);
      this.txt_Send2.Name = "txt_Send2";
      this.txt_Send2.Size = new System.Drawing.Size(683, 26);
      this.txt_Send2.TabIndex = 6;
      this.txt_Send2.Text = "STRING:1:Hello";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(12, 122);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(29, 20);
      this.label6.TabIndex = 5;
      this.label6.Text = "Url";
      // 
      // cmd_Open2
      // 
      this.cmd_Open2.Location = new System.Drawing.Point(16, 34);
      this.cmd_Open2.Name = "cmd_Open2";
      this.cmd_Open2.Size = new System.Drawing.Size(219, 58);
      this.cmd_Open2.TabIndex = 0;
      this.cmd_Open2.Text = "Open";
      this.cmd_Open2.UseVisualStyleBackColor = true;
      // 
      // txt_Url2
      // 
      this.txt_Url2.Location = new System.Drawing.Point(79, 119);
      this.txt_Url2.Name = "txt_Url2";
      this.txt_Url2.Size = new System.Drawing.Size(683, 26);
      this.txt_Url2.TabIndex = 1;
      this.txt_Url2.Text = "ws://localhost:8080/Test?IP=169.254.0.1";
      // 
      // cmd_Send2
      // 
      this.cmd_Send2.Location = new System.Drawing.Point(487, 34);
      this.cmd_Send2.Name = "cmd_Send2";
      this.cmd_Send2.Size = new System.Drawing.Size(219, 58);
      this.cmd_Send2.TabIndex = 4;
      this.cmd_Send2.Text = "Send";
      this.cmd_Send2.UseVisualStyleBackColor = true;
      // 
      // TestForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1778, 1092);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "TestForm";
      this.Text = "WebSocket Test-Form";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_LevelValue)).EndInit();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_ChannelPort1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_ChannelChannel1)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);

    }

        #endregion

        private System.Windows.Forms.Button cmd_Open1;
        private System.Windows.Forms.TextBox txt_Url1;
        private System.Windows.Forms.Button cmd_Send1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmd_Close1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_Data1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Send1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmd_Close2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_Data2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_Send2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button cmd_Open2;
        private System.Windows.Forms.TextBox txt_Url2;
        private System.Windows.Forms.Button cmd_Send2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lab_LevelValue;
        private System.Windows.Forms.NumericUpDown num_LevelInput;
        private System.Windows.Forms.Label lab_LevelInput;
        private System.Windows.Forms.NumericUpDown num_LevelValue;
        private System.Windows.Forms.Button cmd_SendLevel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button cmd_Push1;
        private System.Windows.Forms.Label lab_Channel;
        private System.Windows.Forms.NumericUpDown num_ChannelChannel1;
        private System.Windows.Forms.Button cmd_ChannelOn;
        private System.Windows.Forms.Button cmd_ChannelOff;
        private System.Windows.Forms.TextBox txt_Text;
        private System.Windows.Forms.Button cmd_SendCmd;
        private System.Windows.Forms.Button cmd_SendString;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown num_ChannelPort1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
    }
}

