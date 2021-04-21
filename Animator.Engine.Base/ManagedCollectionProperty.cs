using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    [DebuggerDisplay("ManagedCollectionProperty {Name} of type {Type.Name}")]
    public class ManagedCollectionProperty : ManagedProperty
    {
        private readonly ManagedCollectionMetadata metadata;

        protected override BasePropertyMetadata ProvideBaseMetadata() => metadata;

        public ManagedCollectionProperty(Type ownerClassType, string name, Type propertyType, ManagedCollectionMetadata metadata)
            : base(ownerClassType, name, propertyType)
        {
            this.metadata = metadata;
        }

        public new ManagedCollectionMetadata Metadata => metadata;
    }
}
