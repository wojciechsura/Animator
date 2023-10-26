using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as an elliptical arc.
    /// First point of the arc equals to the last point of previous
    /// path element.
    /// Endpoint of the arc is expressed in relative coordinates.
    /// </summary>
    public partial class RelativeArcSegment : BaseArcSegment
    {
        // Protected methods --------------------------------------------------

        protected override PointF[][] BuildBeziers(PointF start)
        {
            RelativePoint point = new RelativePoint(start);
            return InternalBuildBeziers(start, RX, RY, Angle, LargeArcFlag, SweepFlag, point.Delta(DeltaEndPoint));
        }

        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"a {F(RX)} {F(RY)} {F(Angle)} {(LargeArcFlag ? 1 : 0)} {(SweepFlag ? 1 : 0)} {F(DeltaEndPoint.X)} {F(DeltaEndPoint.Y)}";

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

        public static readonly ManagedProperty RXProperty = ManagedProperty.Register(typeof(RelativeArcSegment),
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

        public static readonly ManagedProperty RYProperty = ManagedProperty.Register(typeof(RelativeArcSegment),
            nameof(RY),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region Angle managed property

        /// <summary>
        /// Angle of the arc
        /// </summary>
        public float Angle
        {
            get => (float)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly ManagedProperty AngleProperty = ManagedProperty.Register(typeof(RelativeArcSegment),
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

        public static readonly ManagedProperty LargeArcFlagProperty = ManagedProperty.Register(typeof(RelativeArcSegment),
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

        public static readonly ManagedProperty SweepFlagProperty = ManagedProperty.Register(typeof(RelativeArcSegment),
            nameof(SweepFlag),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false, ValueChangedHandler = HandleCurveChanged });

        #endregion

        #region DeltaEndPoint managed property

        /// <summary>
        /// End point of the arc, relative to the endpoint of previous path element.
        /// </summary>
        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeArcSegment),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f), ValueChangedHandler = HandleCurveChanged });

        #endregion
    }
}