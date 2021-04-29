using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
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

        public TimeSpan Duration
        {
            get => EndTime - StartTime;
            set => EndTime = StartTime + value;
        }

        #region Animations managed collection

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
