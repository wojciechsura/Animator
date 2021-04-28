using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Documentation
{
    static class Documentation
    {
        private static Dictionary<string, string> loadedXmlDocumentation = new Dictionary<string, string>();

        private static string XmlDocumentationKeyHelper(
            string typeFullNameString,
            string memberNameString)
        {
            string key = Regex.Replace(typeFullNameString, @"\[.*\]", string.Empty).Replace('+', '.');
            
            if (memberNameString != null)
                key += "." + memberNameString;

            return key;
        }

        public static XmlDocument GetDocumentation(this Type type)
        {
            string key = "T:" + XmlDocumentationKeyHelper(type.FullName, null);
            loadedXmlDocumentation.TryGetValue(key, out string documentation);

            if (documentation != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(documentation);
                return doc;
            }
            else
                return null;
        }

        public static XmlDocument GetDocumentation(this PropertyInfo propertyInfo)
        {
            string key = "P:" + XmlDocumentationKeyHelper(propertyInfo.DeclaringType.FullName, propertyInfo.Name);
            loadedXmlDocumentation.TryGetValue(key, out string documentation);
            
            if (documentation != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(documentation);
                return doc;
            }
            else
                return null;
        }

        public static void LoadXmlDocumentation(string xmlDocumentation)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlDocumentation);

            var members = doc["doc"]["members"];

            foreach (XmlElement element in members.ChildNodes
                .OfType<XmlElement>()
                .Where(e => e.Name == "member"))
            {
                loadedXmlDocumentation[element.Attributes["name"].InnerText] = element.OuterXml;
            }
        }
    }
}
