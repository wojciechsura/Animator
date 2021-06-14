using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a vertical line.
    /// All points are expressed in absolute coordinates.
    /// </summary>
    public class AbsoluteVerticalLinePathElement : LineBasedPathElement
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildLine(PointF start)
        {
            PointF end = new PointF(start.X, Y);

            return new[] { start, end };
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
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, ValueChangedHandler = HandleLineChanged });

        #endregion
    }
}