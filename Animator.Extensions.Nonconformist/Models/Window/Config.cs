using Animator.Engine.Elements;
using Animator.Extensions.Nonconformist.Models.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Window
{
    [XmlRoot]
    public class Config
    {
        [XmlAttribute]
        public float Width { get; set; } = 800.0f;

        [XmlAttribute]
        public float Height { get; set; } = 600.0f;

        [XmlAttribute]
        public float DecorSize { get; set; } = 48.0f;

        [XmlAttribute]
        public float CornerThickness { get; set; } = 6.0f;

        [XmlAttribute]
        public float ContentBorderThickness { get; set; } = 1.0f;

        [XmlAttribute]
        public float InternalMargin { get; set; } = 6.0f;

        [XmlAttribute]
        public bool ShowHeader { get; set; } = true;

        [XmlAttribute]
        public float HeaderWidth { get; set; } = 350.0f;

        [XmlAttribute]
        public float HeaderLineWidth { get; set; } = 200.0f;

        [XmlAttribute]
        public float HeaderHeight { get; set; } = 100.0f;

        [XmlAttribute]
        public float HeaderLineHeight { get; set; } = 200.0f;

        [XmlAttribute]
        public bool ShowFooter { get; set; } = true;

        [XmlAttribute]
        public float FooterWidth { get; set; } = 300.0f;

        [XmlElement]
        public Animation Animation { get; set; } = new();
    }
}
