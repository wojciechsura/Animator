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
    /// A keyframe for a property of type float.
    /// </summary>
    public partial class BoolKeyframe : NumericKeyframe
    {
        // Public methods -----------------------------------------------------

        public override object GetValue() => Value;

        public override object EvalValue(float fromTimeMs, object fromValue, float currentTimeMs)
        {
            float easedFactor = EasingFunctionRepository.Ease(EasingFunction, TimeCalculator.EvalAnimationFactor(fromTimeMs, (float)Time.TotalMilliseconds, currentTimeMs));
            var fromPoint = (bool)fromValue ? 1.0f : 0.0f;
            bool result = (fromPoint + ((Value ? 1.0f : 0.0f) - fromPoint) * easedFactor) > 0.5f;

            return result;
        }

        // Public properties --------------------------------------------------


        #region Value managed property

        /// <summary>
        /// Value of the keyframe.
        /// </summary>
        public bool Value
        {
            get => (bool)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(BoolKeyframe),
            nameof(Value),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = true });

        #endregion        
    }
}
