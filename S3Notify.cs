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

            // Sync the directory first.
            S3Conn = new S3Connection(txtAWSID.Text, txtAWSKey.Text);
            Bucket = new S3Bucket(S3Conn, txtBucket.Text);

            try
            {
                initSync();
                fileSystemWatcher.Path = strLocalPath;
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(ae.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            TopMost = false;
            this.WindowState = FormWindowState.Minimized;
            notifyIcon.Text = "Amazon S3 Sync [running]";
        }
        private void menuSetting_Click(object sender, EventArgs e)
        {
            TopMost = true;
            this.WindowState = FormWindowState.Normal;
        }
        private void menuExit_Click(object sender, EventArgs e)
        {
            boolContextFormClose = true;
            Application.Exit();
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

                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.BalloonTipTitle = "Amazon S3 Sync [Beta] running ...";
                notifyIcon.BalloonTipText = "Amazon S3 Sync is still running in background.\nDouble click me to open setting window again.";
                notifyIcon.ShowBalloonTip(5);
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            notifyIcon.Text = "Amazon S3 Sync [not running]";
            boolContextFormClose = true;
            Application.Exit();
        }

        private void showNotifyMessage(string title, string message)
        {
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(5);
        }

        private void initSync()
        {
            // If the bucket doesn't exist, then create it.
            if (!Bucket.Exists)
            {
                try
                {
                    Bucket.Create();
                    showNotifyMessage("Bucket Created", "Bucket \"" + txtBucket.Text + "\" created successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            #region init sync file
            // TODO: Upload local new file to S3 server.
            foreach (var S3File in Bucket.Keys)
            {
                string StrFileName = strLocalPath + "\\" + S3File.Key;

                FileInfo FiLocal = new FileInfo(StrFileName);
                if (Utils.IsDirectory(S3File.Key))
                {
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
                    }
                    showNotifyMessage("Directory Created", "Directory \"" + FiLocal.DirectoryName + "\" created successfully.");
                    continue;
                }

                if (!FiLocal.Exists)
                {
                    S3File.S3ObjectDownload(FiLocal.FullName);
                    showNotifyMessage("File Downloaded", FiLocal.FullName);
                }
                else
                {
                    DateTime lastWriteTime = FiLocal.LastWriteTime;
                    if (lastWriteTime > S3File.LastModified)
                    {
                        string LocalFileETag = Utils.Md5(StrFileName);
                        if (!LocalFileETag.ToLower().Equals(S3File.ETag))
                        {
                            S3File.S3ObjectUpload(FiLocal.FullName);
                            showNotifyMessage("S3 File Updated", FiLocal.FullName);
                        }
                        continue;
                    }

                    if (lastWriteTime < S3File.LastModified)
                    {
                        string LocalFileETag = Utils.Md5(StrFileName);
                        if (!LocalFileETag.ToLower().Equals(S3File.ETag))
                        {
                            S3File.S3ObjectDownload(FiLocal.FullName);
                            showNotifyMessage("Local File Updated", FiLocal.FullName);
                        }
                        continue;
                    }
                }
            }
            #endregion
        }

        private static string getKey(string FilePath)
        {
            FileInfo FiLocal = new FileInfo(FilePath);
            FileInfo FiBase = new FileInfo(strLocalPath + "\\");
            string FiLocalDir = FiLocal.DirectoryName;
            string FiBaseDir = FiBase.DirectoryName;
            string S3Key = FiLocal.FullName.Substring(FiBaseDir.Length + 1);
            
            S3Key = S3Key.Replace("\\", "/");
            return S3Key;
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                this.showNotifyMessage("File " + e.ChangeType + ":", "TODO:" + e.FullPath);
                return;
            }
            try
            {
                S3Object S3Obj = new S3Object(S3Conn, txtBucket.Text, getKey(e.FullPath));
                S3Obj.S3ObjectUpload(e.FullPath);
                this.showNotifyMessage("File Uploaded", getKey(e.FullPath));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                this.showNotifyMessage("File " + e.ChangeType + ":", "TODO:" + e.FullPath);
                return;
            }
            try
            {

                S3Object S3Obj = new S3Object(S3Conn, txtBucket.Text, getKey(e.FullPath));
                S3Obj.S3ObjectDelete();
                this.showNotifyMessage("File Deleted", getKey(e.FullPath));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                this.showNotifyMessage("File " + e.ChangeType + ":", "TODO:" + e.FullPath);
                return;
            }
            try
            {
                S3Object S3Obj = new S3Object(S3Conn, txtBucket.Text, getKey(e.OldFullPath));
                S3Obj.S3ObjectDelete();
                S3Obj = new S3Object(S3Conn, txtBucket.Text, getKey(e.FullPath));
                S3Obj.S3ObjectUpload(e.FullPath);
                this.showNotifyMessage("File Updated", getKey(e.FullPath));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //this.showNotifyMessage("File " + e.ChangeType + ":", "From " + e.OldFullPath + " to " + e.FullPath + " [" + DateTime.Now.ToLongTimeString() + "]");
        }
    }
}
