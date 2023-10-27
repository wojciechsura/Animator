using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Utilities
{
    public class PartialBitmapBuffer : BitmapBuffer
    {
        public PartialBitmapBuffer(Bitmap bitmap, Graphics graphics, Point location) 
            : base(bitmap, graphics, null)
        {
            Location = location;
        }

        public Point Location { get; }
    }
}
