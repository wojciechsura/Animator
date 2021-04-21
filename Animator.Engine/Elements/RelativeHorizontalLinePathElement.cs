using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class RelativeHorizontalLinePathElement : PathElement
    {
        // Protected methods --------------------------------------------------

        protected override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PathElement lastElement, PointF lastControlPoint, GraphicsPath path)
        {
            PointF end = new PointF(start.X + DX, start.Y);

            path.AddLine(start, end);

            return (end, end);
        }

        // Public methods -----------------------------------------------------

        public override string ToPathString() => $"h {F(DX)}";        

        // Public properties --------------------------------------------------

        #region DX managed property

        public float DX
        {
            get => (float)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(RelativeHorizontalLinePathElement),
            nameof(DX),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}