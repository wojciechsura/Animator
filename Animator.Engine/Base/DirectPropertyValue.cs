using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    internal class DirectPropertyValue : BasePropertyValue
    {
        private readonly object baseValue;

        public DirectPropertyValue(int propertyIndex, object baseValue)
            : base(propertyIndex)
        {
            this.baseValue = baseValue;
        }

        protected override object ProvideBaseValue() => baseValue;        
    }
}
