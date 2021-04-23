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

        /// <summary>Used to construct the collection in custom way 
        /// (ie. when it does not contain public, parameterless
        /// contstructor)</summary>
        public InitializeCollectionDelegate CollectionInitializer { get; init; }

        /// <summary>Custom serializer allows the collection to 
        /// be serialized into an XML attribute</summary>
        public TypeSerializer CustomSerializer { get; init; }

        /// <summary>Handler for collection change events</summary>
        public ManagedCollectionChangedDelegate CollectionChangedHandler { get; init; }
    }
}
