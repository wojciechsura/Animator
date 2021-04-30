using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class FloatKeyframe : BaseNumericKeyframe
    {
        // Public methods -----------------------------------------------------

        public override object GetValue() => Value;

        public override object EvalValue(float fromTimeMs, object fromValue, float currentTimeMs)
        {
            float easedFactor = EasingFunctionRepository.Ease(EasingFunction, TimeCalculator.EvalAnimationFactor(fromTimeMs, (float)Time.TotalMilliseconds, currentTimeMs));
            var fromPoint = (float)fromValue;
            float result = fromPoint + (Value - fromPoint) * easedFactor;

            return result;
        }

        // Public properties --------------------------------------------------

        #region Value managed property

        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(FloatKeyframe),
            nameof(Value),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}
