namespace Fr.Zhou.S3
{
    partial class S3Notify
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
            if (disposing && (components != null))
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(S3Notify));
            this.label1 = new System.Windows.Forms.Label();
            this.txtAWSID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAWSKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBucket = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLocalPath = new System.Windows.Forms.TextBox();
            this.localDirBrowser = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.manuallySyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "AWS ID:";
            // 
            // txtAWSID
            // 
            this.txtAWSID.Location = new System.Drawing.Point(95, 12);
            this.txtAWSID.Name = "txtAWSID";
            this.txtAWSID.Size = new System.Drawing.Size(188, 20);
            this.txtAWSID.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "AWS Key:";
            // 
            // txtAWSKey
            // 
            this.txtAWSKey.Location = new System.Drawing.Point(95, 49);
            this.txtAWSKey.Name = "txtAWSKey";
            this.txtAWSKey.Size = new System.Drawing.Size(188, 20);
            this.txtAWSKey.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Bucket Name:";
            // 
            // txtBucket
            // 
            this.txtBucket.Location = new System.Drawing.Point(96, 86);
            this.txtBucket.Name = "txtBucket";
            this.txtBucket.Size = new System.Drawing.Size(187, 20);
            this.txtBucket.TabIndex = 3;
            this.txtBucket.Text = "steatite";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Local Directory:";
            // 
            // txtLocalPath
            // 
            this.txtLocalPath.Location = new System.Drawing.Point(95, 123);
            this.txtLocalPath.Name = "txtLocalPath";
            this.txtLocalPath.ReadOnly = true;
            this.txtLocalPath.Size = new System.Drawing.Size(152, 20);
            this.txtLocalPath.TabIndex = 7;
            this.txtLocalPath.Text = "C:\\";
            // 
            // localDirBrowser
            // 
            this.localDirBrowser.Location = new System.Drawing.Point(253, 122);
            this.localDirBrowser.Name = "localDirBrowser";
            this.localDirBrowser.Size = new System.Drawing.Size(30, 23);
            this.localDirBrowser.TabIndex = 4;
            this.localDirBrowser.Text = "...";
            this.localDirBrowser.UseVisualStyleBackColor = true;
            this.localDirBrowser.Click += new System.EventHandler(this.localDirBrowser_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(134, 161);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(215, 161);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 10;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.IncludeSubdirectories = true;
            this.fileSystemWatcher.SynchronizingObject = this;
            this.fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(this.fileSystemWatcher_Renamed);
            this.fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Deleted);
            this.fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
            this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Amazon S3 Sync [not running]";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSetting,
            this.manuallySyncToolStripMenuItem,
            this.menuExit});
            this.contextMenuStrip.Name = "NotifyContextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(179, 92);
            // 
            // menuSetting
            // 
            this.menuSetting.Name = "menuSetting";
            this.menuSetting.Size = new System.Drawing.Size(178, 22);
            this.menuSetting.Text = "Open Setting Window";
            this.menuSetting.Click += new System.EventHandler(this.menuSetting_Click);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(178, 22);
            this.menuExit.Text = "Exit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // manuallySyncToolStripMenuItem
            // 
            this.manuallySyncToolStripMenuItem.Name = "manuallySyncToolStripMenuItem";
            this.manuallySyncToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.manuallySyncToolStripMenuItem.Text = "Force Sync Manually";
            this.manuallySyncToolStripMenuItem.Click += new System.EventHandler(this.manuallySyncToolStripMenuItem_Click);
            // 
            // S3Notify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 190);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.localDirBrowser);
            this.Controls.Add(this.txtLocalPath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtBucket);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAWSKey);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAWSID);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "S3Notify";
            this.ShowInTaskbar = false;
            this.Text = "Amazon S3 Sync";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.S3Notify_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.S3Notify_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAWSID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAWSKey;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBucket;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLocalPath;
        private System.Windows.Forms.Button localDirBrowser;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnExit;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuSetting;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem manuallySyncToolStripMenuItem;
    }
}

