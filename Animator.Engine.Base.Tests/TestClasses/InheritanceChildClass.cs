using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    public class InheritanceChildClass : ManagedObject
    {

        #region Value managed property

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(InheritanceChildClass),
            nameof(Value),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 20, InheritedFromParent = true });

        #endregion


        #region Value2 managed property

        public int Value2
        {
            get => (int)GetValue(Value2Property);
            set => SetValue(Value2Property, value);
        }

        public static readonly ManagedProperty Value2Property = ManagedProperty.Register(typeof(InheritanceChildClass),
            nameof(Value2),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 20, InheritedFromParent = true, CoerceValueHandler = CoerceValue2 });

        private static object CoerceValue2(ManagedObject obj, object baseValue)
        {
            return Math.Min(100, (int)baseValue);
        }

        #endregion
    }
}
