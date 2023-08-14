using Animator.Extensions.Nonconformist.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class Series
    {
        [XmlArray]
        public List<Point> Points { get; set; } = new();

        [XmlAttribute]
        public SeriesType Type { get; set; } = SeriesType.Bar;

        [XmlAttribute]
        public string Color { get; set; } = "#60EBF2";

        [XmlAttribute]
        public float LineWidth { get; set; } = 2.0f;
    }
}
