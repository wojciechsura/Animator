using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a line.
    /// All points are expressed in absolute coordinates.
    /// </summary>
    public class LineSegment : LineBasedSegment
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildLine(PointF start)
        {
            return new[] { start, EndPoint };
        }

        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"L {F(EndPoint.X)} {F(EndPoint.Y)}";        

        // Public properties --------------------------------------------------

        #region EndPoint managed property

        /// <summary>
        /// End point of the line.
        /// </summary>
        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(LineSegment),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleLineChanged });

        #endregion
    }
}