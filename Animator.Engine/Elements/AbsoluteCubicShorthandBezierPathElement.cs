using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a cubic Bezier curve.
    /// First point of the curve equals to the last point of previous
    /// path element. First control point is deduced from the previous
    /// path element as a mirror of its last control point against its
    /// endpoint.
    /// All points are expressed in absolute coordinates.
    /// </summary>
    public class AbsoluteCubicShorthandBezierPathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            var delta = start.Subtract(lastControlPoint);
            var controlPoint1 = start.Add(delta);

            path.AddBezier(start, controlPoint1, ControlPoint2, EndPoint);

            return (EndPoint, ControlPoint2);
        }

        internal override string ToPathString() => $"S {F(ControlPoint2.X)} {F(ControlPoint2.Y)} {F(EndPoint.X)} {F(EndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region ControlPoint2 managed property

        /// <summary>
        /// Second control point of the curve.
        /// </summary>
        public PointF ControlPoint2
        {
            get => (PointF)GetValue(ControlPoint2Property);
            set => SetValue(ControlPoint2Property, value);
        }

        public static readonly ManagedProperty ControlPoint2Property = ManagedProperty.Register(typeof(AbsoluteCubicShorthandBezierPathElement),
            nameof(ControlPoint2),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region EndPoint managed property

        /// <summary>
        /// End point of the curve.
        /// </summary>
        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteCubicShorthandBezierPathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}