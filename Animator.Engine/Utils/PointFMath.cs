using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Utils
{
    public static class PointFMath
    {
        public static PointF Subtract(this PointF first, PointF second)
        {
            return new PointF(first.X - second.X, first.Y - second.Y);
        }

        public static PointF Add(this PointF first, PointF second)
        {
            return new PointF(first.X + second.X, first.Y - second.Y);
        }

        public static PointF Multiply(this PointF point, float factor)
        {
            return new PointF(point.X * factor, point.Y * factor);
        }
    }
}
