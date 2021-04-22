using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public enum CollectionChange
    {
        ItemsAdded,
        ItemsRemoved,
        ItemsReplaced
    }

    public class CollectionChangedEventArgs : EventArgs
    {
        public CollectionChangedEventArgs(CollectionChange change, List<object> itemsAdded, List<object> itemsRemoved)
        {
            Change = change;
            ItemsAdded = itemsAdded;
            ItemsRemoved = itemsRemoved;
        }

        public CollectionChange Change { get; }
        public List<object> ItemsAdded { get; }
        public List<object> ItemsRemoved { get; }
    }

    public delegate void CollectionChangedDelegate(ManagedCollection sender, CollectionChangedEventArgs args);

    public abstract class ManagedCollection : IList
    {
        // Protected methods --------------------------------------------------

        protected abstract IList GetList();

        protected virtual void OnCollectionChanged(CollectionChange change, List<object> itemsAdded, List<object> itemsRemoved)
        {
            CollectionChanged?.Invoke(this, new CollectionChangedEventArgs(change, itemsAdded, itemsRemoved));
        }

        // IList implementation -----------------------------------------------

        int IList.Add(object value)
        {
            var list = GetList();
            var result = list.Add(value);

            OnCollectionChanged(CollectionChange.ItemsAdded, new List<object> { value }, null);

            return result;
        }

        void IList.Clear()
        {
            var list = GetList();

            var removedItems = new List<object>();
            foreach (object obj in list)
                removedItems.Add(obj);

            list.Clear();

            OnCollectionChanged(CollectionChange.ItemsRemoved, null, removedItems);
        }

        bool IList.Contains(object value)
        {
            var list = GetList();

            return list.Contains(value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            var list = GetList();

            list.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var list = GetList();

            return list.GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            var list = GetList();

            return list.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            var list = GetList();

            list.Insert(index, value);

            OnCollectionChanged(CollectionChange.ItemsAdded, new List<object> { value }, null);
        }

        void IList.Remove(object value)
        {
            var list = GetList();

            if (list.Contains(value))
            {
                list.Remove(value);

                OnCollectionChanged(CollectionChange.ItemsRemoved, null, new List<object> { value });
            }
        }

        void IList.RemoveAt(int index)
        {
            var list = GetList();

            var item = list[index];
            list.RemoveAt(index);

            OnCollectionChanged(CollectionChange.ItemsRemoved, null, new List<object> { item });

        }

        object IList.this[int index] 
        { 
            get => GetList()[index];
            set
            {
                var oldValue = GetList()[index];
                GetList()[index] = value;

                OnCollectionChanged(CollectionChange.ItemsReplaced, new List<object> { oldValue }, new List<object> { value });
            }
        }

        bool IList.IsFixedSize => GetList().IsFixedSize;

        bool IList.IsReadOnly => GetList().IsReadOnly;

        int ICollection.Count => GetList().Count;

        bool ICollection.IsSynchronized => GetList().IsSynchronized;

        object ICollection.SyncRoot => GetList().SyncRoot;

        // Public properties --------------------------------------------------

        public event CollectionChangedDelegate CollectionChanged;
    }

    public class ManagedCollection<T> : ManagedCollection, IList<T>
    {
        // Private fields -----------------------------------------------------

        private readonly List<T> list = new();

        // Protected methods --------------------------------------------------

        protected override IList GetList() => list;

        // Public methods -----------------------------------------------------

        public void Add(T item)
        {
            list.Add(item);

            OnCollectionChanged(CollectionChange.ItemsAdded, new List<object> { item }, null);
        }

        public void Clear()
        {
            var items = new List<object>();
            foreach (var item in list)
                items.Add(item);

            ((ICollection<T>)list).Clear();

            OnCollectionChanged(CollectionChange.ItemsRemoved, null, items);
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);

            OnCollectionChanged(CollectionChange.ItemsAdded, new List<object> { item }, null);
        }

        public bool Remove(T item)
        {
            var removed = ((ICollection<T>)list).Remove(item);

            if (removed)
                OnCollectionChanged(CollectionChange.ItemsRemoved, null, new List<object> { item });

            return removed;
        }

        public void RemoveAt(int index)
        {
            var item = list[index];

            list.RemoveAt(index);

            OnCollectionChanged(CollectionChange.ItemsRemoved, null, new List<object> { item });
        }

        // Public properties --------------------------------------------------

        public T this[int index]
        {
            get => list[index];
            set
            {
                var item = list[index];
                list[index] = value;

                OnCollectionChanged(CollectionChange.ItemsReplaced, new List<object> { value }, new List<object> { item });
            }
        }

        public int Count => list.Count;

        public bool IsReadOnly => ((ICollection<T>)list).IsReadOnly;
    }
}
