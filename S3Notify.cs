using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;

namespace Fr.Zhou.S3
{

    public partial class S3Notify : Form
    {

        private static readonly string REGISTRY_KEY = "Software\\S3Sync\\LocalPath";
        private static bool boolContextFormClose = false;
        private static string strLocalPath = "";
        private static S3Connection S3Conn = null;
        private static S3Bucket Bucket = null;
        private static string BucketName = null;
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);


        public S3Notify()
        {
            InitializeComponent();
        }


        private void S3Notify_Load(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser;
            RegistryKey rkOpen = rk.OpenSubKey(REGISTRY_KEY);
            if (rkOpen != null)
            {
                if (rkOpen.GetValue("LocalPath") != null)
                    txtLocalPath.Text = rkOpen.GetValue("LocalPath").ToString();
                if (rkOpen.GetValue("AWSID") != null)
                    txtAWSID.Text = rkOpen.GetValue("AWSID").ToString();
                if (rkOpen.GetValue("AWSKey") != null)
                    txtAWSKey.Text = rkOpen.GetValue("AWSKey").ToString();
                if (rkOpen.GetValue("Bucket") != null)
                    txtBucket.Text = rkOpen.GetValue("Bucket").ToString();
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TopMost = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void S3Notify_FormClosing(object sender, FormClosingEventArgs e)
        {
            //If the flag is "false", then it means that it is not raised from context menu and it is raised by clicking X button in the form
            if (boolContextFormClose == false)
            {
                e.Cancel = true;
                TopMost = false;
                this.WindowState = FormWindowState.Minimized;
                showNotifyMessage("Amazon S3 Sync [Beta] running ...", "Amazon S3 Sync is still running in background.\nDouble click me to open setting window again.");
            }
            //Otherwise the close event is raised from context menu and ask the user whether he wants to close the application
            else
            {
                if (MessageBox.Show("Do you want to close \"Amazon S3 Sync\"?", "Amazon S3 Sync", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                
            }
        }

        private void menuSetting_Click(object sender, EventArgs e)
        {
            TopMost = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            boolContextFormClose = true;
            Application.Exit();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if ((txtLocalPath.Text.Length == 0) || (txtAWSID.Text.Length == 0)
                 || (txtAWSKey.Text.Length == 0) || (txtBucket.Text.Length == 0))
            {
                MessageBox.Show("Please input the AWS information, Bucket Name, and Local Path.", "Please define AWS information, Bucket name, Local Path ...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            RegistryKey rk = Registry.CurrentUser;
            RegistryKey rkOpen = rk.CreateSubKey(REGISTRY_KEY);
            rkOpen.SetValue("LocalPath", txtLocalPath.Text);
            rkOpen.SetValue("AWSID", txtAWSID.Text);
            rkOpen.SetValue("AWSKey", txtAWSKey.Text);
            rkOpen.SetValue("Bucket", txtBucket.Text);
            BucketName = txtBucket.Text;
            strLocalPath = txtLocalPath.Text;
            //Create the directory if it is not created
            if (!Directory.Exists(strLocalPath))
            {
                try
                {
                    Directory.CreateDirectory(strLocalPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            TopMost = false;
            this.WindowState = FormWindowState.Minimized;
            notifyIcon.Text = "Amazon S3 Sync [running]";

            // Sync the directory first.
            S3Conn = new S3Connection(txtAWSID.Text, txtAWSKey.Text);
            Bucket = new S3Bucket(S3Conn, BucketName);

            try
            {
                CheckBucket();
                SyncS3();
                fileSystemWatcher.Path = strLocalPath;
            }
            catch (ArgumentException ae)
            {
                showErrorMessage("Error", ae.Message);
            }
            catch (Exception ee)
            {
                showErrorMessage("Error", ee.Message);
            }
            btnStart.Enabled = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            notifyIcon.Text = "Amazon S3 Sync [not running]";
            boolContextFormClose = true;
            Application.Exit();
        }

        private void localDirBrowser_Click(object sender, EventArgs e)
        {
            try
            {
                //If path is already defined, then set the selected path for folderBrowserDialog control
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey rkOpen = rk.OpenSubKey(REGISTRY_KEY);
                if (rkOpen != null)
                {
                    this.folderBrowserDialog.SelectedPath = rkOpen.GetValue("LocalPath").ToString();
                }
                //If OK is selected then display the path in textbox and update the Registry Key
                folderBrowserDialog.Description = "Please select the path for sync.";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    txtLocalPath.Text = folderBrowserDialog.SelectedPath;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckBucket()
        {
            // If the bucket doesn't exist, then create it.
            if (!Bucket.Exists)
            {
                try
                {
                    Bucket.Create();
                    showNotifyMessage("Bucket Created", "Bucket \"" + BucketName + "\" created successfully.");
                }
                catch (Exception ex)
                {
                    showErrorMessage("Error", ex.Message);
                    return;
                }
            }
        }

        private void SyncS3()
        {
            #region init sync file
            Utils.ListDirectory(strLocalPath);
            foreach (var S3Obj in Bucket.Keys)
            {
                string StrPathName = S3Obj.Key.Replace("/", "\\");
                string StrFileName = strLocalPath + "\\" + S3Obj.Key;

                FileInfo FiLocal = new FileInfo(StrFileName);
                // Make sure the directory is already created.
                if (!Directory.Exists(FiLocal.DirectoryName))
                {
                    try
                    {
                        Directory.CreateDirectory(FiLocal.DirectoryName);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    showNotifyMessage("Directory Created", "Directory \"" + FiLocal.DirectoryName + "\" created successfully.");
                }
                
                // Is a directory, skip it.
                if (S3Obj.Key.EndsWith("/"))
                {
                    Utils.AlDirectories.Remove(FiLocal.DirectoryName);
                    continue;
                }

                if (!FiLocal.Exists)
                {
                    S3Obj.Get(FiLocal.FullName);
                    showNotifyMessage("File Downloaded", FiLocal.FullName);
                }
                else
                {
                    Utils.AlFiles.Remove(FiLocal.FullName);
                    DateTime lastWriteTime = FiLocal.LastWriteTime;
                    if (lastWriteTime > S3Obj.LastModified)
                    {
                        string LocalFileETag = Utils.Md5(StrFileName);
                        if (!LocalFileETag.ToLower().Equals(S3Obj.ETag))
                        {
                            S3Obj.Upload(FiLocal.FullName);
                            showNotifyMessage("S3 File Updated", FiLocal.FullName + ", \nEtag:" + LocalFileETag.ToLower() + "\nS3ETag" + S3Obj.ETag);
                        }
                        continue;
                    }

                    if (lastWriteTime < S3Obj.LastModified)
                    {
                        string LocalFileETag = Utils.Md5(StrFileName);
                        if (!LocalFileETag.ToLower().Equals(S3Obj.ETag))
                        {
                            S3Obj.Get(FiLocal.FullName);
                            showNotifyMessage("Local File Updated", FiLocal.FullName + ", \nEtag:" + LocalFileETag.ToLower() + "\nS3ETag" + S3Obj.ETag);
                        }
                        continue;
                    }
                }
            }
            foreach (var StrPath in Utils.AlDirectories)
            {
                string S3Key = Utils.GetKeyByPath((string)StrPath, strLocalPath);
                S3Key += "/";
                S3Object S3Obj = new S3Object(S3Conn, BucketName, S3Key);
                S3Obj.Upload(StrPath.ToString());
                showNotifyMessage("Upload Directory", (string)StrPath);
            }
            foreach (var StrPath in Utils.AlFiles)
            {
                string S3Key = Utils.GetKeyByPath((string)StrPath, strLocalPath);
                S3Object S3Obj = new S3Object(S3Conn, BucketName, S3Key);
                S3Obj.Upload(StrPath.ToString());
                showNotifyMessage("Upload File", (string)StrPath);
            }
            #endregion
        }

        #region show ballon tip
        private void showNotifyMessage(string title, string message)
        {
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(5);
        }

        private void showErrorMessage(string title, string message)
        {
            notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(5);
        }
        #endregion

        #region file system watcher event
        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            string S3Key = Utils.GetKeyByPath(e.FullPath, strLocalPath);
            string FileType = "File";
            if (Directory.Exists(e.FullPath))
            {
                S3Key += "/";
                FileType = "Directory";
                if (e.ChangeType.ToString() == "Changed")
                    return;
            }
            try
            {
                S3Object S3Obj = new S3Object(S3Conn, BucketName, S3Key);
                S3Obj.Upload(e.FullPath);
                this.showNotifyMessage(FileType + " " + e.ChangeType, S3Key);
            }
            catch (Exception ee)
            {
                showErrorMessage("Error", ee.Message);
            }
        }

        private void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string S3Key = Utils.GetKeyByPath(e.FullPath, strLocalPath);
            try
            {
                S3Object S3Obj = new S3Object(S3Conn, BucketName, S3Key);
                S3Obj.Delete();
                Bucket.DeleteDirectory(S3Key + "/");
                this.showNotifyMessage("File or Directory Deleted", e.FullPath);
            }
            catch (Exception ee)
            {
                showErrorMessage("Error", ee.Message);
            }
        }

        private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            string S3Key = Utils.GetKeyByPath(e.FullPath, strLocalPath);
            string FileType = "File";
            if (Directory.Exists(e.FullPath))
            {
                S3Key += "/";
                FileType = "Directory";
            }
            try
            {
                if (FileType != "Directory")
                {
                    S3Object S3Obj = new S3Object(S3Conn, BucketName, Utils.GetKeyByPath(e.OldFullPath, strLocalPath));
                    S3Obj.Delete();
                    S3Obj = new S3Object(S3Conn, BucketName, S3Key);
                    S3Obj.Upload(e.FullPath);
                }
                else
                {
                    Bucket.DeleteDirectory(S3Key);
                    Utils.ListDirectory(e.FullPath);
                    foreach (var StrPath in Utils.AlDirectories)
                    {
                        string S3KeyOfDir = Utils.GetKeyByPath((string)StrPath, strLocalPath);
                        S3KeyOfDir += "/";
                        S3Object S3Obj = new S3Object(S3Conn, BucketName, S3KeyOfDir);
                        S3Obj.Upload(StrPath.ToString());
                        showNotifyMessage("Upload Directory", (string)StrPath);
                    }
                    foreach (var StrPath in Utils.AlFiles)
                    {
                        string S3KeyOfDir = Utils.GetKeyByPath((string)StrPath, strLocalPath);
                        S3Object S3Obj = new S3Object(S3Conn, BucketName, S3KeyOfDir);
                        S3Obj.Upload(StrPath.ToString());
                        showNotifyMessage("Upload File", (string)StrPath);
                    }
                }
                this.showNotifyMessage(FileType + " Renamed", S3Key);
            }
            catch (Exception ee)
            {
                showErrorMessage("Error", ee.Message);
            }
        }
        #endregion

        private void manuallySyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SyncS3();
        }
    }
        
}
