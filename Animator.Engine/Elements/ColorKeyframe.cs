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
    /// A keyframe for a property of type Color.
    /// </summary>
    public partial class ColorKeyframe : NumericKeyframe
    {
        // Public methods -----------------------------------------------------

        public override object GetValue() => Color;

        public override object EvalValue(float fromTimeMs, object fromValue, float currentTimeMs)
        {
            float easedFactor = EasingFunctionRepository.Ease(EasingFunction, TimeCalculator.EvalAnimationFactor(fromTimeMs, (float)Time.TotalMilliseconds, currentTimeMs));
            var fromColor = (Color)fromValue;

            var a = (int)Math.Min(255, Math.Max(0, fromColor.A + (Color.A - fromColor.A) * easedFactor));
            var r = (int)Math.Min(255, Math.Max(0, fromColor.R + (Color.R - fromColor.R) * easedFactor));
            var g = (int)Math.Min(255, Math.Max(0, fromColor.G + (Color.G - fromColor.G) * easedFactor));
            var b = (int)Math.Min(255, Math.Max(0, fromColor.B + (Color.B - fromColor.B) * easedFactor));

            Color result = Color.FromArgb(a, r, g, b);

            return result;
        }

        // Public properties --------------------------------------------------


        #region Color managed property

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly ManagedProperty ColorProperty = ManagedProperty.Register(typeof(ColorKeyframe),
            nameof(Color),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.White });

        #endregion
    }
}
