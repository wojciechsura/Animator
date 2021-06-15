using Animator.Engine.Base;
using Animator.Engine.Utils;
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
        /// <summary>
        /// Defines a threshold for Bezier curve length estimation. If a new
        /// estimation differs by previous less than this value, algorithm
        /// stops.
        /// </summary>
        private const float LENGTH_ESTIMATION_ACCURACY = 0.1f;

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

        protected PointF GetPointOnBezier(PointF[] bezier, float t)
        {
            if (bezier == null)
                throw new ArgumentNullException(nameof(bezier));
            if (bezier.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(bezier));

            return GetPointOnBezier(bezier[0], bezier[1], bezier[2], bezier[3], t);
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
        protected (PointF[] bezier1, PointF[] bezier2) SplitBezier(PointF P1, PointF C1, PointF C2, PointF P2, float t)
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
        protected (PointF[] bezier1, PointF[] bezier2) SplitBezier(PointF[] bezier, float t)
        {
            if (bezier == null)
                throw new ArgumentNullException(nameof(bezier));
            if (bezier.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(bezier));

            return SplitBezier(bezier[0], bezier[1], bezier[2], bezier[3], t);
        }

        /// <summary>
        /// Estimates Bezier curve length by subdividing it into smaller segments.
        /// </summary>
        /// <param name="bezier">Cubic Bezier path points</param>
        /// <returns>Estimated length and equally divided segments, which were used to evaluate it.</returns>
        protected (float length, float[] segments) EstimateBezierLength(PointF[] bezier)
        {
            if (bezier == null)
                throw new ArgumentNullException(nameof(bezier));
            if (bezier.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(bezier));

            int divisions = 1;

            float length = bezier[0].DistanceTo(bezier[3]);
            float[] segments = new float[] { length };
            bool loop = true;

            // Usually around 5-6 subdivisions are needed to estimate length
            // properly.
            do
            {
                divisions <<= 1;

                float[] newSegments = new float[divisions];
                float newLength = 0.0f;

                for (int i = 0; i < divisions; i++)
                {
                    var point1 = GetPointOnBezier(bezier, i / (float)divisions);
                    var point2 = GetPointOnBezier(bezier, (i + 1) / (float)divisions);

                    newSegments[i] = point1.DistanceTo(point2);
                    newLength += newSegments[i];
                }

                if (Math.Abs(newLength - length) > LENGTH_ESTIMATION_ACCURACY)
                {
                    length = newLength;
                    segments = newSegments;
                }
                else
                    loop = false;
            }
            while (loop);

            return (length, segments);
        }

        /// <summary>
        /// Formats a float into a SVG path-compatible string.
        /// </summary>
        protected string F(float value) => string.Format(CultureInfo.InvariantCulture, "{0:.##}", value);

        // Internal methods -----------------------------------------------------

        internal abstract (float length, PointF endPoint, PointF lastControlPoint) EvalLength(PointF start, PointF lastControlPoint);

        internal abstract (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path);

        internal abstract (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path, float? cutFrom, float? cutTo);

        internal abstract string ToPathString();        
    }
}
