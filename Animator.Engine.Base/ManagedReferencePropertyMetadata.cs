using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedReferencePropertyMetadata : BaseValuePropertyMetadata
    {
        private static readonly ManagedReferencePropertyMetadata defaultMetadata = new ManagedReferencePropertyMetadata();

        internal static ManagedReferencePropertyMetadata DefaultFor(Type propertyType) => defaultMetadata;

        public ManagedReferencePropertyMetadata(PropertyValueChangedDelegate valueChangedHandler = null, TypeSerializer customSerializer = null)
            : base(valueChangedHandler, customSerializer)
        {

        }
    }
}
