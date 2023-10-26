using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Allows applying an animation to a float property of some
    /// object. It is meant to be used only if you want to apply
    /// single animation per whole scene. Otherwise, use Storyboard
    /// and FloatKeyframe to define the whole animation.
    /// </summary>
    public partial class AnimateFloat : AnimateNumericPropertyInTime
    {
        // Public methods -----------------------------------------------------

        public override bool ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);

            var factor = TimeCalculator.EvalAnimationFactor((float)StartTime.TotalMilliseconds, (float)EndTime.TotalMilliseconds, timeMs);
            var easedValue = Ease(factor);

            float from = IsPropertySet(FromProperty) ? From : (float)obj.GetBaseValue(prop);
            float to = IsPropertySet(ToProperty) ? To : (float)obj.GetBaseValue(prop);

            var previous = (float)obj.GetValue(prop);
            var value = from + (to - from) * easedValue;
            obj.SetAnimatedValue(prop, value);
            var next = (float)obj.GetValue(prop);

            // if (!object.Equals(next, previous)) { Console.WriteLine($"{obj}.{prop} changed from {previous} to {next}"); }

            return previous != next;
        }

        // Public properties --------------------------------------------------
      
        #region From managed property

        /// <summary>
        /// Initial value of the property when animation starts (at StartTime)
        /// </summary>
        public float From
        {
            get => (float)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly ManagedProperty FromProperty = ManagedProperty.Register(typeof(AnimateFloat),
            nameof(From),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion

        #region To managed property

        /// <summary>
        /// Final value of the property when animation ends (at EndTime)
        /// </summary>
        public float To
        {
            get => (float)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly ManagedProperty ToProperty = ManagedProperty.Register(typeof(AnimateFloat),
            nameof(To),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}
