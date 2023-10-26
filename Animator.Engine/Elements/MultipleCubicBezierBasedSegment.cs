using Animator.Engine.Base;
using Animator.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all path elements, which are interpolated
    /// by a series of cubic Bezier curves
    /// </summary>
    public abstract partial class MultipleCubicBezierBasedSegment : Segment
    {
        // Private methods ----------------------------------------------------

        private bool lengthsValid = false;
        private (float length, float[] segments)[] lengths;

        private bool beziersValid;
        private PointF[][] beziers;

        private PointF cachedStart;

        // Private methods ----------------------------------------------------

        private void NotifyCurveChanged()
        {
            // This will invalidate length as well
            InvalidateBeziers();
        }

        private void ValidateBeziers(PointF start)
        {
            beziers = BuildBeziers(start);
            if (beziers.Length < 1 || beziers.Any(bezier => bezier.Length != 4))
                throw new InvalidOperationException("Invalid implementation of BuildBezier: should return non-empty array of arrays of length 4!");
            beziersValid = true;

            InvalidateLength();
        }

        private void ValidateLength(PointF start, PointF lastControlPoint)
        {
            ValidateBeziers(start);

            lengths = beziers.Select(b => Bezier.EstimateLength(b))
                .ToArray();
            
            lengthsValid = true;
        }

        // Protected methods --------------------------------------------------

        protected void InvalidateLength()
        {
            lengths = null;
            lengthsValid = false;
        }

        protected void InvalidateBeziers()
        {
            beziersValid = false;
            beziers = null;
            
            InvalidateLength();
        }

        /// <summary>
        /// Builds cubic Bezier curve equivalent for current path fragment.
        /// </summary>
        protected abstract PointF[][] BuildBeziers(PointF start);

        /// <summary>
        /// Gets cubic Bezier curve equivalent for current path fragment.
        /// </summary>
        protected PointF[][] GetBeziers(PointF start)
        {
            // Note: floating point equality is on purpose.
            // That's because we're not relying on floating point
            // operation precision, but on their consistency.
            if (!beziersValid || start != cachedStart)
            {
                ValidateBeziers(start);

                cachedStart = start;                
            }

            return beziers;
        }

        protected static void HandleCurveChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            if (sender is MultipleCubicBezierBasedSegment multipleCubic)
                multipleCubic.NotifyCurveChanged();
        }

        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            PointF[][] beziers = GetBeziers(start);

            if (path != null)
                foreach (var bezier in beziers)                
                    path.AddBezier(bezier[0], bezier[1], bezier[2], bezier[3]);

            var lastBezier = beziers.Last();

            return (lastBezier[3], lastBezier[3]);
        }

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path, float? cutFrom, float? cutTo)
        {
            if (cutFrom == null && cutTo == null)
                return AddToGeometry(start, lastControlPoint, path);

            // Given absolute position on series of beziers (0 <= absolutePosition <= 1),
            // this method evaluates bezier and position on it (t) representing that
            // exact absolute posiiton.
            void evalBezierAndT(float? absolutePosition, ref int bezier, ref float t)
            {
                float totalLength = lengths.Sum(l => l.length);

                if (absolutePosition.HasValue)
                {
                    var positionLength = totalLength * absolutePosition.Value;

                    float lengthAcc = 0.0f;
                    int i = 0;
                    while (i < lengths.Length && lengthAcc + lengths[i].length < positionLength)
                    {
                        lengthAcc += lengths[i].length;
                        i++;
                    }

                    // In this case there is nothing to draw (ie. from = 1)
                    if (i == lengths.Length)
                        return;

                    bezier = i;

                    // FromLength now should be smaller than lengths[i]
                    positionLength -= lengthAcc;

                    // Now evaluating position on (element)th bezier
                    lengthAcc = 0.0f;

                    i = 0;
                    while (i < lengths[bezier].segments.Length && lengthAcc + lengths[bezier].segments[i] < positionLength)
                    {
                        lengthAcc += lengths[bezier].segments[i];
                        i++;
                    }

                    if (i >= lengths[bezier].segments.Length)
                    {
                        t = 1.0f;
                        return;
                    }

                    positionLength -= lengthAcc;

                    t = (i + positionLength / lengths[bezier].segments[i]) / lengths[bezier].segments.Length;
                }
            }

            int startBezier = -1;
            float startT = float.NaN;
            int endBezier = beziers.Length;
            float endT = float.NaN;

            evalBezierAndT(cutFrom, ref startBezier, ref startT);
            evalBezierAndT(cutTo, ref endBezier, ref endT);

            // Special case
            if (startBezier == endBezier)
            {
                (_, PointF[] middle, _) = Bezier.Split(beziers[startBezier], startT, endT);

                if (path != null)
                    path.AddBezier(middle[0], middle[1], middle[2], middle[3]);

                return (middle[3], middle[2]);
            }

            PointF? newStart = null;
            PointF? newLastControlPoint = null;

            if (startBezier >= 0)
            {
                (_, PointF[] end) = Bezier.Split(beziers[startBezier], startT);

                if (path != null)
                    path.AddBezier(end[0], end[1], end[2], end[3]);
                newStart = end[3];
                newLastControlPoint = end[2];
            }

            for (int i = startBezier + 1; i < endBezier; i++)
            {
                if (path != null)
                    path.AddBezier(beziers[i][0], beziers[i][1], beziers[i][2], beziers[i][3]);
                newStart = beziers[i][3];
                newLastControlPoint = beziers[i][2];
            }

            if (endBezier >= 0)
            {
                (PointF[] first, _) = Bezier.Split(beziers[endBezier], endT);
                if (path != null)
                    path.AddBezier(first[0], first[1], first[2], first[3]);
                newStart = first[3];
                newLastControlPoint = first[2];
            }

            if (newStart == null || newLastControlPoint == null)
                throw new InvalidOperationException("Bezier selection algorithm failure!");

            return (newStart.Value, newLastControlPoint.Value);
        }

        internal override (float length, PointF endPoint, PointF lastControlPoint) EvalLength(PointF start, PointF lastControlPoint)
        {
            if (!lengthsValid)
                ValidateLength(start, lastControlPoint);

            var lastBezier = beziers.Last();

            return (lengths.Sum(l => l.length), lastBezier[3], lastBezier[3]);
        }
    }
}
