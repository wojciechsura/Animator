using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class TimeResource : Resource
    {
        public override object GetValue() => Value;

        #region Value managed property

        /// <summary>Value of this resource</summary>
        public TimeSpan Value
        {
            get => (TimeSpan)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(TimeResource),
            nameof(Value),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.Zero });

        #endregion
    }
}
