using System;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class YAxis
    {
        [XmlAttribute]
        public float Scale { get; set; } = 1.0f;

        [XmlAttribute]
        public string Color { get; set; } = "#60EBF2";

        [XmlElement]
        public YAxisLabels Labels { get; set; } = new();
    }
}