using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class RelativeVerticalLinePathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            PointF end = new PointF(start.X, start.Y + DY);

            path.AddLine(start, end);

            return (end, end);
        }

        internal override string ToPathString() => $"v {F(DY)}";

        // Public properties --------------------------------------------------

        #region DY managed property

        public float DY
        {
            get => (float)GetValue(DYProperty);
            set => SetValue(DYProperty, value);
        }

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(RelativeVerticalLinePathElement),
            nameof(DY),
            typeof(float),
            new ManagedAnimatedPropertyMetadata(0.0f));

        #endregion
    }
}