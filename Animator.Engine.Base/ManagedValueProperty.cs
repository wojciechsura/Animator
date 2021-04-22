using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public abstract class ManagedValueProperty : ManagedProperty
    {
        protected abstract BaseValuePropertyMetadata ProvideBaseValueMetadata();

        public ManagedValueProperty(Type ownerClassType, string name, Type type) 
            : base(ownerClassType, name, type)
        {

        }

        public new BaseValuePropertyMetadata Metadata => ProvideBaseValueMetadata();
    }
}
