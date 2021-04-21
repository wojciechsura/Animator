using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class AbsoluteMovePathElement : PathElement
    {
        // Protected methods --------------------------------------------------

        protected override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PathElement lastElement, PointF lastControlPoint, GraphicsPath path)
        {
            path.StartFigure();

            return (EndPoint, EndPoint);
        }

        // Public methods -----------------------------------------------------

        public override string ToPathString() => $"M {F(EndPoint.X)} {F(EndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region EndPoint managed property

        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteMovePathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}