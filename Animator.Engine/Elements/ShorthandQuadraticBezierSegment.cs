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
    /// End point is expressed in absolute coordinates.
    /// </summary>
    public partial class ShorthandQuadraticBezierSegment : BaseQuadraticBezierSegment
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildBezier(PointF start, PointF lastControlPoint)
        {
            var delta = start.Subtract(lastControlPoint);
            var controlPoint = start.Add(delta);

            (var controlPoint1, var controlPoint2) = EstimateCubicControlPoints(start, controlPoint, EndPoint);

            return new[] { start, controlPoint1, controlPoint2, EndPoint };
        }

        internal override string ToPathString() => $"T {F(EndPoint.X)} {F(EndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region EndPoint managed property

        /// <summary>
        /// End point of the curve.
        /// </summary>
        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(ShorthandQuadraticBezierSegment),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion
    }
}