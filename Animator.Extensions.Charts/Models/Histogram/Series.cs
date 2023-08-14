using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Charts.Models.Histogram
{
    [XmlRoot(ElementName = "Series")]
    public class Series
    {

        [XmlElement(ElementName = "Point")]
        public List<float> Point { get; set; }
    }
}
