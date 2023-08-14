using Animator.Engine.Base;
using Animator.Engine.Elements;
using Animator.Extensions.Charts.Models.Histogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Animator.Extensions.Charts
{
    public class Histogram : BaseGenerator
    {
        public override ManagedObject Generate(XmlNode node)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HistogramModel));
            var reader = new StringReader(node.OuterXml);
            HistogramModel model = (HistogramModel)serializer.Deserialize(reader);

            var result = new Rectangle()
            {
                Brush = new SolidBrush
                {
                    Color = System.Drawing.Color.FromArgb(255, 255, 0, 0)
                },
                Position = new System.Drawing.PointF(10, 10),
                Width = 20,
                Height = 20
            };

            return result;
        }
    }
}
