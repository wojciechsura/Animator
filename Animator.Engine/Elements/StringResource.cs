using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class StringResource : Resource
    {
        public override object GetValue() => Value;

        #region Value managed property

        /// <summary>Value of this resource</summary>
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(StringResource),
            nameof(Value),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = String.Empty });

        #endregion
    }
}
