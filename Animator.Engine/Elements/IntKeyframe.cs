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
    /// A keyframe for a property of type int.
    /// </summary>
    public class IntKeyframe : NumericKeyframe
    {
        // Public methods -----------------------------------------------------

        public override object GetValue() => Value;

        public override object EvalValue(float fromTimeMs, object fromValue, float currentTimeMs)
        {
            float easedFactor = EasingFunctionRepository.Ease(EasingFunction, TimeCalculator.EvalAnimationFactor(fromTimeMs, (float)Time.TotalMilliseconds, currentTimeMs));
            var fromPoint = (int)fromValue;
            int result = (int)(fromPoint + (Value - fromPoint) * easedFactor);

            return result;
        }

        // Public properties --------------------------------------------------

        #region Value managed property

        /// <summary>
        /// Value of the keyframe.
        /// </summary>
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(IntKeyframe),
            nameof(Value),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion
    }
}
