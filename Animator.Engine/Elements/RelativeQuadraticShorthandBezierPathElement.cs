using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class RelativeQuadraticShorthandBezierPathElement : PathElement
    {
        public override string ToPathString() => $"t {F(DX)} {F(DY)}";

        #region DX managed property

        public float DX
        {
            get => (float)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(RelativeQuadraticShorthandBezierPathElement),
            nameof(DX),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion

        #region DY managed property

        public float DY
        {
            get => (float)GetValue(DYProperty);
            set => SetValue(DYProperty, value);
        }

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(RelativeQuadraticShorthandBezierPathElement),
            nameof(DY),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}