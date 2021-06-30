using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public abstract class BaseMarkupExtension
    {
        public abstract void ProvideValue(ManagedObject @object, ManagedProperty property);
    }
}
