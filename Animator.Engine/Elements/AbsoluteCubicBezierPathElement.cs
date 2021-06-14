using Animator.Engine.Base;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as a cubic Bezier curve.
    /// First point of the curve equals to the last point of previous
    /// path element.
    /// All points are expressed in absolute coordinates.
    /// </summary>
    public class AbsoluteCubicBezierPathElement : CubicBezierBasedPathElement
    {
        // Protected methods --------------------------------------------------

        protected override PointF[] BuildBezier(PointF endPoint, PointF lastControlPoint)
        {
            return new PointF[] { endPoint, ControlPoint1, ControlPoint2, EndPoint };
        }

        // Public methods -----------------------------------------------------

        internal override string ToPathString() => $"C {F(ControlPoint1.X)} {F(ControlPoint1.Y)} {F(ControlPoint2.X)} {F(ControlPoint2.Y)} {F(EndPoint.X)} {F(EndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region ControlPoint1 managed property

        /// <summary>
        /// First control point of the curve.
        /// </summary>
        public PointF ControlPoint1
        {
            get => (PointF)GetValue(ControlPoint1Property);
            set => SetValue(ControlPoint1Property, value);
        }

        public static readonly ManagedProperty ControlPoint1Property = ManagedProperty.Register(typeof(AbsoluteCubicBezierPathElement),
            nameof(ControlPoint1),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region ControlPoint2 managed property

        /// <summary>
        /// Second control point of the curve.
        /// </summary>
        public PointF ControlPoint2
        {
            get => (PointF)GetValue(ControlPoint2Property);
            set => SetValue(ControlPoint2Property, value);
        }

        public static readonly ManagedProperty ControlPoint2Property = ManagedProperty.Register(typeof(AbsoluteCubicBezierPathElement),
            nameof(ControlPoint2),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

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

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteCubicBezierPathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion
    }
}