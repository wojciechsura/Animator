using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class AbsoluteHorizontalLinePathElement : PathElement
    {
        // Protected methods --------------------------------------------------

        protected override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PathElement lastElement, PointF lastControlPoint, GraphicsPath path)
        {
            PointF end = new PointF(X, start.Y);

            path.AddLine(start, end);

            return (end, end);
        }

        // Public methods -----------------------------------------------------

        public override string ToPathString() => $"H {F(X)}";

        // Public properties --------------------------------------------------

        #region X managed property

        public float X
        {
            get => (float)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly ManagedProperty XProperty = ManagedProperty.Register(typeof(AbsoluteHorizontalLinePathElement),
            nameof(X),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}