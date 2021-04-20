using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class AbsoluteVerticalLinePathElement : PathElement
    {
        public override string ToPathString() => $"V {F(Y)}";


        #region Y managed property

        public float Y
        {
            get => (float)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly ManagedProperty YProperty = ManagedProperty.Register(typeof(AbsoluteVerticalLinePathElement),
            nameof(Y),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}