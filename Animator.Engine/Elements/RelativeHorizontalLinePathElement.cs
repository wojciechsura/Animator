using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class RelativeHorizontalLinePathElement : PathElement
    {
        public override string ToPathString() => $"h {F(DX)}";


        #region DX managed property

        public float DX
        {
            get => (float)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(RelativeHorizontalLinePathElement),
            nameof(DX),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}