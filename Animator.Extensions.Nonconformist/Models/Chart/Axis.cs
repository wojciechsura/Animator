using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class Axis
    {
        [XmlElement]
        public XAxis XAxis { get; set; } = new();

        [XmlElement]
        public YAxis YAxis { get; set; } = new();
    }
}
