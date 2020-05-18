namespace ICSPControl.Dialogs
{
  partial class DlgFileTransfer
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgFileTransfer));
      this.cmd_CreatePhysicalDevice = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // cmd_CreatePhysicalDevice
      // 
      this.cmd_CreatePhysicalDevice.Location = new System.Drawing.Point(13, 14);
      this.cmd_CreatePhysicalDevice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_CreatePhysicalDevice.Name = "cmd_CreatePhysicalDevice";
      this.cmd_CreatePhysicalDevice.Size = new System.Drawing.Size(321, 43);
      this.cmd_CreatePhysicalDevice.TabIndex = 30;
      this.cmd_CreatePhysicalDevice.Text = "Settings => Create Physical Device";
      this.cmd_CreatePhysicalDevice.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(438, 316);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(671, 113);
      this.label1.TabIndex = 31;
      this.label1.Text = "In Progress ...";
      // 
      // DlgFileTransfer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1611, 822);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.cmd_CreatePhysicalDevice);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MinimumSize = new System.Drawing.Size(1534, 624);
      this.Name = "DlgFileTransfer";
      this.Text = "File Transfer";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
    private System.Windows.Forms.Button cmd_CreatePhysicalDevice;
        private System.Windows.Forms.Label label1;
    }
}

