using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class RelativeQuadraticShorthandBezierPathElement : BaseQuadraticBezierPathElement
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