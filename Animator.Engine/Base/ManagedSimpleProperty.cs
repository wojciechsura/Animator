using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedSimpleProperty : ManagedProperty
    {
        private readonly ManagedSimplePropertyMetadata metadata;

        internal ManagedSimpleProperty(Type ownerClassType, string name, Type propertyType, ManagedSimplePropertyMetadata metadata)
            : base(ownerClassType, name, propertyType)
        {
            this.metadata = metadata;
        }

        public ManagedSimplePropertyMetadata Metadata => metadata;
    }
}
