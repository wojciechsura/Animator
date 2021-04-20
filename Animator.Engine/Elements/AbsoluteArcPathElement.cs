using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class AbsoluteArcPathElement : PathElement
    {
        public override string ToPathString() => $"A {F(RX)} {F(RY)} {F(XAxisRotation)} {(LargeArcFlag ? 1 : 0)} {(SweepFlag ? 1 : 0)} {F(X)} {F(Y)}";

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

        #region XAxisRotation managed property

        public float XAxisRotation
        {
            get => (float)GetValue(XAxisRotationProperty);
            set => SetValue(XAxisRotationProperty, value);
        }

        public static readonly ManagedProperty XAxisRotationProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
            nameof(XAxisRotation),
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


        #region X managed property

        public float X
        {
            get => (float)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly ManagedProperty XProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
            nameof(X),
            typeof(float),
            new ManagedSimplePropertyMetadata(0));

        #endregion


        #region Y managed property

        public float Y
        {
            get => (float)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly ManagedProperty YProperty = ManagedProperty.Register(typeof(AbsoluteArcPathElement),
            nameof(Y),
            typeof(float),
            new ManagedSimplePropertyMetadata(0));

        #endregion
    }
}