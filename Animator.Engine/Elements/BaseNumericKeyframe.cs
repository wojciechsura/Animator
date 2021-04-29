using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;

namespace Animator.Engine.Elements
{
    public abstract class BaseNumericKeyframe : BaseKeyframe
    {
        // Protected methods --------------------------------------------------

        protected float EvalTimeFactor(float fromTimeMs, float currentTimeMs)
        {
            var timeFactor = 0.0f;
            if (Time.TotalMilliseconds - fromTimeMs > 0)
                timeFactor = (currentTimeMs - fromTimeMs) / ((float)Time.TotalMilliseconds - fromTimeMs);
            var easedFactor = EasingFunctionRepository.Ease(EasingFunction, timeFactor);
            return easedFactor;
        }

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