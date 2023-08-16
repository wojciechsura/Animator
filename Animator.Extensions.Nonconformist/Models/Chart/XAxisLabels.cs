using Animator.Engine.Elements.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class XAxisLabels
    {
        // Attributes ---------------------------------------------------------

        [XmlAttribute]
        public bool Show { get; set; } = true;

        [XmlAttribute]
        public float AreaHeight { get; set; } = 100.0f;

        [XmlAttribute]
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;

        [XmlAttribute]
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;

        [XmlAttribute]
        public float Angle { get; set; } = 0;

        [XmlAttribute]
        public string FontFamily { get; set; } = "Montserrat";

        [XmlAttribute]
        public float FontSize { get; set; } = 20.0f;

        [XmlAttribute]
        public string Color { get; set; } = "#60EBF2";

        [XmlAttribute]
        public float Margin { get; set; } = 20.0f;

        // Elements -----------------------------------------------------------

        [XmlElement("Label")]
        public List<string> Labels { get; set; } = new();
    }
}
