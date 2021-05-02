using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a horizontal line.
    /// All points are expressed in absolute coordinates.
    /// </summary>
    public class AbsoluteHorizontalLinePathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            PointF end = new PointF(X, start.Y);

            path.AddLine(start, end);

            return (end, end);
        }

        internal override string ToPathString() => $"H {F(X)}";

        // Public properties --------------------------------------------------

        #region X managed property

        /// <summary>
        /// X-coordinate of the end point. Y-coordinate equals to
        /// the end point's of the previous path element.
        /// </summary>
        public float X
        {
            get => (float)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly ManagedProperty XProperty = ManagedProperty.Register(typeof(AbsoluteHorizontalLinePathElement),
            nameof(X),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}