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
    /// by a cubic Bezier curve
    /// </summary>
    public abstract class CubicBezierBasedPathElement : PathElement
    {
        // Private fields -----------------------------------------------------

        private bool lengthValid = false;
        private float length;
        private float[] lengthSegments;

        private bool bezierValid;
        private PointF[] bezier;

        private PointF cachedStart;
        private PointF cachedLastControlPoint;

        // Private methods ----------------------------------------------------

        private void NotifyCurveChanged()
        {
            // This will invalidate length as well
            InvalidateBezier();
        }

        private void ValidateBezier(PointF start, PointF lastControlPoint)
        {
            bezier = BuildBezier(start, lastControlPoint);
            if (bezier.Length != 4)
                throw new InvalidOperationException("Invalid implementation of BuildBezier: should return array of length 4!");
            bezierValid = true;

            InvalidateLength();
        }

        private void ValidateLength(PointF start, PointF lastControlPoint)
        {
            ValidateBezier(start, lastControlPoint);

            (length, lengthSegments) = EstimateBezierLength(bezier);
            lengthValid = true;
        }

        // Protected methods --------------------------------------------------

        protected void InvalidateLength()
        {
            length = float.NaN;
            lengthSegments = null;
            lengthValid = false;
        }

        protected void InvalidateBezier()
        {
            bezierValid = false;
            bezier = null;
            InvalidateLength();
        }

        /// <summary>
        /// Builds cubic Bezier curve equivalent for current path fragment.
        /// </summary>
        protected abstract PointF[] BuildBezier(PointF start, PointF lastControlPoint);

        /// <summary>
        /// Gets cubic Bezier curve equivalent for current path fragment.
        /// </summary>
        protected PointF[] GetBezier(PointF start, PointF lastControlPoint)
        {
            // Note: floating point equality is on purpose.
            // That's because we're not relying on floating point
            // operation precision, but on their consistency.
            if (!bezierValid || start != cachedStart || lastControlPoint != cachedLastControlPoint)
            {
                ValidateBezier(start, lastControlPoint);

                cachedStart = start;
                cachedLastControlPoint = lastControlPoint;
            }

            return bezier;
        }

        protected static void HandleCurveChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            if (sender is CubicBezierBasedPathElement cubic)
                cubic.NotifyCurveChanged();
        }

        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            PointF[] bezier = GetBezier(start, lastControlPoint);

            path.AddBezier(bezier[0], bezier[1], bezier[2], bezier[3]);

            return (bezier[3], bezier[2]);
        }

        internal override (float length, PointF endPoint, PointF lastControlPoint) EvalLength(PointF start, PointF lastControlPoint)
        {
            if (!lengthValid)
                ValidateLength(start, lastControlPoint);

            return (length, bezier[3], bezier[2]);
        }
    }
}
