using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Fr.Zhou.S3
{
    class Utils
    {
        public static ArrayList AlFiles = new ArrayList();
        public static ArrayList AlDirectories = new ArrayList();

        public static void ListDirectory(string StrBaseDir)
        {
            DirectoryInfo Di = new DirectoryInfo(StrBaseDir);
            DirectoryInfo[] DiS = Di.GetDirectories();
            FileInfo[] FiDir = Di.GetFiles();
            for (int i = 0; i < FiDir.Length; i++)
                AlFiles.Add(FiDir[i].FullName);
            
            for (int i = 0; i < DiS.Length; i++)
            {
                AlDirectories.Add(DiS[i].FullName);
                ListDirectory(DiS[i].FullName);
            }
        }

        public static bool IsDirectory(string path)
        {
            if (path.EndsWith("/"))
                return true;
            else
                return false;
        }

        public static string GetKeyByPath(string FilePath, string BasePath)
        {
            FileInfo FiLocal = new FileInfo(FilePath);
            FileInfo FiBase = new FileInfo(BasePath + "\\");
            string FiLocalDir = FiLocal.DirectoryName;
            string FiBaseDir = FiBase.DirectoryName;
            string S3Key = FiLocal.FullName.Substring(FiBaseDir.Length + 1);

            S3Key = S3Key.Replace("\\", "/");
            return S3Key;
        }

        public static string Md5(string path)
        {
            try
            {
                FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(get_file);

                string resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "");
                get_file.Close();
                return resule;
            }
            catch (Exception e)
            {

                return e.ToString();

            }
        }
    }
}
