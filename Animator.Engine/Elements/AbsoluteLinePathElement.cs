using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a line.
    /// All points are expressed in absolute coordinates.
    /// </summary>
    public class AbsoluteLinePathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            path.AddLine(start, EndPoint);

            return (EndPoint, EndPoint);
        }

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

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteLinePathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}