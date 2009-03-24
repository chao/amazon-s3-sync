using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Affirma.ThreeSharp;
using Affirma.ThreeSharp.Model;
using System.Xml;
using Affirma.ThreeSharp.Query;

namespace Fr.Zhou.S3
{
    public class S3Connection
    {
        ThreeSharpConfig m_config;
        IThreeSharp m_service;
        public IThreeSharp Service { get { return m_service; } }

        public S3Connection(String awsAccessKeyId, String awsSecretAccessKey)
        {
            m_config = new ThreeSharpConfig();
            m_config.AwsAccessKeyID = awsAccessKeyId;
            m_config.AwsSecretAccessKey = awsSecretAccessKey;
            m_config.IsSecure = false;

            m_service = new ThreeSharpQuery(m_config);
        }
    }
}
