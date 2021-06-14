using Animator.Engine.Base;
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
    public abstract class MultipleCubicBezierBasedPathElement : PathElement
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

            lengths = beziers.Select(b => EstimateBezierLength(b))
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
            if (sender is MultipleCubicBezierBasedPathElement multipleCubic)
                multipleCubic.NotifyCurveChanged();
        }

        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            PointF[][] beziers = GetBeziers(start);

            foreach (var bezier in beziers)
                path.AddBezier(bezier[0], bezier[1], bezier[2], bezier[3]);

            var lastBezier = beziers.Last();

            return (lastBezier[3], lastBezier[3]);
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
