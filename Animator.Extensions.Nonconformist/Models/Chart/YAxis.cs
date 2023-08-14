using System;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class YAxis
    {
        [XmlAttribute]
        public bool ShowLabels { get; set; } = true;

        [XmlAttribute]
        public float LabelAreaWidth { get; set; } = 100.0f;

        [XmlAttribute]
        public float Scale { get; set; } = 1.0f;

        [XmlAttribute]
        public int Skip { get; set; } = 1;

        [XmlAttribute]
        public string Color { get; set; } = "#60EBF2";
    }
}