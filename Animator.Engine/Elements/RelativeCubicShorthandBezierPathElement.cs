using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class RelativeCubicShorthandBezierPathElement : PathElement
    {
        public override string ToPathString() => $"s {F(DX2)} {F(DY2)} {F(DX)} {F(DY)}";


        #region DX2 managed property

        public float DX2
        {
            get => (float)GetValue(DX2Property);
            set => SetValue(DX2Property, value);
        }

        public static readonly ManagedProperty DX2Property = ManagedProperty.Register(typeof(RelativeCubicShorthandBezierPathElement),
            nameof(DX2),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion


        #region DY2 managed property

        public float DY2
        {
            get => (float)GetValue(DY2Property);
            set => SetValue(DY2Property, value);
        }

        public static readonly ManagedProperty DY2Property = ManagedProperty.Register(typeof(RelativeCubicShorthandBezierPathElement),
            nameof(DY2),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion


        #region DX managed property

        public float DX
        {
            get => (float)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(RelativeCubicShorthandBezierPathElement),
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

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(RelativeCubicShorthandBezierPathElement),
            nameof(DY),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion        
    }
}