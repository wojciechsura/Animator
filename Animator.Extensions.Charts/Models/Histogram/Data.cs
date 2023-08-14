using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Charts.Models.Histogram
{
    [XmlRoot(ElementName = "Data")]
    public class Data
    {

        [XmlElement(ElementName = "Series")]
        public List<Series> Series { get; set; }
    }
}
