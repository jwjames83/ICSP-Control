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
      this.grp_Development = new System.Windows.Forms.GroupBox();
      this.ckb_CreateJson = new System.Windows.Forms.CheckBox();
      this.grp_Javsscript = new System.Windows.Forms.GroupBox();
      this.txt_JsVariableName = new System.Windows.Forms.TextBox();
      this.lab_JsVariableName = new System.Windows.Forms.Label();
      this.lab_JsFileName = new System.Windows.Forms.Label();
      this.txt_JsFileName = new System.Windows.Forms.TextBox();
      this.lab_OutputDirectory = new System.Windows.Forms.Label();
      this.txt_PanelDirectory = new System.Windows.Forms.TextBox();
      this.cmd_OutputDirectoryBrowse = new System.Windows.Forms.Button();
      this.FolderDialog = new System.Windows.Forms.FolderBrowserDialog();
      this.lab_Info = new System.Windows.Forms.Label();
      this.txt_Info = new System.Windows.Forms.TextBox();
      this.cmd_OpenFolder = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.prb_2 = new System.Windows.Forms.ProgressBar();
      this.prb_1 = new System.Windows.Forms.ProgressBar();
      this.grp_Development.SuspendLayout();
      this.grp_Javsscript.SuspendLayout();
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
      // grp_Development
      // 
      this.grp_Development.Controls.Add(this.ckb_CreateJson);
      this.grp_Development.Location = new System.Drawing.Point(13, 145);
      this.grp_Development.Name = "grp_Development";
      this.grp_Development.Size = new System.Drawing.Size(301, 81);
      this.grp_Development.TabIndex = 31;
      this.grp_Development.TabStop = false;
      this.grp_Development.Text = "Development";
      // 
      // ckb_CreateJson
      // 
      this.ckb_CreateJson.AutoSize = true;
      this.ckb_CreateJson.Checked = true;
      this.ckb_CreateJson.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_CreateJson.Location = new System.Drawing.Point(18, 34);
      this.ckb_CreateJson.Name = "ckb_CreateJson";
      this.ckb_CreateJson.Size = new System.Drawing.Size(232, 24);
      this.ckb_CreateJson.TabIndex = 0;
      this.ckb_CreateJson.Text = "Generate single JSON-Files";
      this.ckb_CreateJson.UseVisualStyleBackColor = true;
      // 
      // grp_Javsscript
      // 
      this.grp_Javsscript.Controls.Add(this.txt_JsVariableName);
      this.grp_Javsscript.Controls.Add(this.lab_JsVariableName);
      this.grp_Javsscript.Controls.Add(this.lab_JsFileName);
      this.grp_Javsscript.Controls.Add(this.txt_JsFileName);
      this.grp_Javsscript.Location = new System.Drawing.Point(329, 145);
      this.grp_Javsscript.Name = "grp_Javsscript";
      this.grp_Javsscript.Size = new System.Drawing.Size(1171, 114);
      this.grp_Javsscript.TabIndex = 32;
      this.grp_Javsscript.TabStop = false;
      this.grp_Javsscript.Text = "JSON / Javascript: Project";
      // 
      // txt_JsVariableName
      // 
      this.txt_JsVariableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_JsVariableName.Location = new System.Drawing.Point(148, 68);
      this.txt_JsVariableName.Name = "txt_JsVariableName";
      this.txt_JsVariableName.Size = new System.Drawing.Size(1005, 30);
      this.txt_JsVariableName.TabIndex = 3;
      this.txt_JsVariableName.Text = "Proj";
      // 
      // lab_JsVariableName
      // 
      this.lab_JsVariableName.AutoSize = true;
      this.lab_JsVariableName.Location = new System.Drawing.Point(15, 75);
      this.lab_JsVariableName.Name = "lab_JsVariableName";
      this.lab_JsVariableName.Size = new System.Drawing.Size(116, 20);
      this.lab_JsVariableName.TabIndex = 2;
      this.lab_JsVariableName.Text = "Variablenname";
      // 
      // lab_JsFileName
      // 
      this.lab_JsFileName.AutoSize = true;
      this.lab_JsFileName.Location = new System.Drawing.Point(15, 35);
      this.lab_JsFileName.Name = "lab_JsFileName";
      this.lab_JsFileName.Size = new System.Drawing.Size(74, 20);
      this.lab_JsFileName.TabIndex = 0;
      this.lab_JsFileName.Text = "Filename";
      // 
      // txt_JsFileName
      // 
      this.txt_JsFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_JsFileName.Location = new System.Drawing.Point(148, 28);
      this.txt_JsFileName.Name = "txt_JsFileName";
      this.txt_JsFileName.Size = new System.Drawing.Size(1005, 30);
      this.txt_JsFileName.TabIndex = 1;
      this.txt_JsFileName.Text = "project.js";
      // 
      // lab_OutputDirectory
      // 
      this.lab_OutputDirectory.AutoSize = true;
      this.lab_OutputDirectory.Location = new System.Drawing.Point(15, 73);
      this.lab_OutputDirectory.Name = "lab_OutputDirectory";
      this.lab_OutputDirectory.Size = new System.Drawing.Size(117, 20);
      this.lab_OutputDirectory.TabIndex = 33;
      this.lab_OutputDirectory.Text = "Panel-Directory";
      // 
      // txt_PanelDirectory
      // 
      this.txt_PanelDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_PanelDirectory.Location = new System.Drawing.Point(13, 103);
      this.txt_PanelDirectory.Name = "txt_PanelDirectory";
      this.txt_PanelDirectory.Size = new System.Drawing.Size(1272, 26);
      this.txt_PanelDirectory.TabIndex = 34;
      // 
      // cmd_OutputDirectoryBrowse
      // 
      this.cmd_OutputDirectoryBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmd_OutputDirectoryBrowse.Location = new System.Drawing.Point(1291, 103);
      this.cmd_OutputDirectoryBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.cmd_OutputDirectoryBrowse.Name = "cmd_OutputDirectoryBrowse";
      this.cmd_OutputDirectoryBrowse.Size = new System.Drawing.Size(160, 26);
      this.cmd_OutputDirectoryBrowse.TabIndex = 35;
      this.cmd_OutputDirectoryBrowse.Text = "Browse ...";
      this.cmd_OutputDirectoryBrowse.UseVisualStyleBackColor = true;
      // 
      // lab_Info
      // 
      this.lab_Info.AutoSize = true;
      this.lab_Info.Location = new System.Drawing.Point(8, 339);
      this.lab_Info.Name = "lab_Info";
      this.lab_Info.Size = new System.Drawing.Size(204, 20);
      this.lab_Info.TabIndex = 36;
      this.lab_Info.Text = "Current transfer information";
      // 
      // txt_Info
      // 
      this.txt_Info.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_Info.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_Info.Location = new System.Drawing.Point(13, 369);
      this.txt_Info.Multiline = true;
      this.txt_Info.Name = "txt_Info";
      this.txt_Info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txt_Info.Size = new System.Drawing.Size(1487, 377);
      this.txt_Info.TabIndex = 37;
      // 
      // cmd_OpenFolder
      // 
      this.cmd_OpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmd_OpenFolder.Image = global::ICSPControl.Properties.Resources.OpenFolder;
      this.cmd_OpenFolder.Location = new System.Drawing.Point(1457, 103);
      this.cmd_OpenFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.cmd_OpenFolder.Name = "cmd_OpenFolder";
      this.cmd_OpenFolder.Size = new System.Drawing.Size(43, 26);
      this.cmd_OpenFolder.TabIndex = 38;
      this.cmd_OpenFolder.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 239);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(72, 20);
      this.label1.TabIndex = 39;
      this.label1.Text = "Progress";
      // 
      // prb_2
      // 
      this.prb_2.Location = new System.Drawing.Point(12, 299);
      this.prb_2.Name = "prb_2";
      this.prb_2.Size = new System.Drawing.Size(1488, 23);
      this.prb_2.TabIndex = 40;
      // 
      // prb_1
      // 
      this.prb_1.Location = new System.Drawing.Point(12, 270);
      this.prb_1.Name = "prb_1";
      this.prb_1.Size = new System.Drawing.Size(1488, 23);
      this.prb_1.TabIndex = 41;
      // 
      // DlgFileTransfer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1512, 758);
      this.Controls.Add(this.prb_1);
      this.Controls.Add(this.prb_2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.cmd_OpenFolder);
      this.Controls.Add(this.lab_Info);
      this.Controls.Add(this.txt_Info);
      this.Controls.Add(this.lab_OutputDirectory);
      this.Controls.Add(this.txt_PanelDirectory);
      this.Controls.Add(this.cmd_OutputDirectoryBrowse);
      this.Controls.Add(this.grp_Javsscript);
      this.Controls.Add(this.grp_Development);
      this.Controls.Add(this.cmd_CreatePhysicalDevice);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MinimumSize = new System.Drawing.Size(1534, 624);
      this.Name = "DlgFileTransfer";
      this.Text = "File Transfer";
      this.grp_Development.ResumeLayout(false);
      this.grp_Development.PerformLayout();
      this.grp_Javsscript.ResumeLayout(false);
      this.grp_Javsscript.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
    private System.Windows.Forms.Button cmd_CreatePhysicalDevice;
        private System.Windows.Forms.GroupBox grp_Development;
        private System.Windows.Forms.CheckBox ckb_CreateJson;
        private System.Windows.Forms.GroupBox grp_Javsscript;
        private System.Windows.Forms.TextBox txt_JsVariableName;
        private System.Windows.Forms.Label lab_JsVariableName;
        private System.Windows.Forms.Label lab_JsFileName;
        private System.Windows.Forms.TextBox txt_JsFileName;
        private System.Windows.Forms.Label lab_OutputDirectory;
        private System.Windows.Forms.TextBox txt_PanelDirectory;
        private System.Windows.Forms.Button cmd_OutputDirectoryBrowse;
        private System.Windows.Forms.FolderBrowserDialog FolderDialog;
        private System.Windows.Forms.Label lab_Info;
        private System.Windows.Forms.TextBox txt_Info;
        private System.Windows.Forms.Button cmd_OpenFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar prb_2;
        private System.Windows.Forms.ProgressBar prb_1;
    }
}

