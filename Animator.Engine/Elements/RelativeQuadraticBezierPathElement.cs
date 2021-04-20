using Animator.Engine.Base;

namespace Animator.Engine.Elements
{
    public class RelativeQuadraticBezierPathElement : PathElement
    {
        public override string ToPathString() => $"q {F(DX1)} {F(DY1)} {F(DX)} {F(DY)}";


        #region DX1 managed property

        public float DX1
        {
            get => (float)GetValue(DX1Property);
            set => SetValue(DX1Property, value);
        }

        public static readonly ManagedProperty DX1Property = ManagedProperty.Register(typeof(RelativeQuadraticBezierPathElement),
            nameof(DX1),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion


        #region DY1 managed property

        public float DY1
        {
            get => (float)GetValue(DY1Property);
            set => SetValue(DY1Property, value);
        }

        public static readonly ManagedProperty DY1Property = ManagedProperty.Register(typeof(RelativeQuadraticBezierPathElement),
            nameof(DY1),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion


        #region DX managed property

        public float DX
        {
            get => (float)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(RelativeQuadraticBezierPathElement),
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

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(RelativeQuadraticBezierPathElement),
            nameof(DY),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}