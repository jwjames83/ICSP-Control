namespace ICSP.WebClientTest
{
  partial class Form1
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
      this.cmd_Close1 = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
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
      this.txt_Url1.Text = "ws://localhost:8080/a";
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
      this.groupBox1.Controls.Add(this.cmd_Close1);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.txt_Data1);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.txt_Send1);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.cmd_Open1);
      this.groupBox1.Controls.Add(this.txt_Url1);
      this.groupBox1.Controls.Add(this.cmd_Send1);
      this.groupBox1.Location = new System.Drawing.Point(49, 38);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(786, 312);
      this.groupBox1.TabIndex = 6;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "WebSocket Client 1";
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
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 186);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(44, 20);
      this.label3.TabIndex = 9;
      this.label3.Text = "Data";
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
      this.txt_Send1.Text = "Hello 1";
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
      this.groupBox2.Location = new System.Drawing.Point(49, 368);
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
      this.txt_Send2.Text = "Hello 2";
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
      this.txt_Url2.Text = "ws://localhost:8080/b";
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
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(924, 714);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
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
    }
}

