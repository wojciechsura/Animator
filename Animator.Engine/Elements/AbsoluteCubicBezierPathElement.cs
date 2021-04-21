using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class AbsoluteCubicBezierPathElement : PathElement
    {
        // Protected methods --------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            path.AddBezier(start, ControlPoint1, ControlPoint2, EndPoint);

            return (EndPoint, ControlPoint2);
        }

        // Public methods -----------------------------------------------------

        internal override string ToPathString() => $"C {F(ControlPoint1.X)} {F(ControlPoint1.Y)} {F(ControlPoint2.X)} {F(ControlPoint2.Y)} {F(EndPoint.X)} {F(EndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region ControlPoint1 managed property

        public PointF ControlPoint1
        {
            get => (PointF)GetValue(ControlPoint1Property);
            set => SetValue(ControlPoint1Property, value);
        }

        public static readonly ManagedProperty ControlPoint1Property = ManagedProperty.Register(typeof(AbsoluteCubicBezierPathElement),
            nameof(ControlPoint1),
            typeof(PointF),
            new ManagedAnimatedPropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion

        #region ControlPoint2 managed property

        public PointF ControlPoint2
        {
            get => (PointF)GetValue(ControlPoint2Property);
            set => SetValue(ControlPoint2Property, value);
        }

        public static readonly ManagedProperty ControlPoint2Property = ManagedProperty.Register(typeof(AbsoluteCubicBezierPathElement),
            nameof(ControlPoint2),
            typeof(PointF),
            new ManagedAnimatedPropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion

        #region EndPoint managed property

        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteCubicBezierPathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedAnimatedPropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}