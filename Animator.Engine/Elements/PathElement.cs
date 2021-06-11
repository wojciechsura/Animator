using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all path elements.
    /// </summary>
    public abstract class PathElement : SceneElement
    {
        // Protected types ----------------------------------------------------

        /// <summary>
        /// Utility class to simplify handling relative positions
        /// </summary>
        protected class RunningPoint
        {
            private PointF point;

            public RunningPoint(PointF start)
            {
                point = start;
            }

            public PointF Delta(PointF delta)
            {
                point = new PointF(point.X + delta.X, point.Y + delta.Y);
                return point;
            }

            public PointF Current => point;
        }

        // Protected methods --------------------------------------------------

        /// <summary>
        /// Gets position of a point on a cubic Bezier curve
        /// </summary>
        /// <param name="P1">First point</param>
        /// <param name="C1">First control point</param>
        /// <param name="C2">Second control point</param>
        /// <param name="P2">Second point</param>
        /// <param name="t">Position on curve, 0 &lt;= t &lt;= 1</param>
        /// <returns>Point on a curve</returns>
        protected PointF GetPointOnBezier(PointF P1, PointF C1, PointF C2, PointF P2, float t)
        {
            if (t < 0.0f || t > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(t));

            float tRev = 1.0f - t;

            float x = (tRev * tRev * tRev) * P1.X +
                (3 * tRev * tRev * t) * C1.X +
                (3 * tRev * t * t) * C2.X +
                (3 * t * t * t) * P2.X;

            float y = (tRev * tRev * tRev) * P1.Y +
                (3 * tRev * tRev * t) * C1.Y +
                (3 * tRev * t * t) * C2.Y +
                (3 * t * t * t) * P2.Y;

            return new PointF(x, y);
        }

        /// <summary>
        /// Splits Bezier curve into two parts at given point
        /// </summary>
        /// <param name="P1">First point</param>
        /// <param name="C1">First control point</param>
        /// <param name="C2">Second control point</param>
        /// <param name="P2">Second point</param>
        /// <param name="t">Position on curve, 0 &lt;= t &lt;= 1</param>
        /// <returns>Two sets of four control points</returns>
        protected (PointF[], PointF[]) SplitBezier(PointF P1, PointF C1, PointF C2, PointF P2, float t)
        {
            PointF GetMidpoint(PointF P1, PointF P2, float pos)
            {
                return new PointF(P1.X + (P2.X - P1.X) * pos, P1.Y + (P2.Y - P1.Y) * pos);
            }

            if (t < 0.0f || t > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(t));

            var A1 = GetMidpoint(P1, C1, t);
            var A2 = GetMidpoint(C1, C2, t);
            var A3 = GetMidpoint(C2, P2, t);

            var B1 = GetMidpoint(A1, A2, t);
            var B2 = GetMidpoint(A2, A3, t);

            var D1 = GetMidpoint(B1, B2, t);

            return (new PointF[4] { P1, A1, B1, D1 }, new PointF[4] { D1, B2, A3, P2 } );
        }

        /// <summary>
        /// Splits Bezier curve into two parts at given point
        /// </summary>
        /// <param name="bezier">Array consisting of exactly four points</param>
        /// <param name="t">Position on curve, 0 &lt;= t &lt;= 1</param>
        /// <returns>Two sets of four control points</returns>
        protected (PointF[], PointF[]) SplitBezier(PointF [] bezier, float t)
        {
            if (bezier == null)
                throw new ArgumentNullException(nameof(bezier));
            if (bezier.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(bezier));

            PointF GetMidpoint(PointF P1, PointF P2, float pos)
            {
                return new PointF(P1.X + (P2.X - P1.X) * pos, P1.Y + (P2.Y - P1.Y) * pos);
            }

            if (t < 0.0f || t > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(t));

            var A1 = GetMidpoint(bezier[0], bezier[1], t);
            var A2 = GetMidpoint(bezier[1], bezier[2], t);
            var A3 = GetMidpoint(bezier[2], bezier[3], t);

            var B1 = GetMidpoint(A1, A2, t);
            var B2 = GetMidpoint(A2, A3, t);

            var D1 = GetMidpoint(B1, B2, t);

            return (new PointF[4] { bezier[0], A1, B1, D1 }, new PointF[4] { D1, B2, A3, bezier[3] });
        }

        protected string F(float value) => string.Format(CultureInfo.InvariantCulture, "{0:.##}", value);

        // Internal methods -----------------------------------------------------

        internal abstract (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path);

        internal abstract string ToPathString();        

        // Internal properties ------------------------------------------------


    }
}
