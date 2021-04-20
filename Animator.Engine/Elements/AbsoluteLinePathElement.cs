using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class AbsoluteLinePathElement : PathElement
    {
        public override string ToPathString() => $"L {F(X)} {F(Y)}";


        #region X managed property

        public float X
        {
            get => (float)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly ManagedProperty XProperty = ManagedProperty.Register(typeof(AbsoluteLinePathElement),
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

        public static readonly ManagedProperty YProperty = ManagedProperty.Register(typeof(AbsoluteLinePathElement),
            nameof(Y),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}