using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public partial class FloatVariable : Variable
    {
        #region Value managed property

        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(FloatVariable),
            nameof(Value),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, NotAnimatable = true });

        #endregion
    }
}
