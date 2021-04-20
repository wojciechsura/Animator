using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class AbsoluteHorizontalLinePathElement : PathElement
    {
        public override string ToPathString() => $"H {F(X)}";


        #region X managed property

        public float X
        {
            get => (float)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly ManagedProperty XProperty = ManagedProperty.Register(typeof(AbsoluteHorizontalLinePathElement),
            nameof(X),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}