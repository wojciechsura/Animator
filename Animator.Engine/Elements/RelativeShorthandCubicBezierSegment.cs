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
    /// All points are expressed in relative coordinates.
    /// </summary>
    public class RelativeShorthandCubicBezierSegment : BaseCubicBezierBasedSegment
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildBezier(PointF start, PointF lastControlPoint)
        {
            var delta = start.Subtract(lastControlPoint);
            var controlPoint1 = start.Add(delta);

            RelativePoint point = new RelativePoint(controlPoint1);

            PointF controlPoint2 = point.Delta(DeltaControlPoint2);
            PointF endPoint = point.Delta(DeltaEndPoint);
            
            return new[] { start, controlPoint1, controlPoint2, endPoint };
        }

        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"s {F(DeltaControlPoint2.X)} {F(DeltaControlPoint2.Y)} {F(DeltaEndPoint.X)} {F(DeltaEndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region DeltaControlPoint2 managed property

        /// <summary>
        /// Second control point, relative to the deduced 
        /// first control point.
        /// </summary>
        public PointF DeltaControlPoint2
        {
            get => (PointF)GetValue(DeltaControlPoint2Property);
            set => SetValue(DeltaControlPoint2Property, value);
        }

        public static readonly ManagedProperty DeltaControlPoint2Property = ManagedProperty.Register(typeof(RelativeShorthandCubicBezierSegment),
            nameof(DeltaControlPoint2),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region DeltaEndPoint managed property

        /// <summary>
        /// End point, relative to the second control point.
        /// </summary>
        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeShorthandCubicBezierSegment),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion
    }
}