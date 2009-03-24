using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Affirma.ThreeSharp.Model;
using Affirma.ThreeSharp;

namespace Fr.Zhou.S3
{
    public class S3Object : S3BaseType
    {
        string m_key;
        public string Key { get { return m_key; } }

        int m_size;
        public int Size { get { return m_size; } }
        
        string m_eTag;
        public string ETag { get { return m_eTag; } }

        DateTime m_lastModified;
        public DateTime LastModified { get { return m_lastModified; } }

        string m_buckname;

        /// <summary>
        /// Construct an S3Object from the XML result of a GET request on a S3Bucket.
        /// </summary>
        internal S3Object(S3Connection connection, string BucketName, XmlNode node)
            : base(connection)
        {
            m_key = node.SelectSingleS3String("s3:Key");
            m_eTag = node.SelectSingleS3String("s3:ETag");
            m_size = int.Parse(node.SelectSingleS3String("s3:Size"));
            m_lastModified = node.SelectSingleS3Date("s3:LastModified");
            m_buckname = BucketName;
        }

        internal S3Object(S3Connection connection, string BucketName, string Key)
            : base(connection)
        {
            m_key = Key;
            m_buckname = BucketName;
        }

        public void S3ObjectDownload(string DistLocation)
        {
            ObjectGetResponse response = null;
            try
            {
                ObjectGetRequest request = new ObjectGetRequest(m_buckname, m_key);
                response = Service.ObjectGet(request);
                response.StreamResponseToFile(DistLocation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (response != null && response.DataStream != null)
                    response.DataStream.Close();
            }
        }

        public void S3ObjectUpload(string SourceLocation)
        {
             
            ObjectAddResponse response = null;
            try
            {
                ObjectAddRequest request = new ObjectAddRequest(m_buckname, m_key);
                request.LoadStreamWithFile(SourceLocation);
                response = Service.ObjectAdd(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (response != null && response.DataStream != null)
                    response.DataStream.Close();
            }
        }

        public void S3ObjectDelete()
        {
            ObjectDeleteResponse response = null;
            try
            {
                ObjectDeleteRequest request = new ObjectDeleteRequest(m_buckname, m_key);
                response = Service.ObjectDelete(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (response != null && response.DataStream != null)
                    response.DataStream.Close();
            }
        }

        public override string ToString()
        {
            return m_key;
        }
    }
}
