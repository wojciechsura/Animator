using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Animator.Extensions.Utils.Serialization
{
    public static class XmlSerializationExtensions
    {
        public static T? Deserialize<T>(this XmlElement element, XmlSerializer? serializer = null)
        {
            using var reader = new ProperXmlNodeReader(element);
            return (T?)(serializer ?? new XmlSerializer(typeof(T))).Deserialize(reader);
        }
    }

    /// <remarks>
    /// Bug fix from this answer https://stackoverflow.com/a/30115691/3744182
    /// By https://stackoverflow.com/users/8799/nathan-baulch
    /// To https://stackoverflow.com/questions/30102275/deserialize-object-property-with-stringreader-vs-xmlnodereader
    /// You may need to test whether this is still necessary, 
    /// </remarks>
    public class ProperXmlNodeReader : XmlNodeReader
    {        
        public ProperXmlNodeReader(XmlNode node) 
            : base(node) 
        { 
        
        }

        public override string? LookupNamespace(string prefix) => 
            base.LookupNamespace(prefix) is { } ns ? NameTable.Add(ns) : null;
    }
}
