using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class RelativeVerticalLinePathElement : PathElement
    {
        public override string ToPathString() => $"v {F(DY)}";

        #region DY managed property

        public float DY
        {
            get => (float)GetValue(DYProperty);
            set => SetValue(DYProperty, value);
        }

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(RelativeVerticalLinePathElement),
            nameof(DY),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}