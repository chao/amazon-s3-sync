using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Fr.Zhou.S3
{
    static class S3Utils
    {
        public static IEnumerable<XmlNode> SelectS3Nodes(this XmlNode node, string xpath)
        {
            foreach (XmlNode selectedNode in node.SelectNodes(xpath, GetNS(node)))
            {
                yield return selectedNode;
            }
        }

        public static XmlNode SelectSingleS3Node(this XmlNode node, string xpath)
        {
            return node.SelectSingleNode(xpath, GetNS(node));
        }

        public static string SelectSingleS3String(this XmlNode node, string xpath)
        {
            return node.SelectSingleS3Node(xpath).InnerXml;
        }

        public static DateTime SelectSingleS3Date(this XmlNode node, string xpath)
        {
            return ParseDate(node.SelectSingleS3String(xpath));
        }

        static XmlNamespaceManager GetNS(XmlNode node)
        {
            return node.OwnerDocument == null ? GetNS((XmlDocument)node) : GetNS(node.OwnerDocument);
        }

        static XmlNamespaceManager GetNS(XmlDocument doc)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s3", @"http://s3.amazonaws.com/doc/2006-03-01/");
            return ns;
        }

        static DateTime ParseDate(string dateString)
        {
            return DateTime.ParseExact(dateString, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
                            System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
