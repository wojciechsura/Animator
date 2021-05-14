using System;

namespace Animator.Editor.Configuration
{
    public abstract class BaseCollectionItem : BaseItemContainer
    {
        // Private fields -----------------------------------------------------

        private BaseItemCollection parentCollection;

        // Internal methods ---------------------------------------------------

        internal void AttachToCollection(BaseItemCollection collection)
        {
            if (parentCollection != null)
                throw new InvalidOperationException("Already in collection!");

            parentCollection = collection;
        }

        internal void DetachFromCollection(BaseItemCollection collection)
        {
            if (collection != parentCollection)
                throw new InvalidOperationException("Cannot detach - requesting collection is not parent collection of this item!");
            if (parentCollection == null)
                throw new InvalidOperationException("Already detached!");

            parentCollection = null;
        }

        // Public methods -----------------------------------------------------

        public BaseCollectionItem(string name)
            : base(name)
        {

        }

        // Public properties --------------------------------------------------

        public BaseItemCollection ParentCollection
        {
            get
            {
                return parentCollection;
            }
        }
    }
}