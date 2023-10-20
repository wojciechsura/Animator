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
    /// Allows applying an animation to a float property of some
    /// object. It is meant to be used only if you want to apply
    /// single animation per whole scene. Otherwise, use Storyboard
    /// and ColorKeyframe to define the whole animation.
    public class AnimateColor : AnimateNumericPropertyInTime
    {
        public override bool ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);

            var factor = TimeCalculator.EvalAnimationFactor((float)StartTime.TotalMilliseconds, (float)EndTime.TotalMilliseconds, timeMs);
            
            var easedValue = Ease(factor);

            Color from = IsPropertySet(FromProperty) ? From : (Color)obj.GetBaseValue(prop);
            Color to = IsPropertySet(ToProperty) ? To : (Color)obj.GetBaseValue(prop);

            var a = (int)Math.Min(255, Math.Max(0, (from.A + (to.A - from.A) * easedValue)));
            var r = (int)Math.Min(255, Math.Max(0, (from.R + (to.R - from.R) * easedValue)));
            var g = (int)Math.Min(255, Math.Max(0, (from.G + (to.G - from.G) * easedValue)));
            var b = (int)Math.Min(255, Math.Max(0, (from.B + (to.B - from.B) * easedValue)));

            var previous = (Color)obj.GetValue(prop);
            var value = Color.FromArgb(a, r, g, b);
            obj.SetAnimatedValue(prop, value);
            var next = (Color)obj.GetValue(prop);

            // if (!object.Equals(next, previous)) { Console.WriteLine($"{obj}.{prop} changed from {previous} to {next}"); }

            return previous != next;
        }

        // Public properties --------------------------------------------------


        #region From managed property

        /// <summary>
        /// Initial color
        /// </summary>
        public Color From
        {
            get => (Color)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly ManagedProperty FromProperty = ManagedProperty.Register(typeof(AnimateColor),
            nameof(From),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.White });

        #endregion

        #region To managed property

        /// <summary>
        /// Final color
        /// </summary>
        public Color To
        {
            get => (Color)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly ManagedProperty ToProperty = ManagedProperty.Register(typeof(AnimateColor),
            nameof(To),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.Black });

        #endregion
    }
}
