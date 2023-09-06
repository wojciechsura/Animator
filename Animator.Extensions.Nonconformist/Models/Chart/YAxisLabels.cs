using Animator.Engine.Elements.Types;
using Animator.Extensions.Nonconformist.Types.Chart;
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
        public bool Bold { get; set; } = false;

        [XmlAttribute]
        public bool Italic { get; set; } = false;

        [XmlAttribute]
        public bool Underline { get; set; } = false;

        [XmlAttribute]
        public string Color { get; set; } = "#60EBF2";

        [XmlAttribute]
        public float Margin { get; set; } = 20.0f;

        [XmlAttribute]
        public string NumericFormat { get; set; } = "#";

        [XmlAttribute]
        public string LabelFormat { get; set; } = "{0}";

        [XmlAttribute]
        public RoundingType Rounding { get; set; } = RoundingType.None;

        [XmlAttribute]
        public float Skip { get; set; } = 1;

    }
}
