using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Affirma.ThreeSharp;

namespace Fr.Zhou.S3
{
    public abstract class S3BaseType
    {
        S3Connection m_connection;
        protected S3Connection Connection { get { return m_connection; } }
        protected IThreeSharp Service { get { return m_connection.Service; } }

        public S3BaseType(S3Connection connection)
        {
            m_connection = connection;
        }
    }
}
