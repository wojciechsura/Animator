using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    [DebuggerDisplay("ManagedSimpleProperty {Name} of type {Type.Name}")]    
    public class ManagedSimpleProperty : ManagedValueProperty
    {
        private readonly ManagedSimplePropertyMetadata metadata;

        protected override BasePropertyMetadata ProvideBaseMetadata() => metadata;

        protected override BaseValuePropertyMetadata ProvideBaseValueMetadata() => metadata;

        internal ManagedSimpleProperty(Type ownerClassType, string name, Type propertyType, ManagedSimplePropertyMetadata metadata)
            : base(ownerClassType, name, propertyType)
        {
            if (!propertyType.IsValueType && propertyType != typeof(string))
                throw new InvalidOperationException($"Only value-type properties can be registered with ManagedProperty.Register. If you need a reference property, use ManagedProperty.RegisterReference insted.");

            this.metadata = metadata;
        }

        public new ManagedSimplePropertyMetadata Metadata => metadata;
    }
}
