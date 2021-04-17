using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedCollectionProperty : ManagedProperty
    {
        private readonly ManagedCollectionMetadata metadata;

        public ManagedCollectionProperty(Type ownerClassType, string name, Type propertyType, ManagedCollectionMetadata metadata)
            : base(ownerClassType, name, propertyType)
        {
            this.metadata = metadata;
        }

        public ManagedCollectionMetadata Metadata => metadata;
    }
}
