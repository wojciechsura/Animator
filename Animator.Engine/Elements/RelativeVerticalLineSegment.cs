using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a horizontal line.
    /// All points are expressed in relative coordinates.
    /// </summary>
    public class RelativeVerticalLineSegment : LineBasedSegment
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildLine(PointF start)
        {
            PointF end = new PointF(start.X, start.Y + DY);

            return new[] { start, end };
        }

        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"v {F(DY)}";

        // Public properties --------------------------------------------------

        #region DY managed property

        /// <summary>
        /// Y-coordinate of the end point relative to Y-coordinate of 
        /// end point of the previous path element. X-coordinate equals
        /// to end point's of the previous path element.
        /// </summary>
        public float DY
        {
            get => (float)GetValue(DYProperty);
            set => SetValue(DYProperty, value);
        }

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(RelativeVerticalLineSegment),
            nameof(DY),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, ValueChangedHandler = HandleLineChanged });

        #endregion
    }
}