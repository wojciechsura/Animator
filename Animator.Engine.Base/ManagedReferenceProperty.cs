using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    [DebuggerDisplay("ManagedReferenceProperty {Name} of type {Type.Name}")]
    public class ManagedReferenceProperty : ManagedValueProperty
    {
        private readonly ManagedReferencePropertyMetadata metadata;

        protected override BasePropertyMetadata ProvideBaseMetadata() => metadata;

        protected override BaseValuePropertyMetadata ProvideBaseValueMetadata() => metadata;

        internal ManagedReferenceProperty(Type ownerClassType, string name, Type propertyType, ManagedReferencePropertyMetadata metadata)
            : base(ownerClassType, name, propertyType)
        {
            if (propertyType.IsValueType)
                throw new InvalidOperationException($"Only reference-type properties can be registered with ManagedProperty.RegisterReference. If you need a value-type property, use ManagedProperty.Register insted.");

            this.metadata = metadata;
        }

        public new ManagedReferencePropertyMetadata Metadata => metadata;
    }
}
