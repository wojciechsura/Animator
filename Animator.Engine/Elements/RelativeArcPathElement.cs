using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class RelativeArcPathElement : PathElement
    {
        public override string ToPathString() => $"a {F(RX)} {F(RY)} {F(XAxisRotation)} {(LargeArcFlag ? 1 : 0)} {(SweepFlag ? 1 : 0)} {F(DX)} {F(DY)}";


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


        #region XAxisRotation managed property

        public float XAxisRotation
        {
            get => (float)GetValue(XAxisRotationProperty);
            set => SetValue(XAxisRotationProperty, value);
        }

        public static readonly ManagedProperty XAxisRotationProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
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

        #region DX managed property

        public float DX
        {
            get => (float)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(DX),
            typeof(float),
            new ManagedSimplePropertyMetadata(0));

        #endregion

        #region DY managed property

        public float DY
        {
            get => (float)GetValue(DYProperty);
            set => SetValue(DYProperty, value);
        }

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(RelativeArcPathElement),
            nameof(DY),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion        
    }
}