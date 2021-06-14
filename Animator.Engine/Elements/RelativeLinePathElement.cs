using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a line.
    /// All points are expressed in relative coordinates.
    /// </summary>
    public class RelativeLinePathElement : LineBasedPathElement
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildLine(PointF start)
        {
            var end = start.Add(DeltaEndPoint);

            return new[] { start, end };
        }

        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"l {F(DeltaEndPoint.X)} {F(DeltaEndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region DeltaEndPoint managed property

        /// <summary>
        /// End point of the line, relative to end point
        /// of the previous path element.
        /// </summary>
        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeLinePathElement),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleLineChanged });

        #endregion
    }
}