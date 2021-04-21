using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedCollectionMetadata : BasePropertyMetadata
    {
        public ManagedCollectionMetadata(Func<object> collectionInitializer, TypeSerializer customSerializer = null)
        {
            CollectionInitializer = collectionInitializer;
            CustomSerializer = customSerializer;
        }

        public Func<object> CollectionInitializer { get; }
        public TypeSerializer CustomSerializer { get; }

        internal static ManagedCollectionMetadata DefaultFor(Type propertyType)
        {
            return new ManagedCollectionMetadata(() => Activator.CreateInstance(propertyType));
        }
    }
}
