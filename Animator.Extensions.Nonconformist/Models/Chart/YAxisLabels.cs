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
    public class YAxisLabels
    {
        // Attributes ---------------------------------------------------------

        [XmlAttribute]
        public bool Show { get; set; } = true;

        [XmlAttribute]
        public float AreaWidth { get; set; } = 100.0f;

        [XmlAttribute]
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Right;

        [XmlAttribute]
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

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

        [XmlAttribute]
        public string NumericFormat { get; set; } = "#";

        [XmlAttribute]
        public string LabelFormat { get; set; } = "{0}";

        [XmlAttribute]
        public bool UseSIRounding { get; set; } = false;

        [XmlAttribute]
        public int Skip { get; set; } = 1;

    }
}
