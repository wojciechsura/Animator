using Animator.Engine.Elements.Types;
using System;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class XAxis
    {
        [XmlElement]
        public XAxisLabels Labels { get; set; } = new();
      
        [XmlAttribute]
        public float BarScale { get; set; } = 0.75f;

        [XmlAttribute]
        public string Color { get; set; } = "#60EBF2";
    }
}