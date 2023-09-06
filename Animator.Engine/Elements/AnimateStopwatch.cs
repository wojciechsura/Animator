using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Allows applying an animation to a string property of some
    /// object. The value will be set to a stopwatch started
    /// at StartTime and stopped on EndTime.
    /// </summary>
    public class AnimateStopwatch : AnimatePropertyInTime
    {
        public override bool ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);

            TimeSpan timeSpan;

            if (timeMs < StartTime.TotalMilliseconds)
                timeSpan = TimeSpan.Zero;
            else if (timeMs > EndTime.TotalMilliseconds)
                timeSpan = TimeSpan.FromMilliseconds(EndTime.TotalMilliseconds - StartTime.TotalMilliseconds);
            else
                timeSpan = TimeSpan.FromMilliseconds(timeMs - StartTime.TotalMilliseconds);

            var previous = (string)obj.GetValue(prop);
            var value = timeSpan.ToString(Format);
            obj.SetAnimatedValue(prop, value);
            var next = (string)obj.GetValue(prop);

            // if (!object.Equals(next, previous)) { Console.WriteLine($"{obj}.{prop} changed from {previous} to {next}"); }

            return previous != next;
        }

        // Public properties --------------------------------------------------

        #region Format managed property

        /// <summary>
        /// Specifies format of the stopwatch. Compatible
        /// with TimeSpan formatting strings.
        /// </summary>
        public string Format
        {
            get => (string)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }

        public static readonly ManagedProperty FormatProperty = ManagedProperty.Register(typeof(AnimateStopwatch),
            nameof(Format),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = "hh\\:mm\\:ss\\.ff" });

        #endregion
    }
}
