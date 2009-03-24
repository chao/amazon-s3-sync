using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Fr.Zhou.S3
{
    static class Keys
    {
        public static readonly string AwsAccessKeyId = "1S02BY3BSPRME2AGK402";
        public static readonly string AwsSecretAccessKey = "njYNT4hmLPFeCsoQjRaqH1/W1RmJB5IiaO9uz97/";
    }

    class S3Test
    {
        static void Main(string[] args)
        {
            string BucketName = "steatite";
            string StrLocalPath = "c:\\testing";
            S3Connection S3Conn = new S3Connection(Keys.AwsAccessKeyId, Keys.AwsSecretAccessKey);
            S3Bucket bucket = new S3Bucket(S3Conn, BucketName);
            if (!bucket.Exists)
            {
                Console.WriteLine("Bucket {0} doesn't exist!", BucketName);
                try
                {
                    bucket.Create();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
                return;
            }


            #region init sync file
            foreach (var S3File in bucket.Keys)
            {
                string StrFileName = StrLocalPath + "\\" + S3File.Key;

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
                    Console.WriteLine(FiLocal.DirectoryName);
                    continue;
                }

                if (!FiLocal.Exists)
                {
                    S3File.S3ObjectDownload(FiLocal.FullName);
                    Console.WriteLine("Download New File");
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
                            Console.WriteLine("Upload Overwrite");
                        }
                        continue;
                    }
                    if (lastWriteTime < S3File.LastModified)
                    {
                        string LocalFileETag = Utils.Md5(StrFileName);
                        if (!LocalFileETag.ToLower().Equals(S3File.ETag))
                        {
                            S3File.S3ObjectDownload(FiLocal.FullName);
                            Console.WriteLine("Download Overwrite");
                        }
                        continue;
                    }
                }
            }
            #endregion


            Console.ReadLine();
        }


    }
}