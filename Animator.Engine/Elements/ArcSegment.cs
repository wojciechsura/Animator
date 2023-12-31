using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as an elliptical arc.
    /// First point of the arc equals to the last point of previous
    /// path element.
    /// Endpoint of the arc is expressed in absolute coordinates.
    /// </summary>
    public partial class ArcSegment : BaseArcSegment
    {
        // Protected methods --------------------------------------------------

        protected override PointF[][] BuildBeziers(PointF start)
        {
            return InternalBuildBeziers(start, RX, RY, Angle, LargeArcFlag, SweepFlag, EndPoint);
        }

        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"A {F(RX)} {F(RY)} {F(Angle)} {(LargeArcFlag ? 1 : 0)} {(SweepFlag ? 1 : 0)} {F(EndPoint.X)} {F(EndPoint.Y)}";

        // Public properties --------------------------------------------------

        #region RX managed property

        /// <summary>
        /// X radius of the ellipse, from which the arc is built.
        /// </summary>
        public float RX
        {
            get => (float)GetValue(RXProperty);
            set => SetValue(RXProperty, value);
        }

        public static readonly ManagedProperty RXProperty = ManagedProperty.Register(typeof(ArcSegment),
            nameof(RX),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region RY managed property

        /// <summary>
        /// Y radius of the ellipse, from which the arc is built.
        /// </summary>
        public float RY
        {
            get => (float)GetValue(RYProperty);
            set => SetValue(RYProperty, value);
        }

        public static readonly ManagedProperty RYProperty = ManagedProperty.Register(typeof(ArcSegment),
            nameof(RY),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region Angle managed property

        /// <summary>
        /// Angle of the arc.
        /// </summary>
        public float Angle
        {
            get => (float)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly ManagedProperty AngleProperty = ManagedProperty.Register(typeof(ArcSegment),
            nameof(Angle),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region LargeArcFlag managed property

        /// <summary>
        /// Determines, whether arc should follow the larger path or smaller one.
        /// </summary>
        public bool LargeArcFlag
        {
            get => (bool)GetValue(LargeArcFlagProperty);
            set => SetValue(LargeArcFlagProperty, value);
        }

        public static readonly ManagedProperty LargeArcFlagProperty = ManagedProperty.Register(typeof(ArcSegment),
            nameof(LargeArcFlag),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false, ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region SweepFlag managed property

        /// <summary>
        /// Determines, whether arc will be drawn positive-angle or negative-angle.
        /// </summary>
        public bool SweepFlag
        {
            get => (bool)GetValue(SweepFlagProperty);
            set => SetValue(SweepFlagProperty, value);
        }

        public static readonly ManagedProperty SweepFlagProperty = ManagedProperty.Register(typeof(ArcSegment),
            nameof(SweepFlag),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false, ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region EndPoint managed property

        /// <summary>
        /// End point of the arc
        /// </summary>
        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(ArcSegment),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion
    }
}