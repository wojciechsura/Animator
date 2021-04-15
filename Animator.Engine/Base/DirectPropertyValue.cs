using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    internal class DirectPropertyValue : BasePropertyValue
    {
        private readonly object value;

        protected override object ProvideValue()
        {
            return value;
        }

        public DirectPropertyValue(object value)
        {
            this.value = value;
        }
    }
}
