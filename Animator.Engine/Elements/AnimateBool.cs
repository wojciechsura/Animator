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
    /// Allows applying an animation to a bool property of some
    /// object. It is meant to be used only if you want to apply
    /// single animation per whole scene. Otherwise, use Storyboard
    /// and BoolKeyframe to define the whole animation.
    /// </summary>
    public class AnimateBool : AnimateNumericPropertyInTime
    {
        // Public methods -----------------------------------------------------

        public override bool ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);

            var factor = TimeCalculator.EvalAnimationFactor((float)StartTime.TotalMilliseconds, (float)EndTime.TotalMilliseconds, timeMs);
            var easedValue = Ease(factor);

            float from = (IsPropertySet(FromProperty) ? From : (bool)obj.GetBaseValue(prop)) ? 1.0f : 0.0f;
            float to = (IsPropertySet(ToProperty) ? To : (bool)obj.GetBaseValue(prop)) ? 1.0f : 0.0f;

            bool previous = (bool)obj.GetValue(prop);
            bool value = (from + (to - from) * easedValue) >= 0.5f;
            obj.SetAnimatedValue(prop, value);
            bool next = (bool)obj.GetValue(prop);

            // if (!object.Equals(next, previous)) { Console.WriteLine($"{obj}.{prop} changed from {previous} to {next}"); }

            return previous != next;
        }

        // Public properties --------------------------------------------------

        #region From managed property

        /// <summary>
        /// Initial value of the property when animation starts (at StartTime)
        /// </summary>
        public bool From
        {
            get => (bool)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly ManagedProperty FromProperty = ManagedProperty.Register(typeof(AnimateBool),
            nameof(From),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = true });

        #endregion

        #region To managed property

        /// <summary>
        /// Final value of the property when animation ends (at EndTime)
        /// </summary>
        public bool To
        {
            get => (bool)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly ManagedProperty ToProperty = ManagedProperty.Register(typeof(AnimateBool),
            nameof(To),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = true });

        #endregion
    }
}
