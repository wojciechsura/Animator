using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class AbsoluteCubicShorthandBezierPathElement : PathElement
    {
        public override string ToPathString() => $"S {F(X2)} {F(Y2)} {F(X)} {F(Y)}";


        #region X2 managed property

        public float X2
        {
            get => (float)GetValue(X2Property);
            set => SetValue(X2Property, value);
        }

        public static readonly ManagedProperty X2Property = ManagedProperty.Register(typeof(AbsoluteCubicShorthandBezierPathElement),
            nameof(X2),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion


        #region Y2 managed property

        public float Y2
        {
            get => (float)GetValue(Y2Property);
            set => SetValue(Y2Property, value);
        }

        public static readonly ManagedProperty Y2Property = ManagedProperty.Register(typeof(AbsoluteCubicShorthandBezierPathElement),
            nameof(Y2),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion


        #region X managed property

        public float X
        {
            get => (float)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly ManagedProperty XProperty = ManagedProperty.Register(typeof(AbsoluteCubicShorthandBezierPathElement),
            nameof(X),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion


        #region Y managed property

        public float Y
        {
            get => (float)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly ManagedProperty YProperty = ManagedProperty.Register(typeof(AbsoluteCubicShorthandBezierPathElement),
            nameof(Y),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}