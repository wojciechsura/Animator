using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a quadratic Bezier curve.
    /// First point of the curve equals to the last point of previous
    /// path element. 
    /// All points are expressed in relative coordinates.
    /// </summary>
    public class RelativeQuadraticBezierPathElement : QuadraticBezierPathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            // Note: deltas are evaluated differently in case of this element
            // See: https://developer.mozilla.org/en-US/docs/Web/SVG/Tutorial/Paths#b%C3%A9zier_curves

            var controlPoint = start.Add(DeltaControlPoint);
            var end = start.Add(DeltaEndPoint);

            (var controlPoint1, var controlPoint2) = EstimateCubicControlPoints(start, controlPoint, end);
            path.AddBezier(start, controlPoint1, controlPoint2, end);

            return (end, controlPoint);
        }

        internal override string ToPathString() => $"q {F(DeltaControlPoint.X)} {F(DeltaControlPoint.Y)} {F(DeltaEndPoint.X)} {F(DeltaEndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region DeltaControlPoint managed property

        /// <summary>
        /// Control point of the curve, relative to endpoint of the previous
        /// path element.
        /// </summary>
        public PointF DeltaControlPoint
        {
            get => (PointF)GetValue(DeltaControlPointProperty);
            set => SetValue(DeltaControlPointProperty, value);
        }

        public static readonly ManagedProperty DeltaControlPointProperty = ManagedProperty.Register(typeof(RelativeQuadraticBezierPathElement),
            nameof(DeltaControlPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion


        #region DeltaEndPoint managed property

        /// <summary>
        /// End point of the curve, relative to endpoint of the previous
        /// path element.
        /// </summary>
        /// <remarks>
        /// As an exception, coordinates of end point od the quadratic curve
        /// are expressed in relation to previous path element's end point
        /// and <strong>not</strong> to the control point. This is to conform
        /// to SVG's standard of defining paths.
        /// </remarks>
        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeQuadraticBezierPathElement),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}