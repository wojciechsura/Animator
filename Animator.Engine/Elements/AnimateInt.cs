using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Allows applying an animation to an int property of some
    /// object. It is meant to be used only if you want to apply
    /// single animation per whole scene. Otherwise, use Storyboard
    /// and IntKeyframe to define the whole animation.
    /// </summary>
    public class AnimateInt : AnimateNumericPropertyInTime
    {
        // Public methods -----------------------------------------------------

        public override bool ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);

            var factor = TimeCalculator.EvalAnimationFactor((float)StartTime.TotalMilliseconds, (float)EndTime.TotalMilliseconds, timeMs);
            var easedValue = EasingFunctionRepository.Ease(EasingFunction, factor);

            int from = IsPropertySet(FromProperty) ? From : (int)obj.GetBaseValue(prop);
            int to = IsPropertySet(ToProperty) ? To : (int)obj.GetBaseValue(prop);

            var previous = (int)obj.GetValue(prop);
            var value = (int)Math.Round(from + (to - from) * easedValue);
            obj.SetAnimatedValue(prop, value);
            var next = (int)obj.GetValue(prop);

            // if (!object.Equals(next, previous)) { Console.WriteLine($"{obj}.{prop} changed from {previous} to {next}"); }

            return previous != next;
        }

        // Public properties --------------------------------------------------

        #region From managed property

        /// <summary>
        /// Initial value of the property when animation starts (at StartTime)
        /// </summary>
        public int From
        {
            get => (int)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly ManagedProperty FromProperty = ManagedProperty.Register(typeof(AnimateInt),
            nameof(From),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion

        #region To managed property

        /// <summary>
        /// Final value of the property when animation ends (at EndTime)
        /// </summary>
        public int To
        {
            get => (int)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly ManagedProperty ToProperty = ManagedProperty.Register(typeof(AnimateInt),
            nameof(To),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion
    }
}
