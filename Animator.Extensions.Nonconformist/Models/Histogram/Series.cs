using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Histogram
{
    [XmlRoot]
    public class Series
    {
        [XmlArray]
        public List<Point> Points { get; set; } = new();
    }
}
