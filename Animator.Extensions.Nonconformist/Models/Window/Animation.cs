using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Animator.Extensions.Nonconformist.Models.Window
{
    [XmlRoot("Animation")]
    public class Animation
    {
        [XmlAttribute]
        public string ShowStart { get; set; } = "0:0:0.0";

        [XmlAttribute]
        public string ShowEnd { get; set; } = "0:0:1.0";
    }
}
