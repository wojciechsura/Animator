using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class AbsoluteCubicShorthandBezierPathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            var delta = start.Subtract(lastControlPoint);
            var controlPoint1 = start.Add(delta);

            path.AddBezier(start, controlPoint1, ControlPoint2, EndPoint);

            return (EndPoint, ControlPoint2);
        }

        internal override string ToPathString() => $"S {F(ControlPoint2.X)} {F(ControlPoint2.Y)} {F(EndPoint.X)} {F(EndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region ControlPoint2 managed property

        public PointF ControlPoint2
        {
            get => (PointF)GetValue(ControlPoint2Property);
            set => SetValue(ControlPoint2Property, value);
        }

        public static readonly ManagedProperty ControlPoint2Property = ManagedProperty.Register(typeof(AbsoluteCubicShorthandBezierPathElement),
            nameof(ControlPoint2),
            typeof(PointF),
            new ManagedSimplePropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion

        #region EndPoint managed property

        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteCubicShorthandBezierPathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}