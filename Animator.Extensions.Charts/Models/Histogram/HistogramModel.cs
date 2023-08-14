using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Charts.Models.Histogram
{
    [XmlRoot("Histogram", Namespace = "assembly=Animator.Extensions.Charts;namespace=Animator.Extensions.Charts")]
    public class HistogramModel
    {

        [XmlElement(ElementName = "Data")]
        public Data Data { get; set; }
    }
}
