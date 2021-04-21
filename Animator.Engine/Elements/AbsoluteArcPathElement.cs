using Animator.Engine.Base;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class AbsoluteArcPathElement : BaseArcPathElement
    {
        // Public methods -----------------------------------------------------

        internal override string ToPathString() => $"A {F(RX)} {F(RY)} {F(Angle)} {(LargeArcFlag ? 1 : 0)} {(SweepFlag ? 1 : 0)} {F(EndPoint.X)} {F(EndPoint.Y)}";

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            InternalAddToGeometry(start, RX, RY, Angle, LargeArcFlag, SweepFlag, EndPoint, path);
            return (EndPoint, EndPoint);
        }

        // Public properties --------------------------------------------------

        #region RX managed property

        public float RX
        {
            get => (float)GetValue(RXProperty);
            set => SetValue(RXProperty, value);
        }

        public static readonly ManagedProperty RXProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
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

        public static readonly ManagedProperty RYProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
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

        public static readonly ManagedProperty AngleProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
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

        public static readonly ManagedProperty LargeArcFlagProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
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

        public static readonly ManagedProperty SweepFlagProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
            nameof(SweepFlag),
            typeof(bool),
            new ManagedSimplePropertyMetadata(false));

        #endregion

        #region EndPoint managed property

        public PointF EndPoint
        {
            get => (PointF)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly ManagedProperty EndPointProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
            nameof(EndPoint),
            typeof(PointF),
            new ManagedSimplePropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}