using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class Config
    {
        [XmlElement]
        public Animation Animation { get; set; } = new();

        [XmlElement]
        public Axis Axis { get; set; } = new();

        [XmlAttribute]
        public string ChartBackgroundColor { get; set; } = "#00000000";

        [XmlElement]
        public Data Data { get; set; } = new();

        [XmlElement]
        public Header Header { get; set; } = new();

        [XmlAttribute]
        public float Height { get; set; } = 480;

        [XmlAttribute]
        public float LineWidth { get; set; } = 1.0f;

        [XmlAttribute]
        public float Width { get; set; } = 640;
    }
}
