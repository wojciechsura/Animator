using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a horizontal line.
    /// All points are expressed in relative coordinates.
    /// </summary>
    public class RelativeHorizontalLineSegment : LineBasedSegment
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildLine(PointF start)
        {
            PointF end = new PointF(start.X + DX, start.Y);

            return new[] { start, end };
        }

        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"h {F(DX)}";        

        // Public properties --------------------------------------------------

        #region DX managed property

        /// <summary>
        /// X-coordinate of the end point relative to X-coordinate of 
        /// end point of the previous path element. Y-coordinate equals
        /// to end point's of the previous path element.
        /// </summary>
        public float DX
        {
            get => (float)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(RelativeHorizontalLineSegment),
            nameof(DX),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, ValueChangedHandler = HandleLineChanged });

        #endregion
    }
}