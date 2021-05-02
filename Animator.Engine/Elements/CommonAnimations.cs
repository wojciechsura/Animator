using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Types;
using Animator.Engine.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Allows grouping a couple of animators, so that you don't have to specify
    /// various properties multiple times. CommonAnimations itself does nothing,
    /// but values of its properties are inherited by animators placed inside.
    /// </summary>
    [ContentProperty(nameof(CommonAnimations.Animations))]
    public class CommonAnimations : BaseAnimator
    {
        // Public methods -----------------------------------------------------

        public override void ApplyAnimation(float timeMs)
        {
            foreach (var animation in Animations)
                animation.ApplyAnimation(timeMs);
        }

        public override void ResetAnimation()
        {
            foreach (var animation in Animations)
                animation.ResetAnimation();
        }

        // Public properties --------------------------------------------------


        #region TargetName managed property

        /// <summary>
        /// Defines name of an object, which property should be modified.
        /// </summary>
        public string TargetName
        {
            get => (string)GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        public static readonly ManagedProperty TargetNameProperty = ManagedProperty.Register(typeof(CommonAnimations),
            nameof(TargetName),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true, Inheritable = true, InheritedFromParent = true });

        #endregion

        #region Path managed property

        /// <summary>
        /// Defines path to a property, starting at the object pointed
        /// to by TargetName. Path may be either a single property,
        /// for example <code>Position</code>, or a chain of properties,
        /// leading through subsequent object, like <code>Pen.Color</code>.
        /// </summary>
        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly ManagedProperty PathProperty = ManagedProperty.Register(typeof(CommonAnimations),
            nameof(Path),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true, Inheritable = true, InheritedFromParent = true });

        #endregion

        #region StartTime managed property

        /// <summary>
        /// Defines time since start of scene, when value of
        /// a property should start being changed.
        /// </summary>
        public TimeSpan StartTime
        {
            get => (TimeSpan)GetValue(StartTimeProperty);
            set => SetValue(StartTimeProperty, value);
        }

        public static readonly ManagedProperty StartTimeProperty = ManagedProperty.Register(typeof(CommonAnimations),
            nameof(StartTime),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { 
                DefaultValue = TimeSpan.FromMilliseconds(0), 
                ValueChangedHandler = HandleStartTimeChanged,
                NotAnimatable = true,
                Inheritable = true,
                InheritedFromParent = true
            });

        private static void HandleStartTimeChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            sender.CoerceValue(EndTimeProperty);
        }

        #endregion

        #region EndTime managed property

        /// <summary>
        /// Defines time since start of scene, when value of
        /// a property reaches its final value.
        /// </summary>
        public TimeSpan EndTime
        {
            get => (TimeSpan)GetValue(EndTimeProperty);
            set => SetValue(EndTimeProperty, value);
        }

        public static readonly ManagedProperty EndTimeProperty = ManagedProperty.Register(typeof(CommonAnimations),
            nameof(EndTime),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromMilliseconds(0), 
                CoerceValueHandler = CoerceEndTime,
                NotAnimatable = true,
                Inheritable = true,
                InheritedFromParent = true
            });

        private static object CoerceEndTime(ManagedObject obj, object baseValue)
        {
            var startTime = (TimeSpan)obj.GetFinalBaseValue(StartTimeProperty);
            var endTime = (TimeSpan)baseValue;

            if (startTime < endTime)
                return endTime;
            else
                return startTime;
        }

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
        /// </summary>
        public EasingFunction EasingFunction
        {
            get => (EasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        public static readonly ManagedProperty EasingFunctionProperty = ManagedProperty.Register(typeof(CommonAnimations),
            nameof(EasingFunction),
            typeof(EasingFunction),
            new ManagedSimplePropertyMetadata { 
                DefaultValue = EasingFunction.Linear,
                NotAnimatable = true,
                Inheritable = true, 
                InheritedFromParent = true });

        #endregion

        /// <summary>
        /// A short-hand property, which allows you to specify
        /// animation's duration instead of its end time.
        /// </summary>
        [DoNotDocument]
        public TimeSpan Duration
        {
            get => EndTime - StartTime;
            set => EndTime = StartTime + value;
        }

        #region Animations managed collection

        /// <summary>
        /// Defines a list of grouped animations.
        /// </summary>
        public ManagedCollection<BaseAnimator> Animations
        {
            get => (ManagedCollection<BaseAnimator>)GetValue(AnimationsProperty);
        }

        public static readonly ManagedProperty AnimationsProperty = ManagedProperty.RegisterCollection(typeof(CommonAnimations),
            nameof(Animations),
            typeof(ManagedCollection<BaseAnimator>));

        #endregion
    }
}
