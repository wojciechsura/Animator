using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a quadratic Bezier curve.
    /// Start point of the curve equals to the last point of previous
    /// path element. The control point is deduced from the previous
    /// path element as a mirror of its last control point against its
    /// endpoint.
    /// End point is expressed in relative coordinates.
    /// </summary>
    public class RelativeQuadraticShorthandBezierPathElement : QuadraticBezierPathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            var delta = start.Subtract(lastControlPoint);
            var controlPoint = start.Add(delta);

            var end = start.Add(DeltaEndPoint);

            (var controlPoint1, var controlPoint2) = EstimateCubicControlPoints(start, controlPoint, end);

            path.AddBezier(start, controlPoint1, controlPoint2, end);

            return (end, controlPoint);
        }

        internal override string ToPathString() => $"t {F(DeltaEndPoint.X)} {F(DeltaEndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region DeltaEndPoint managed property

        /// <summary>
        /// End point of the curve, relative to end point of the previous
        /// path element.
        /// </summary>
        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeQuadraticShorthandBezierPathElement),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}