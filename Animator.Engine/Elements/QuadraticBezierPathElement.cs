using Animator.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base abstract class for all quadratic Bezier path elements.
    /// </summary>
    public abstract class QuadraticBezierPathElement : PathElement
    {
        protected (PointF controlPoint1, PointF controlPoint2) EstimateCubicControlPoints(PointF start, PointF controlPoint, PointF end)
        {
            var delta1 = controlPoint.Subtract(start);
            PointF controlPoint1 = start.Add(delta1.Multiply(2.0f / 3.0f));

            var delta2 = end.Subtract(controlPoint);
            PointF controlPoint2 = end.Add(delta2.Multiply(2.0f / 3.0f));

            return (controlPoint1, controlPoint2);
        }
    }
}
