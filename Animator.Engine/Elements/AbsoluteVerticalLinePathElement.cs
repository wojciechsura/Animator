using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class AbsoluteVerticalLinePathElement : PathElement
    {
        // Protected methods --------------------------------------------------

        protected override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PathElement lastElement, PointF lastControlPoint, GraphicsPath path)
        {
            PointF end = new PointF(start.X, Y);

            path.AddLine(start, end);

            return (end, end);
        }

        // Public methods -----------------------------------------------------

        public override string ToPathString() => $"V {F(Y)}";

        // Public properties --------------------------------------------------

        #region Y managed property

        public float Y
        {
            get => (float)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly ManagedProperty YProperty = ManagedProperty.Register(typeof(AbsoluteVerticalLinePathElement),
            nameof(Y),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}