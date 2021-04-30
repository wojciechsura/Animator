using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents a path part, which is drawn as an elliptical arc.
    /// Endpoint of the arc is expressed in relative coordinates.
    /// </summary>
    public class RelativeArcPathElement : BaseArcPathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            RunningPoint point = new RunningPoint(start);

            InternalAddToGeometry(point.Current, RX, RY, Angle, LargeArcFlag, SweepFlag, point.Delta(DeltaEndPoint), path);

            return (point.Current, point.Current);
        }

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

        public static readonly ManagedProperty RXProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(RX),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

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

        public static readonly ManagedProperty RYProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(RY),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

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

        public static readonly ManagedProperty AngleProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(Angle),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

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

        public static readonly ManagedProperty LargeArcFlagProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(LargeArcFlag),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

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

        public static readonly ManagedProperty SweepFlagProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(SweepFlag),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

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

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}