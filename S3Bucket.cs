using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Affirma.ThreeSharp.Model;
using Affirma.ThreeSharp;
using System.Diagnostics;
using System.Net;

namespace Fr.Zhou.S3
{
    public class S3Bucket : S3BaseType
    {
        string m_name;
        string m_redirectUrl = null;
        bool m_headed = false;

        public string Name { get { return m_name; } }

        internal S3Bucket(S3Connection connection, string name)
            : base(connection)
        {
            m_name = name;
        }

        public bool Exists
        {
            get
            {
                try
                {
                    BucketListRequest request = new BucketListRequest(m_name);
                    request.Method = "HEAD";
                    BucketListResponse response = Service.BucketList(request);
                    response.DataStream.Close();
                    if (response.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect)
                    {
                        m_redirectUrl = response.Headers["Location"].ToString();
                        m_headed = true;
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    if (ex is WebException)
                        return false;
                    else
                        throw ex;
                }
            }
        }

        public IEnumerable<S3Object> Keys
        {
            get
            {
                BucketListRequest request = null;
                BucketListResponse response = null;
                try
                {
                    if (!m_headed)
                    {
                        BucketListRequest testRequest = new BucketListRequest(m_name);
                        testRequest.Method = "HEAD";
                        BucketListResponse testResponse = Service.BucketList(testRequest);
                        testResponse.DataStream.Close();

                        if (testResponse.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect)
                        {
                            m_redirectUrl = testResponse.Headers["Location"].ToString();
                        }
                    }
                    bool isTruncated = true;
                    string marker = string.Empty;

                    // The while-loop is here because S3 will only return a maximum of 1000 items at a time, so if the list
                    // was truncated, we need to make another call and get the rest
                    while (isTruncated)
                    {
                        request = new BucketListRequest(m_name);
                        request.RedirectUrl = m_redirectUrl;
                        if (!string.IsNullOrEmpty(marker))
                        {
                            request.QueryList.Add("marker", marker);
                        }
                        response = Service.BucketList(request);
                        XmlDocument xdoc = response.StreamResponseToXmlDocument();
                        Debug.WriteLine(xdoc);

                        XmlNode responseNode = xdoc.SelectSingleS3Node("/s3:ListBucketResult");

                        foreach (var objNode in responseNode.SelectS3Nodes("s3:Contents"))
                        {
                            S3Object obj = new S3Object(Connection, m_name, objNode);
                            yield return obj;
                            marker = obj.Key;
                        }

                        isTruncated = bool.Parse(responseNode.SelectSingleS3String("s3:IsTruncated"));
                    }
                }
                finally
                {
                    if (response != null && response.DataStream != null) response.DataStream.Close();
                }
            }
        }

        public void Create()
        {
            try
            {
                BucketAddRequest request = new BucketAddRequest(m_name);
                BucketAddResponse response = Service.BucketAdd(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override string ToString()
        {
            return m_name;
        }
    }
}
