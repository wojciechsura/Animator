using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedCollectionMetadata
    {
        private readonly Func<object> collectionInitializer;

        public ManagedCollectionMetadata(Func<object> collectionInitializer)
        {
            this.collectionInitializer = collectionInitializer;
        }

        public Func<object> CollectionInitializer => collectionInitializer;

        internal static ManagedCollectionMetadata DefaultFor(Type propertyType)
        {
            return new ManagedCollectionMetadata(() => Activator.CreateInstance(propertyType));
        }
    }
}
