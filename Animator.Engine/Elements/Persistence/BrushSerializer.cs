using Animator.Engine.Base;
using Animator.Engine.Exceptions;
using Animator.Engine.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Persistence
{
    public class BrushSerializer : TypeSerializer
    {
        public override bool CanDeserialize(string value)
        {
            return TypeSerialization.CanDeserialize(value, typeof(System.Drawing.Color));
        }

        public override object Deserialize(string data)
        {
            if (TypeSerialization.CanDeserialize(data, typeof(System.Drawing.Color)))
            {
                var color = (System.Drawing.Color)TypeSerialization.Deserialize(data, typeof(System.Drawing.Color));

                var result = new SolidBrush
                {
                    Color = color
                };

                return result;
            }

            throw new ParseException($"Shorthand notation for property of type Brush allows only converting a color to SolidBrush, but {data} is not a valid color definition.");
        }

        public override bool CanSerialize(object obj)
        {
            // Only solid brushes (for now) can be stored as a shorthand property
            return obj is SolidBrush;
        }

        public override string Serialize(object obj)
        {
            var solidBrush = (SolidBrush)obj;

            return TypeSerialization.Serialize(solidBrush.Color);
        }
    }
}
