using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class IntPropertyAnimator : TimeDurationNumericPropertyAnimator
    {
        // Public methods -----------------------------------------------------

        public override void ApplyAnimation(float timeMs)
        {
            if (Scene == null)
                throw new InvalidOperationException("Animation can be applied only when scene is available!");

            (var obj, var prop) = Scene.FindProperty(TargetName, Path);

            if (obj != null && prop != null)
            {
                var factor = TimeCalculator.EvalAnimationFactor((float)StartTime.TotalMilliseconds, (float)EndTime.TotalMilliseconds, timeMs);
                var easedValue = EasingFunctionRepository.Ease(EasingFunction, factor);

                int from = IsPropertySet(FromProperty) ? From : (int)obj.GetBaseValue(prop);
                int to = IsPropertySet(ToProperty) ? To : (int)obj.GetBaseValue(prop);

                obj.SetAnimatedValue(prop, (int)Math.Round(from + (to - from) * easedValue));
            }
        }

        // Public properties --------------------------------------------------

        #region From managed property

        public int From
        {
            get => (int)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly ManagedProperty FromProperty = ManagedProperty.Register(typeof(IntPropertyAnimator),
            nameof(From),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion

        #region To managed property

        public int To
        {
            get => (int)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly ManagedProperty ToProperty = ManagedProperty.Register(typeof(IntPropertyAnimator),
            nameof(To),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion
    }
}
