using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a quadratic Bezier curve.
    /// First point of the curve equals to the last point of previous
    /// path element.
    /// All points are expressed in absolute coordinates.
    /// </summary>
    public class AbsoluteQuadraticBezierPathElement : BaseQuadraticBezierPathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            (var controlPoint1, var controlPoint2) = EstimateCubicControlPoints(start, ControlPoint, EndPoint);

            path.AddBezier(start, controlPoint1, controlPoint2, EndPoint);

            return (EndPoint, ControlPoint);
        }

        internal override string ToPathString() => $"Q {F(ControlPoint.X)} {F(ControlPoint.Y)} {F(EndPoint.X)} {F(EndPoint.Y)}";


        // Public properties --------------------------------------------------

        #region ControlPoint managed property

        /// <summary>
        /// Control point of the curve.
        /// </summary>
        public PointF ControlPoint
        {
            get => (PointF)GetValue(ControlPointProperty);
            set => SetValue(ControlPointProperty, value);
        }

        public static readonly ManagedProperty ControlPointProperty = ManagedProperty.Register(typeof(AbsoluteQuadraticBezierPathElement),
            nameof(ControlPoint),
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

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteQuadraticBezierPathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}