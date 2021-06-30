using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class FloatResource : Resource
    {
        public override object GetValue() => Value;

        #region Value managed property

        /// <summary>Value of this resource</summary>
        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(FloatResource),
            nameof(Value),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}
