using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
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

        public float RX
        {
            get => (float)GetValue(RXProperty);
            set => SetValue(RXProperty, value);
        }

        public static readonly ManagedProperty RXProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(RX),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion

        #region RY managed property

        public float RY
        {
            get => (float)GetValue(RYProperty);
            set => SetValue(RYProperty, value);
        }

        public static readonly ManagedProperty RYProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(RY),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion

        #region Angle managed property

        public float Angle
        {
            get => (float)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly ManagedProperty AngleProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(Angle),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion

        #region LargeArcFlag managed property

        public bool LargeArcFlag
        {
            get => (bool)GetValue(LargeArcFlagProperty);
            set => SetValue(LargeArcFlagProperty, value);
        }

        public static readonly ManagedProperty LargeArcFlagProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(LargeArcFlag),
            typeof(bool),
            new ManagedSimplePropertyMetadata(false));

        #endregion

        #region SweepFlag managed property

        public bool SweepFlag
        {
            get => (bool)GetValue(SweepFlagProperty);
            set => SetValue(SweepFlagProperty, value);
        }

        public static readonly ManagedProperty SweepFlagProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(SweepFlag),
            typeof(bool),
            new ManagedSimplePropertyMetadata(false));

        #endregion

        #region DeltaEndPoint managed property

        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}