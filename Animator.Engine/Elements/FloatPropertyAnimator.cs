using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class FloatPropertyAnimator : PropertyAnimator
    {
        // Public properties --------------------------------------------------


        #region StartTime managed property

        public TimeSpan StartTime
        {
            get => (TimeSpan)GetValue(StartTimeProperty);
            set => SetValue(StartTimeProperty, value);
        }

        public static readonly ManagedProperty StartTimeProperty = ManagedProperty.Register(typeof(FloatPropertyAnimator),
            nameof(StartTime),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata(TimeSpan.FromMilliseconds(0), HandleStartTimeChanged));
        
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

        public static readonly ManagedProperty EndTimeProperty = ManagedProperty.Register(typeof(FloatPropertyAnimator),
            nameof(EndTime),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata(TimeSpan.FromMilliseconds(0), null, CoerceEndTime));

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

        public TimeSpan Duration
        {
            get => EndTime - StartTime;
            set => EndTime = StartTime + value;
        }

        #region From managed property

        public float From
        {
            get => (float)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly ManagedProperty FromProperty = ManagedProperty.Register(typeof(FloatPropertyAnimator),
            nameof(From),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion

        #region To managed property

        public float To
        {
            get => (float)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly ManagedProperty ToProperty = ManagedProperty.Register(typeof(FloatPropertyAnimator),
            nameof(To),
            typeof(float),
            new ManagedSimplePropertyMetadata(0.0f));

        #endregion
    }
}
