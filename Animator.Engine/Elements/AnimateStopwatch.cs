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
    public class AnimateStopwatch : AnimatePropertyInTime
    {
        public override void ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);

            TimeSpan timeSpan;

            if (timeMs < StartTime.TotalMilliseconds)
                timeSpan = TimeSpan.Zero;
            else if (timeMs > EndTime.TotalMilliseconds)
                timeSpan = TimeSpan.FromMilliseconds(EndTime.TotalMilliseconds - StartTime.TotalMilliseconds);
            else
                timeSpan = TimeSpan.FromMilliseconds(timeMs - StartTime.TotalMilliseconds);

            obj.SetAnimatedValue(prop, timeSpan.ToString(Format));
        }

        // Public properties --------------------------------------------------

        #region Format managed property

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
