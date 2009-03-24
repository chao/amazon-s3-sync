using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fr.Zhou.S3
{
    class Utils
    {
        public static bool IsDirectory(string path)
        {
            if (path.EndsWith("/"))
                return true;
            else
                return false;
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
                return resule;
            }
            catch (Exception e)
            {

                return e.ToString();

            }
        }
    }
}
