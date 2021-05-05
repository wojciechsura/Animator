using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Types;
using Animator.Engine.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Allows defining complex animations consisting of
    /// multiple keyframes. Storyboard can contain definitions
    /// of multiple animations (eg. multiple properties). You can
    /// also use its properties, so that keyframes will inherit 
    /// them.
    /// </summary>
    [ContentProperty(nameof(Storyboard.Keyframes))]
    public class Storyboard : BaseAnimator
    {
        // Public methods -----------------------------------------------------

        public override void ApplyAnimation(float timeMs)
        {
            var groups = Keyframes.GroupBy(k => k.PropertyRef);
            
            foreach (var group in groups)
            {
                var keyframes = group.OrderBy(k => k.Time).ToList();

                if (keyframes.Any(k => k.GetType() != keyframes[0].GetType()))
                    throw new AnimationException($"All keyframes for property reference {keyframes[0].PropertyRef} must be of the same type!");

                (var obj, var prop) = AnimatedObject.FindProperty(group.Key);

                if (prop is not ManagedSimpleProperty simpleProperty)
                    throw new AnimationException($"Property {prop.Name} of object {obj.GetType().Name} is not simple property and thus can not be animated!");

                int i = -1;
                while (i + 1 < keyframes.Count && keyframes[i + 1].Time.TotalMilliseconds <= timeMs)
                    i++;

                if (i == -1)
                {
                    // Before first keyframe
                    object fromValue = simpleProperty.Metadata.DefaultValue;
                    object value = keyframes[0].EvalValue(0, fromValue, timeMs);
                    obj.SetAnimatedValue(prop, value);
                }
                else if (i == keyframes.Count - 1)
                {
                    // Past last keyframe
                    object value = keyframes[i].GetValue();
                    obj.SetAnimatedValue(prop, value);
                }
                else
                {
                    object fromValue = keyframes[i].GetValue();
                    TimeSpan fromTime = keyframes[i].Time;
                    object value = keyframes[i + 1].EvalValue((float)fromTime.TotalMilliseconds, fromValue, timeMs);
                    obj.SetAnimatedValue(prop, value);
                }
            }
        }

        public override void ResetAnimation()
        {
            var groups = Keyframes.GroupBy(k => k.PropertyRef);

            foreach (var group in groups)
            {
                (var obj, var prop) = AnimatedObject.FindProperty(group.Key);

                if (prop is not ManagedSimpleProperty simpleProperty)
                    throw new AnimationException($"Property {prop.Name} of object {obj.GetType().Name} is not simple property and thus can not be animated!");

                obj.ClearAnimatedValue(prop);
            }
        }

        // Public properties --------------------------------------------------

        #region PropertyRef managed property

        /// <summary>
        /// Reference to a property, relative to object owning this animator.
        /// Subsequent elements must be separated by dots. You may call elements
        /// from collections as well, as long as they have their Name property
        /// set and it is unique in this collection.
        /// </summary>
        /// <example>
        /// <code>PropertyRef=\"MyRectangle.Pen.Color\"</code>
        /// </example>
        public string PropertyRef
        {
            get => (string)GetValue(PropertyRefProperty);
            set => SetValue(PropertyRefProperty, value);
        }

        public static readonly ManagedProperty PropertyRefProperty = ManagedProperty.Register(typeof(Storyboard),
            nameof(PropertyRef),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = null, Inheritable = true, InheritedFromParent = true });

        #endregion


        #region EasingFunction managed property

        /// <summary>
        /// Defines easing function used for animation. The role of
        /// easing function is to define, how value should change
        /// between previous and current keyframe. Possible
        /// easing functions include the following:
        /// 
        /// <ul>
        ///     <li>Linear</li>
        ///     <li>EaseSineSpeedUp</li>
        ///     <li>EaseSineSlowDown</li>
        ///     <li>EaseSineBoth</li>
        ///     <li>EaseQuadSpeedUp</li>
        ///     <li>EaseQuadSlowDown</li>
        ///     <li>EaseQuadBoth</li>
        ///     <li>EaseCubicSpeedUp</li>
        ///     <li>EaseCubicSlowDown</li>
        ///     <li>EaseCubicBoth</li>
        ///     <li>EaseQuartSpeedUp</li>
        ///     <li>EaseQuartSlowDown</li>
        ///     <li>EaseQuartBoth</li>
        ///     <li>EaseBackSpeedUp</li>
        ///     <li>EaseBackSlowDown</li>
        ///     <li>EaseBackBoth</li>
        /// </ul>
        ///
        /// Storyboard does not use this value, but it may be set, so that
        /// keyframes will inherit it (for a shorthand notation).
        /// </summary>
        public EasingFunction EasingFunction
        {
            get => (EasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        public static readonly ManagedProperty EasingFunctionProperty = ManagedProperty.Register(typeof(Storyboard),
            nameof(EasingFunction),
            typeof(EasingFunction),
            new ManagedSimplePropertyMetadata { DefaultValue = EasingFunction.Linear, Inheritable = true, InheritedFromParent = true });

        #endregion

        #region Keyframes managed collection

        /// <summary>
        /// List of keyframes, which define complex animation.
        /// </summary>
        public ManagedCollection<BaseKeyframe> Keyframes
        {
            get => (ManagedCollection<BaseKeyframe>)GetValue(KeyframesProperty);
        }

        public static readonly ManagedProperty KeyframesProperty = ManagedProperty.RegisterCollection(typeof(Storyboard),
            nameof(Keyframes),
            typeof(ManagedCollection<BaseKeyframe>));

        #endregion
    }
}
