using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a cubic Bezier curve.
    /// First point of the curve equals to the last point of previous
    /// path element.
    /// All points are expressed in relative coordinates.
    /// </summary>
    public class RelativeCubicBezierSegment : BaseCubicBezierBasedSegment
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildBezier(PointF start, PointF lastControlPoint)
        {
            RelativePoint point = new RelativePoint(start);

            PointF controlPoint1 = point.Delta(DeltaControlPoint1);
            PointF controlPoint2 = point.Delta(DeltaControlPoint2);
            PointF endPoint = point.Delta(DeltaEndPoint);

            return new[] { start, controlPoint1, controlPoint2, endPoint };
        }

        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"c {F(DeltaControlPoint1.X)} {F(DeltaControlPoint1.Y)} {F(DeltaControlPoint2.X)} {F(DeltaControlPoint2.Y)} {F(DeltaEndPoint.X)} {F(DeltaEndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region DeltaControlPoint1 managed property

        /// <summary>
        /// First control point, relative to endpoint of the previous
        /// path element.
        /// </summary>
        public PointF DeltaControlPoint1
        {
            get => (PointF)GetValue(DeltaControlPoint1Property);
            set => SetValue(DeltaControlPoint1Property, value);
        }

        public static readonly ManagedProperty DeltaControlPoint1Property = ManagedProperty.Register(typeof(RelativeCubicBezierSegment),
            nameof(DeltaControlPoint1),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region DeltaControlPoint2 managed property

        /// <summary>
        /// Second control point, relative to the first control
        /// point.
        /// </summary>
        public PointF DeltaControlPoint2
        {
            get => (PointF)GetValue(DeltaControlPoint2Property);
            set => SetValue(DeltaControlPoint2Property, value);
        }

        public static readonly ManagedProperty DeltaControlPoint2Property = ManagedProperty.Register(typeof(RelativeCubicBezierSegment),
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

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeCubicBezierSegment),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion
    }
}