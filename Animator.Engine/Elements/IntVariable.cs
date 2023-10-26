using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public partial class IntVariable : Variable
    {
        #region Value managed property

        public int Value
        {
            get => (int)GetValue(ValueProperty); 
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(IntVariable),
            nameof(Value),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0, NotAnimatable = true });

        #endregion
    }
}
