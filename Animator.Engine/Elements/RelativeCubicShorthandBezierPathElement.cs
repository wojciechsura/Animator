using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class RelativeCubicShorthandBezierPathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            var delta = start.Subtract(lastControlPoint);
            var controlPoint1 = start.Add(delta);

            RunningPoint point = new RunningPoint(start);

            PointF controlPoint2 = point.Delta(DeltaControlPoint2);
            PointF endPoint = point.Delta(DeltaEndPoint);
            path.AddBezier(point.Current, controlPoint1, controlPoint2, endPoint);

            return (endPoint, controlPoint2);
        }

        internal override string ToPathString() => $"s {F(DeltaControlPoint2.X)} {F(DeltaControlPoint2.Y)} {F(DeltaEndPoint.X)} {F(DeltaEndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region DeltaControlPoint2 managed property

        public PointF DeltaControlPoint2
        {
            get => (PointF)GetValue(DeltaControlPoint2Property);
            set => SetValue(DeltaControlPoint2Property, value);
        }

        public static readonly ManagedProperty DeltaControlPoint2Property = ManagedProperty.Register(typeof(RelativeCubicShorthandBezierPathElement),
            nameof(DeltaControlPoint2),
            typeof(PointF),
            new ManagedSimplePropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion

        #region DeltaEndPoint managed property

        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeCubicShorthandBezierPathElement),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}