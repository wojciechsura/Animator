using Animator.Engine.Base;
using Animator.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all property animator, which apply
    /// animation to a property in specific period of time.
    /// </summary>
    public abstract class TimeDurationPropertyAnimator : PropertyAnimator
    {
        #region StartTime managed property

        /// <summary>
        /// Time, when animation should be started.
        /// </summary>
        public TimeSpan StartTime
        {
            get => (TimeSpan)GetValue(StartTimeProperty);
            set => SetValue(StartTimeProperty, value);
        }

        public static readonly ManagedProperty StartTimeProperty = ManagedProperty.Register(typeof(TimeDurationPropertyAnimator),
            nameof(StartTime),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromMilliseconds(0), ValueChangedHandler = HandleStartTimeChanged, InheritedFromParent = true });

        private static void HandleStartTimeChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            sender.CoerceValue(EndTimeProperty);
        }

        #endregion

        #region EndTime managed property

        /// <summary>
        /// Time, when animation should finish.
        /// </summary>
        public TimeSpan EndTime
        {
            get => (TimeSpan)GetValue(EndTimeProperty);
            set => SetValue(EndTimeProperty, value);
        }

        public static readonly ManagedProperty EndTimeProperty = ManagedProperty.Register(typeof(TimeDurationPropertyAnimator),
            nameof(EndTime),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromMilliseconds(0), CoerceValueHandler = CoerceEndTime, InheritedFromParent = true });

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

        [DoNotDocument]
        public TimeSpan Duration
        {
            get => EndTime - StartTime;
            set => EndTime = StartTime + value;
        }
    }
}
