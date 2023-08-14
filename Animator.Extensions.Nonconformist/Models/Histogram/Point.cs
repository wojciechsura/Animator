using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Histogram
{
    [XmlRoot]
    public class Point
    {
        [XmlAttribute]
        public float Value { get; set; } = 0.0f;
    }
}
