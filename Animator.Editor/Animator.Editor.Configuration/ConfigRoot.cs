using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Editor.Configuration
{
    public class ConfigRoot : BaseItemContainer
    {
        // Public methods -----------------------------------------------------

        public ConfigRoot(string name)
            : base(name)
        {

        }

        public void Load(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                Load(fs);
            }
        }

        public void Load(Stream stream)
        {
            XmlDocument document;

            try
            {
                document = new XmlDocument();
                document.Load(stream);
            }
            catch (Exception e)
            {
                throw new FileLoadException("Cannot load configuration file!", e);
            }

            XmlNode root = document[Name];
            if (root != null)
            {
                InternalLoad(root);
            }
            else
            {
                throw new FileLoadException("Invalid configuration file format!");
            }
        }

        public void Save(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                Save(fs);
            }
        }

        public void Save(Stream stream)
        {
            XmlDocument document = new XmlDocument();

            XmlElement root = document.CreateElement(Name);
            document.AppendChild(root);

            InternalSave(root);

            StreamWriter sw = new StreamWriter(stream);
            XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            });

            document.Save(writer);            
        }
    }
}
