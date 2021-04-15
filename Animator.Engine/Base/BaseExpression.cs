using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public abstract class BaseExpression
    {
        protected readonly ManagedObject obj;
        protected readonly ManagedProperty property;

        public BaseExpression(ManagedObject obj, ManagedProperty property)
        {
            this.obj = obj;
            this.property = property;
        }

        public abstract object GetValue();
    }
}
