using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Begins a new path in specified absolute coordinates
    /// </summary>
    public partial class MoveSegment : Segment
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            if (path != null)
                path.StartFigure();

            return (EndPoint, EndPoint);
        }

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path, float? cutFrom, float? cutTo)
        {
            return AddToGeometry(start, lastControlPoint, path);
        }

        internal override string ToPathString() => $"M {F(EndPoint.X)} {F(EndPoint.Y)}";

        internal override (float length, PointF endPoint, PointF lastControlPoint) EvalLength(PointF start, PointF lastControlPoint)
        {
            return (0.0f, EndPoint, EndPoint);
        }

        // Public properties --------------------------------------------------

        #region EndPoint managed property

        /// <summary>
        /// Coordinates at which new path should begin.
        /// </summary>
        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(MoveSegment),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}