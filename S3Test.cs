using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Fr.Zhou.S3
{
    static class Keys
    {
        private static readonly string REGISTRY_KEY = "Software\\S3Sync\\LocalPath";
        public static string AwsAccessKeyId
        {
            get {
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey rkOpen = rk.OpenSubKey(REGISTRY_KEY);
                if (rkOpen != null && rkOpen.GetValue("AWSID") != null)
                    return rkOpen.GetValue("AWSID").ToString();
                else
                    return "<Your Access Key Id Here>"; 
            }
        }
        public static string AwsSecretAccessKey
        {
            get {
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey rkOpen = rk.OpenSubKey(REGISTRY_KEY);
                if (rkOpen != null && rkOpen.GetValue("AWSKey") != null)
                    return rkOpen.GetValue("AWSKey").ToString();
                else
                    return "<Your Access Key Here>"; 
            }
        }
    }

    class S3Test
    {
        private static string BucketName = "steatite";
        private static string StrLocalPath = "c:\\testing\\";
        static void Main(string[] args)
        {
            SyncToS3();
            Console.ReadLine();
        }

        private static void SyncToS3()
        {
            S3Connection S3Conn = new S3Connection(Keys.AwsAccessKeyId, Keys.AwsSecretAccessKey);
            S3Bucket Bucket = new S3Bucket(S3Conn, BucketName);
            string strLocalPath = "c:\\testing\\";
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
                Console.WriteLine("[local] Upload Directory" + S3Key);
            }
            foreach (var StrPath in Utils.AlFiles)
            {
                string S3Key = Utils.GetKeyByPath((string)StrPath, strLocalPath);
                S3Object S3Obj = new S3Object(S3Conn, BucketName, S3Key);
                S3Obj.Upload(StrPath.ToString());
                Console.WriteLine("[local] Upload Directory" + S3Key);
            }
        }

        private static void S3DirectoryDeleteTest()
        {
            S3Connection S3Conn = new S3Connection(Keys.AwsAccessKeyId, Keys.AwsSecretAccessKey);
            S3Bucket Bucket = new S3Bucket(S3Conn, BucketName);
            foreach (var S3Obj in Bucket.Keys)
                Console.WriteLine(S3Obj.Key + "," + S3Obj.LastModified + "," + S3Obj.Size / 1024 + "KB");
            try
            {
                Bucket.DeleteDirectory("mydir/");
            }
            catch (Exception ee)
            {
                Console.WriteLine("Error:" + ee.Message);
            }
        }

        private static void S3CreateFileInDirectoryTest()
        {
            string S3Key = "mydir/WGAErrLog.txt";
            S3Connection S3Conn = new S3Connection(Keys.AwsAccessKeyId, Keys.AwsSecretAccessKey);
            S3Bucket Bucket = new S3Bucket(S3Conn, BucketName);
            try
            {
                S3Object S3Obj = new S3Object(S3Conn, BucketName, S3Key);
                S3Obj.Upload(StrLocalPath + "\\WGAErrLog.txt");
                Console.WriteLine(S3Key);
            }
            catch (Exception ee)
            {
                Console.WriteLine("Error:" + ee.Message);
            }
        }

        private static void DirectoryListTest()
        {
            string StrBaseDir = "c:\\testing\\";
            FileInfo fi = new FileInfo(StrBaseDir + "\\flickr\\party\\");
            Console.WriteLine(fi.DirectoryName);
            Utils.ListDirectory(StrBaseDir);
            Utils.AlDirectories.Remove(fi.DirectoryName);
            foreach (var StrName in Utils.AlDirectories)
                Console.WriteLine(StrName);
            foreach (var StrName in Utils.AlFiles)
                Console.WriteLine(StrName);

        }

        private static void showNotifyMessage(string title, string message)
        {
            Console.WriteLine(title + ":" + message);
        }
    }
}