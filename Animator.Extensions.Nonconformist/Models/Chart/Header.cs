using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class Header
    {
        [XmlAttribute]
        public bool Show { get; set; } = true;

        [XmlAttribute]
        public string Text { get; set; } = "Header";

        [XmlAttribute]
        public float Height { get; set; } = 100;

        [XmlAttribute]
        public string FontFamily { get; set; } = "Montserrat";

        [XmlAttribute]
        public float FontSize { get; set; } = 30;

        [XmlAttribute]
        public string Color { get; set; } = "#FF8E32";

        [XmlAttribute]
        public bool Bold { get; set; } = false;

        [XmlAttribute]
        public bool Italic { get; set; } = false;

        [XmlAttribute]
        public bool Underline { get; set; } = false;

        [XmlElement]
        public List<string> NextHeader { get; set; } = new();
    }
}
