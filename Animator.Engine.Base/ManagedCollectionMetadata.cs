using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public delegate ManagedCollection InitializeCollectionDelegate();

    public class ManagedCollectionChangedEventArgs : EventArgs
    {
        public ManagedCollectionChangedEventArgs(ManagedCollection collection,
            CollectionChange change,
            IList<object> itemsAdded,
            IList<object> itemsRemoved)
        {
            Collection = collection;
            Change = change;
            ItemsAdded = itemsAdded;
            ItemsRemoved = itemsRemoved;
        }

        public ManagedCollection Collection { get; }
        public CollectionChange Change { get; }
        public IList<object> ItemsAdded { get; }
        public IList<object> ItemsRemoved { get; }
    }

    public delegate void ManagedCollectionChangedDelegate(ManagedObject sender, ManagedCollectionChangedEventArgs args);

    public class ManagedCollectionMetadata : BasePropertyMetadata
    {
        // Private static fields ----------------------------------------------

        private static readonly ManagedCollectionMetadata DefaultMetadata = new ManagedCollectionMetadata();

        // Public static methods ----------------------------------------------

        public static ManagedCollectionMetadata DefaultFor(Type propertyType) => DefaultMetadata;

        // Public methods -----------------------------------------------------

        public ManagedCollectionMetadata()
        {

        }

        // Public properties --------------------------------------------------

        public InitializeCollectionDelegate CollectionInitializer { get; init; }
        public TypeSerializer CustomSerializer { get; init; }
        public ManagedCollectionChangedDelegate CollectionChangedHandler { get; init; }
    }
}
