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
    /// by a cubic Bezier curve
    /// </summary>
    public abstract partial class BaseCubicBezierBasedSegment : Segment
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

            (length, lengthSegments) = Bezier.EstimateLength(bezier);
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
            if (sender is BaseCubicBezierBasedSegment cubic)
                cubic.NotifyCurveChanged();
        }

        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            PointF[] bezier = GetBezier(start, lastControlPoint);

            if (path != null)
                path.AddBezier(bezier[0], bezier[1], bezier[2], bezier[3]);

            return (bezier[3], bezier[2]);
        }

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path, float? cutFrom, float? cutTo)
        {
            if (cutFrom == null && cutTo == null)
            {
                return AddToGeometry(start, lastControlPoint, path);
            }

            if (!lengthValid)
                ValidateLength(start, lastControlPoint);

            // Factors passed to this method represent fraction of Bezier spline
            // length. However, Bezier splines tend to have varying speeds, so
            // length factors does not always reflect spline factor. This method
            // estimates curve factor based on given length factor.
            float FactorToBezierFactor(float factor)
            {
                var lengthPos = length * factor;

                int i = 0;
                float lengthAcc = 0.0f;

                while (i < lengthSegments.Length && lengthAcc + lengthSegments[i] < lengthPos)
                {
                    lengthAcc += lengthSegments[i];
                    i++;
                }

                if (i >= lengthSegments.Length)
                    return 1.0f;

                // The answer is i-th segment, but we may return a little more precise value
                lengthPos -= lengthAcc;

                return (i + lengthPos / lengthSegments[i]) / lengthSegments.Length;
            }

            if (cutFrom != null && cutTo == null)
            {
                float t = FactorToBezierFactor(cutFrom.Value);

                (_, PointF[] bezier2) = Bezier.Split(bezier, t);

                if (path != null)
                    path.AddBezier(bezier2[0], bezier2[1], bezier2[2], bezier2[3]);
                return (bezier2[3], bezier2[2]);
            }
            else if (cutFrom == null && cutTo != null)
            {
                float t = FactorToBezierFactor(cutTo.Value);

                (PointF[] bezier1, _) = Bezier.Split(bezier, t);

                if (path != null)
                    path.AddBezier(bezier1[0], bezier1[1], bezier1[2], bezier1[3]);
                return (bezier1[3], bezier1[2]);
            }
            else
            {
                float t1 = FactorToBezierFactor(cutFrom.Value);
                float t2 = FactorToBezierFactor(cutTo.Value);

                (_, PointF[] middle, _) = Bezier.Split(bezier, t1, t2);

                if (path != null)
                    path.AddBezier(middle[0], middle[1], middle[2], middle[3]);
                return (middle[3], middle[2]);
            }
        }

        internal override (float length, PointF endPoint, PointF lastControlPoint) EvalLength(PointF start, PointF lastControlPoint)
        {
            if (!lengthValid)
                ValidateLength(start, lastControlPoint);

            return (length, bezier[3], bezier[2]);
        }
    }
}
