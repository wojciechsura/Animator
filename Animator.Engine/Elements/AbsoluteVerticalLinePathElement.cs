using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a vertical line.
    /// All points are expressed in absolute coordinates.
    /// </summary>
    public class AbsoluteVerticalLinePathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            PointF end = new PointF(start.X, Y);

            path.AddLine(start, end);

            return (end, end);
        }

        internal override string ToPathString() => $"V {F(Y)}";

        // Public properties --------------------------------------------------

        #region Y managed property

        /// <summary>
        /// Y-coordinate of the end point. X-coordinate equals to
        /// the end point's of the previous path element.
        /// </summary>
        public float Y
        {
            get => (float)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly ManagedProperty YProperty = ManagedProperty.Register(typeof(AbsoluteVerticalLinePathElement),
            nameof(Y),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}