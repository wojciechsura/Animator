using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    internal class PureAnimatedValue : BasePropertyValue
    {
        protected override object ProvideValue()
        {
            // By design
            throw new NotImplementedException();
        }
    }
}
