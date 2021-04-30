using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;

namespace Animator.Engine.Elements
{
    public abstract class BaseNumericKeyframe : BaseKeyframe
    {
        // Public methods -----------------------------------------------------

        #region EasingFunction managed property

        public EasingFunction EasingFunction
        {
            get => (EasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        public static readonly ManagedProperty EasingFunctionProperty = ManagedProperty.Register(typeof(TimeDurationNumericPropertyAnimator),
            nameof(EasingFunction),
            typeof(EasingFunction),
            new ManagedSimplePropertyMetadata { DefaultValue = EasingFunction.Linear, InheritedFromParent = true });

        #endregion
    }
}