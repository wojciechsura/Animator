using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class AbsoluteLinePathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            path.AddLine(start, EndPoint);

            return (EndPoint, EndPoint);
        }

        internal override string ToPathString() => $"L {F(EndPoint.X)} {F(EndPoint.Y)}";        

        // Public properties --------------------------------------------------

        #region EndPoint managed property

        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteLinePathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedAnimatedPropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}