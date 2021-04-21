using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedAnimatedProperty : ManagedProperty
    {
        private readonly ManagedAnimatedPropertyMetadata metadata;

        protected override BasePropertyMetadata ProvideBaseMetadata() => metadata;

        internal ManagedAnimatedProperty(Type ownerClassType, string name, Type propertyType, ManagedAnimatedPropertyMetadata metadata)
            : base(ownerClassType, name, propertyType)
        {
            this.metadata = metadata;
        }

        public new ManagedAnimatedPropertyMetadata Metadata => metadata;
    }
}
