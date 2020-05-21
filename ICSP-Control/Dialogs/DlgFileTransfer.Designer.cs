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
      this.grp_JSON = new System.Windows.Forms.GroupBox();
      this.cmd_CreateJson = new System.Windows.Forms.Button();
      this.lab_FileNameJson = new System.Windows.Forms.Label();
      this.txt_FileNameJson = new System.Windows.Forms.TextBox();
      this.ckb_CreateJson = new System.Windows.Forms.CheckBox();
      this.grp_Javsscript = new System.Windows.Forms.GroupBox();
      this.cmd_CreateJs = new System.Windows.Forms.Button();
      this.ckb_CreateJs = new System.Windows.Forms.CheckBox();
      this.txt_VariableNameJs = new System.Windows.Forms.TextBox();
      this.lab_VariableNameJs = new System.Windows.Forms.Label();
      this.lab_FileNameJs = new System.Windows.Forms.Label();
      this.txt_FileNameJs = new System.Windows.Forms.TextBox();
      this.lab_OutputDirectory = new System.Windows.Forms.Label();
      this.txt_PanelDirectory = new System.Windows.Forms.TextBox();
      this.cmd_OutputDirectoryBrowse = new System.Windows.Forms.Button();
      this.FolderDialog = new System.Windows.Forms.FolderBrowserDialog();
      this.lab_Info = new System.Windows.Forms.Label();
      this.txt_Info = new System.Windows.Forms.TextBox();
      this.cmd_OpenFolder = new System.Windows.Forms.Button();
      this.prb_2 = new System.Windows.Forms.ProgressBar();
      this.prb_1 = new System.Windows.Forms.ProgressBar();
      this.label1 = new System.Windows.Forms.Label();
      this.grp_JSON.SuspendLayout();
      this.grp_Javsscript.SuspendLayout();
      this.SuspendLayout();
      // 
      // cmd_CreatePhysicalDevice
      // 
      this.cmd_CreatePhysicalDevice.Location = new System.Drawing.Point(15, 15);
      this.cmd_CreatePhysicalDevice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_CreatePhysicalDevice.Name = "cmd_CreatePhysicalDevice";
      this.cmd_CreatePhysicalDevice.Size = new System.Drawing.Size(321, 43);
      this.cmd_CreatePhysicalDevice.TabIndex = 0;
      this.cmd_CreatePhysicalDevice.Text = "Settings => Create Physical Device";
      this.cmd_CreatePhysicalDevice.UseVisualStyleBackColor = true;
      // 
      // grp_JSON
      // 
      this.grp_JSON.Controls.Add(this.cmd_CreateJson);
      this.grp_JSON.Controls.Add(this.lab_FileNameJson);
      this.grp_JSON.Controls.Add(this.txt_FileNameJson);
      this.grp_JSON.Controls.Add(this.ckb_CreateJson);
      this.grp_JSON.Location = new System.Drawing.Point(15, 145);
      this.grp_JSON.Name = "grp_JSON";
      this.grp_JSON.Size = new System.Drawing.Size(1485, 165);
      this.grp_JSON.TabIndex = 5;
      this.grp_JSON.TabStop = false;
      this.grp_JSON.Text = "JSON";
      // 
      // cmd_CreateJson
      // 
      this.cmd_CreateJson.Location = new System.Drawing.Point(148, 105);
      this.cmd_CreateJson.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_CreateJson.Name = "cmd_CreateJson";
      this.cmd_CreateJson.Size = new System.Drawing.Size(300, 43);
      this.cmd_CreateJson.TabIndex = 3;
      this.cmd_CreateJson.Text = "Create Json";
      this.cmd_CreateJson.UseVisualStyleBackColor = true;
      // 
      // lab_FileNameJson
      // 
      this.lab_FileNameJson.AutoSize = true;
      this.lab_FileNameJson.Location = new System.Drawing.Point(15, 67);
      this.lab_FileNameJson.Name = "lab_FileNameJson";
      this.lab_FileNameJson.Size = new System.Drawing.Size(74, 20);
      this.lab_FileNameJson.TabIndex = 1;
      this.lab_FileNameJson.Text = "Filename";
      // 
      // txt_FileNameJson
      // 
      this.txt_FileNameJson.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_FileNameJson.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_FileNameJson.Location = new System.Drawing.Point(148, 60);
      this.txt_FileNameJson.Name = "txt_FileNameJson";
      this.txt_FileNameJson.Size = new System.Drawing.Size(1319, 30);
      this.txt_FileNameJson.TabIndex = 2;
      this.txt_FileNameJson.Text = "project.json";
      // 
      // ckb_CreateJson
      // 
      this.ckb_CreateJson.AutoSize = true;
      this.ckb_CreateJson.Checked = true;
      this.ckb_CreateJson.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_CreateJson.Location = new System.Drawing.Point(148, 25);
      this.ckb_CreateJson.Name = "ckb_CreateJson";
      this.ckb_CreateJson.Size = new System.Drawing.Size(351, 24);
      this.ckb_CreateJson.TabIndex = 0;
      this.ckb_CreateJson.Text = "Auto generate file [Project].json after transfer";
      this.ckb_CreateJson.UseVisualStyleBackColor = true;
      // 
      // grp_Javsscript
      // 
      this.grp_Javsscript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grp_Javsscript.Controls.Add(this.cmd_CreateJs);
      this.grp_Javsscript.Controls.Add(this.ckb_CreateJs);
      this.grp_Javsscript.Controls.Add(this.txt_VariableNameJs);
      this.grp_Javsscript.Controls.Add(this.lab_VariableNameJs);
      this.grp_Javsscript.Controls.Add(this.lab_FileNameJs);
      this.grp_Javsscript.Controls.Add(this.txt_FileNameJs);
      this.grp_Javsscript.Location = new System.Drawing.Point(15, 320);
      this.grp_Javsscript.Name = "grp_Javsscript";
      this.grp_Javsscript.Size = new System.Drawing.Size(1485, 205);
      this.grp_Javsscript.TabIndex = 6;
      this.grp_Javsscript.TabStop = false;
      this.grp_Javsscript.Text = "Javascript";
      // 
      // cmd_CreateJs
      // 
      this.cmd_CreateJs.Location = new System.Drawing.Point(148, 145);
      this.cmd_CreateJs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.cmd_CreateJs.Name = "cmd_CreateJs";
      this.cmd_CreateJs.Size = new System.Drawing.Size(300, 43);
      this.cmd_CreateJs.TabIndex = 5;
      this.cmd_CreateJs.Text = "Create Json";
      this.cmd_CreateJs.UseVisualStyleBackColor = true;
      // 
      // ckb_CreateJs
      // 
      this.ckb_CreateJs.AutoSize = true;
      this.ckb_CreateJs.Checked = true;
      this.ckb_CreateJs.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckb_CreateJs.Location = new System.Drawing.Point(148, 25);
      this.ckb_CreateJs.Name = "ckb_CreateJs";
      this.ckb_CreateJs.Size = new System.Drawing.Size(333, 24);
      this.ckb_CreateJs.TabIndex = 0;
      this.ckb_CreateJs.Text = "Auto generate file [Project].js after transfer";
      this.ckb_CreateJs.UseVisualStyleBackColor = true;
      // 
      // txt_VariableNameJs
      // 
      this.txt_VariableNameJs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_VariableNameJs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_VariableNameJs.Location = new System.Drawing.Point(148, 100);
      this.txt_VariableNameJs.Name = "txt_VariableNameJs";
      this.txt_VariableNameJs.Size = new System.Drawing.Size(1319, 30);
      this.txt_VariableNameJs.TabIndex = 4;
      this.txt_VariableNameJs.Text = "Proj";
      // 
      // lab_VariableNameJs
      // 
      this.lab_VariableNameJs.AutoSize = true;
      this.lab_VariableNameJs.Location = new System.Drawing.Point(15, 107);
      this.lab_VariableNameJs.Name = "lab_VariableNameJs";
      this.lab_VariableNameJs.Size = new System.Drawing.Size(107, 20);
      this.lab_VariableNameJs.TabIndex = 3;
      this.lab_VariableNameJs.Text = "Variablename";
      // 
      // lab_FileNameJs
      // 
      this.lab_FileNameJs.AutoSize = true;
      this.lab_FileNameJs.Location = new System.Drawing.Point(15, 67);
      this.lab_FileNameJs.Name = "lab_FileNameJs";
      this.lab_FileNameJs.Size = new System.Drawing.Size(74, 20);
      this.lab_FileNameJs.TabIndex = 1;
      this.lab_FileNameJs.Text = "Filename";
      // 
      // txt_FileNameJs
      // 
      this.txt_FileNameJs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_FileNameJs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_FileNameJs.Location = new System.Drawing.Point(148, 60);
      this.txt_FileNameJs.Name = "txt_FileNameJs";
      this.txt_FileNameJs.Size = new System.Drawing.Size(1319, 30);
      this.txt_FileNameJs.TabIndex = 2;
      this.txt_FileNameJs.Text = "project.js";
      // 
      // lab_OutputDirectory
      // 
      this.lab_OutputDirectory.AutoSize = true;
      this.lab_OutputDirectory.Location = new System.Drawing.Point(15, 73);
      this.lab_OutputDirectory.Name = "lab_OutputDirectory";
      this.lab_OutputDirectory.Size = new System.Drawing.Size(117, 20);
      this.lab_OutputDirectory.TabIndex = 1;
      this.lab_OutputDirectory.Text = "Panel-Directory";
      // 
      // txt_PanelDirectory
      // 
      this.txt_PanelDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_PanelDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.txt_PanelDirectory.Location = new System.Drawing.Point(15, 103);
      this.txt_PanelDirectory.Name = "txt_PanelDirectory";
      this.txt_PanelDirectory.Size = new System.Drawing.Size(1270, 30);
      this.txt_PanelDirectory.TabIndex = 2;
      // 
      // cmd_OutputDirectoryBrowse
      // 
      this.cmd_OutputDirectoryBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmd_OutputDirectoryBrowse.Location = new System.Drawing.Point(1291, 103);
      this.cmd_OutputDirectoryBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.cmd_OutputDirectoryBrowse.Name = "cmd_OutputDirectoryBrowse";
      this.cmd_OutputDirectoryBrowse.Size = new System.Drawing.Size(160, 30);
      this.cmd_OutputDirectoryBrowse.TabIndex = 3;
      this.cmd_OutputDirectoryBrowse.Text = "Browse ...";
      this.cmd_OutputDirectoryBrowse.UseVisualStyleBackColor = true;
      // 
      // lab_Info
      // 
      this.lab_Info.AutoSize = true;
      this.lab_Info.Location = new System.Drawing.Point(15, 545);
      this.lab_Info.Name = "lab_Info";
      this.lab_Info.Size = new System.Drawing.Size(279, 20);
      this.lab_Info.TabIndex = 7;
      this.lab_Info.Text = "Current transfer information / Progress";
      // 
      // txt_Info
      // 
      this.txt_Info.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_Info.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txt_Info.Location = new System.Drawing.Point(15, 574);
      this.txt_Info.Multiline = true;
      this.txt_Info.Name = "txt_Info";
      this.txt_Info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txt_Info.Size = new System.Drawing.Size(1485, 458);
      this.txt_Info.TabIndex = 8;
      // 
      // cmd_OpenFolder
      // 
      this.cmd_OpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmd_OpenFolder.Image = global::ICSPControl.Properties.Resources.OpenFolder;
      this.cmd_OpenFolder.Location = new System.Drawing.Point(1457, 103);
      this.cmd_OpenFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.cmd_OpenFolder.Name = "cmd_OpenFolder";
      this.cmd_OpenFolder.Size = new System.Drawing.Size(43, 30);
      this.cmd_OpenFolder.TabIndex = 4;
      this.cmd_OpenFolder.UseVisualStyleBackColor = true;
      // 
      // prb_2
      // 
      this.prb_2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.prb_2.Location = new System.Drawing.Point(15, 1075);
      this.prb_2.Name = "prb_2";
      this.prb_2.Size = new System.Drawing.Size(1485, 20);
      this.prb_2.TabIndex = 10;
      // 
      // prb_1
      // 
      this.prb_1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.prb_1.Location = new System.Drawing.Point(15, 1046);
      this.prb_1.Name = "prb_1";
      this.prb_1.Size = new System.Drawing.Size(1485, 20);
      this.prb_1.TabIndex = 9;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.Red;
      this.label1.Location = new System.Drawing.Point(355, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(778, 50);
      this.label1.TabIndex = 11;
      this.label1.Text = "Important for G5 Panels:\r\nExcecute from Menu: [Panel]/[Verify Function Maps] in T" +
    "PDesign5 before send.";
      // 
      // DlgFileTransfer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1512, 1111);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.prb_1);
      this.Controls.Add(this.prb_2);
      this.Controls.Add(this.cmd_OpenFolder);
      this.Controls.Add(this.lab_Info);
      this.Controls.Add(this.txt_Info);
      this.Controls.Add(this.lab_OutputDirectory);
      this.Controls.Add(this.txt_PanelDirectory);
      this.Controls.Add(this.cmd_OutputDirectoryBrowse);
      this.Controls.Add(this.grp_Javsscript);
      this.Controls.Add(this.grp_JSON);
      this.Controls.Add(this.cmd_CreatePhysicalDevice);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "DlgFileTransfer";
      this.Text = "File Transfer";
      this.grp_JSON.ResumeLayout(false);
      this.grp_JSON.PerformLayout();
      this.grp_Javsscript.ResumeLayout(false);
      this.grp_Javsscript.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
    private System.Windows.Forms.Button cmd_CreatePhysicalDevice;
        private System.Windows.Forms.GroupBox grp_JSON;
        private System.Windows.Forms.CheckBox ckb_CreateJson;
        private System.Windows.Forms.GroupBox grp_Javsscript;
        private System.Windows.Forms.TextBox txt_VariableNameJs;
        private System.Windows.Forms.Label lab_VariableNameJs;
        private System.Windows.Forms.Label lab_FileNameJs;
        private System.Windows.Forms.TextBox txt_FileNameJs;
        private System.Windows.Forms.Label lab_OutputDirectory;
        private System.Windows.Forms.TextBox txt_PanelDirectory;
        private System.Windows.Forms.Button cmd_OutputDirectoryBrowse;
        private System.Windows.Forms.FolderBrowserDialog FolderDialog;
        private System.Windows.Forms.Label lab_Info;
        private System.Windows.Forms.TextBox txt_Info;
        private System.Windows.Forms.Button cmd_OpenFolder;
        private System.Windows.Forms.ProgressBar prb_2;
        private System.Windows.Forms.ProgressBar prb_1;
        private System.Windows.Forms.Button cmd_CreateJson;
        private System.Windows.Forms.Label lab_FileNameJson;
        private System.Windows.Forms.TextBox txt_FileNameJson;
        private System.Windows.Forms.CheckBox ckb_CreateJs;
        private System.Windows.Forms.Button cmd_CreateJs;
    private System.Windows.Forms.Label label1;
  }
}

