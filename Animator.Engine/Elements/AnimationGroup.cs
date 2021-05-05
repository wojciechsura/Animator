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
    [ContentProperty(nameof(AnimationGroup.Animations))]
    public class AnimationGroup : BaseAnimator
    {
        // Public methods -----------------------------------------------------

        public override void ApplyAnimation(float timeMs)
        {
            if (IsPropertySet(TimeOffsetProperty))
                timeMs -= (float)TimeOffset.TotalMilliseconds;

            foreach (var animation in Animations)
                animation.ApplyAnimation(timeMs);
        }

        public override void ResetAnimation()
        {
            foreach (var animation in Animations)
                animation.ResetAnimation();
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

        public static readonly ManagedProperty PropertyRefProperty = ManagedProperty.Register(typeof(AnimationGroup),
            nameof(PropertyRef),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = null, Inheritable = true, InheritedFromParent = true });

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

        public static readonly ManagedProperty StartTimeProperty = ManagedProperty.Register(typeof(AnimationGroup),
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

        public static readonly ManagedProperty EndTimeProperty = ManagedProperty.Register(typeof(AnimationGroup),
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

        public static readonly ManagedProperty EasingFunctionProperty = ManagedProperty.Register(typeof(AnimationGroup),
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

        public static readonly ManagedProperty AnimationsProperty = ManagedProperty.RegisterCollection(typeof(AnimationGroup),
            nameof(Animations),
            typeof(ManagedCollection<BaseAnimator>));

        #endregion


        #region TimeOffset managed property

        /// <summary>
        /// Offsets all inner animations by a specific duration. Negative values makes them happen earlier,
        /// positive values - later.
        /// </summary>
        public TimeSpan TimeOffset
        {
            get => (TimeSpan)GetValue(TimeOffsetProperty);
            set => SetValue(TimeOffsetProperty, value);
        }

        public static readonly ManagedProperty TimeOffsetProperty = ManagedProperty.Register(typeof(AnimationGroup),
            nameof(TimeOffset),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromSeconds(0) });

        #endregion
    }
}
