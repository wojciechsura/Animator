using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Chart
{
    [XmlRoot]
    public class Animation
    {
        [XmlAttribute]
        public string Start { get; set; } = "0:0:0.0";

        [XmlAttribute]
        public string End { get; set; } = "0:0:5.0";

        [XmlAttribute]
        public string FadeDuration { get; set; } = "0:0:1.0";

        [XmlElement("NextSeriesAt")]
        public List<string> SeriesSwitchTimes { get; set; } = new();
    }
}
