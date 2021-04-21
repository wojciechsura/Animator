using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class AbsoluteQuadraticShorthandBezierPathElement : BaseQuadraticBezierPathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            var delta = start.Subtract(lastControlPoint);
            var controlPoint = start.Add(delta);

            (var controlPoint1, var controlPoint2) = EstimateCubicControlPoints(start, controlPoint, EndPoint);

            path.AddBezier(start, controlPoint1, controlPoint2, EndPoint);

            return (EndPoint, controlPoint);
        }

        internal override string ToPathString() => $"T {F(EndPoint.X)} {F(EndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region EndPoint managed property

        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteQuadraticShorthandBezierPathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}