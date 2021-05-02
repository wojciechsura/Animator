using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// A keyframe for a property of type PointF.
    /// </summary>
    public class PointKeyframe : BaseNumericKeyframe
    {
        // Public methods -----------------------------------------------------

        public override object GetValue() => Value;

        public override object EvalValue(float fromTimeMs, object fromValue, float currentTimeMs)
        {
            float easedFactor = EasingFunctionRepository.Ease(EasingFunction, TimeCalculator.EvalAnimationFactor(fromTimeMs, (float)Time.TotalMilliseconds, currentTimeMs));

            var fromPoint = (PointF)fromValue;

            var result = new PointF(fromPoint.X + (Value.X - fromPoint.X) * easedFactor,
                fromPoint.Y + (Value.Y - fromPoint.Y) * easedFactor);

            return result;
        }

        // Public properties --------------------------------------------------

        #region Value managed property

        /// <summary>
        /// Value of the keyframe.
        /// </summary>
        public PointF Value
        {
            get => (PointF)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(PointKeyframe),
            nameof(Value),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}
