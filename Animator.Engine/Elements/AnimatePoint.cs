﻿using Animator.Engine.Animation.Maths;
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
    /// Allows applying an animation to an PointF property of some
    /// object. It is meant to be used only if you want to apply
    /// single animation per whole scene. Otherwise, use Storyboard
    /// and PointKeyframe to define the whole animation.
    /// </summary>
    public partial class AnimatePoint : AnimateNumericPropertyInTime
    {
        // Public methods -----------------------------------------------------

        public override bool ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);

            var factor = TimeCalculator.EvalAnimationFactor((float)StartTime.TotalMilliseconds, (float)EndTime.TotalMilliseconds, timeMs);
            float easedValue = Ease(factor);

            PointF from = IsPropertySet(FromProperty) ? From : (PointF)obj.GetBaseValue(prop);
            PointF to = IsPropertySet(ToProperty) ? To : (PointF)obj.GetBaseValue(prop);

            var previous = (PointF)obj.GetValue(prop);
            var value = new PointF(from.X + (to.X - from.X) * easedValue, from.Y + (to.Y - from.Y) * easedValue);
            obj.SetAnimatedValue(prop, value);
            var next = (PointF)obj.GetValue(prop);

            // if (!object.Equals(next, previous)) { Console.WriteLine($"{obj}.{prop} changed from {previous} to {next}"); }

            return previous != next;
        }

        // Public properties --------------------------------------------------


        #region From managed property

        /// <summary>
        /// Initial value of the property when animation starts (at StartTime)
        /// </summary>
        public PointF From
        {
            get => (PointF)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly ManagedProperty FromProperty = ManagedProperty.Register(typeof(AnimatePoint),
            nameof(From),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region To managed property

        /// <summary>
        /// Final value of the property when animation ends (at EndTime)
        /// </summary>
        public PointF To
        {
            get => (PointF)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly ManagedProperty ToProperty = ManagedProperty.Register(typeof(AnimatePoint),
            nameof(To),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion
    }
}
