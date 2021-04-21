using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class RelativeQuadraticBezierPathElement : BaseQuadraticBezierPathElement
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

        public PointF DeltaControlPoint
        {
            get => (PointF)GetValue(DeltaControlPointProperty);
            set => SetValue(DeltaControlPointProperty, value);
        }

        public static readonly ManagedProperty DeltaControlPointProperty = ManagedProperty.Register(typeof(RelativeQuadraticBezierPathElement),
            nameof(DeltaControlPoint),
            typeof(PointF),
            new ManagedAnimatedPropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion


        #region DeltaEndPoint managed property

        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeQuadraticBezierPathElement),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedAnimatedPropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}