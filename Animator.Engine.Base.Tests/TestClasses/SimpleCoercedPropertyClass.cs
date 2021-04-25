using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    public class SimpleCoercedPropertyClass : ManagedObject
    {
        #region Max10 managed property

        public int Max10
        {
            get => (int)GetValue(Max10Property);
            set => SetValue(Max10Property, value);
        }

        public static readonly ManagedProperty Max10Property = ManagedProperty.Register(typeof(SimpleCoercedPropertyClass),
            nameof(Max10),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0, CoerceValueHandler = Max10PropertyCoerce });

        private static object Max10PropertyCoerce(ManagedObject obj, object baseValue)
        {
            if (baseValue is int iValue)
            {
                return Math.Min(10, iValue);
            }

            return baseValue;
        }

        #endregion
    }
}
