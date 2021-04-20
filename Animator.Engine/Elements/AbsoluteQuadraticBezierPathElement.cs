using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class AbsoluteQuadraticBezierPathElement : PathElement
    {
        public override string ToPathString() => $"Q {F(X1)} {F(Y1)} {F(X)} {F(Y)}";


        #region X1 managed property

        public float X1
        {
            get => (float)GetValue(X1Property);
            set => SetValue(X1Property, value);
        }

        public static readonly ManagedProperty X1Property = ManagedProperty.Register(typeof(AbsoluteQuadraticBezierPathElement),
            nameof(X1),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion

        #region Y1 managed property

        public float Y1
        {
            get => (float)GetValue(Y1Property);
            set => SetValue(Y1Property, value);
        }

        public static readonly ManagedProperty Y1Property = ManagedProperty.Register(typeof(AbsoluteQuadraticBezierPathElement),
            nameof(Y1),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion


        #region X managed property

        public float X
        {
            get => (float)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly ManagedProperty XProperty = ManagedProperty.Register(typeof(AbsoluteQuadraticBezierPathElement),
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

        public static readonly ManagedProperty YProperty = ManagedProperty.Register(typeof(AbsoluteQuadraticBezierPathElement),
            nameof(Y),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}