using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Editor.Configuration
{
    public abstract class BaseTypedItemCollection<T> : BaseItemCollection, IEnumerable<T>, IList where T : BaseCollectionItem
    {
        // Private fields -----------------------------------------------------

        private List<T> items;

        // Private methods ----------------------------------------------------

        private void Initialize()
        {
            items = new List<T>();
        }

        private void CheckCanAddItem(T newItem)
        {
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem));
            if (items.Contains(newItem))
                throw new InvalidOperationException("Already contains this item!");
            if (!IsChildNodeName(newItem.Name))
                throw new ArgumentException("Cannot add item, its XML node name does not match names registered in the collection", nameof(newItem));
        }

        private void CheckCanRemoveItem(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!items.Contains(item))
                throw new InvalidOperationException("Cannot remove item - is not on the list!");
        }

        private void AttachItem(T item)
        {
            item.AttachToCollection(this);
        }

        private void DetachItem(T item)
        {
            item.DetachFromCollection(this);
        }

        private int DoAddItem(T item)
        {
            CheckCanAddItem(item);

            items.Add(item);
            int pos = items.Count - 1;
            AttachItem(item);

            return pos;
        }

        private void DoClear()
        {
            foreach (var item in items)
                DetachItem(item);

            items.Clear();
        }

        private void DoInsertItem(T item, int index)
        {
            if (index < 0 || index > items.Count)
                throw new IndexOutOfRangeException(nameof(index));
            if (index == items.Count)
                DoAddItem(item);
            else
            {
                CheckCanAddItem(item);

                items.Insert(index, item);
                AttachItem(item);
            }
        }

        private void DoMoveItem(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= items.Count)
                throw new IndexOutOfRangeException(nameof(oldIndex));
            if (newIndex < 0 || newIndex >= items.Count)
                throw new IndexOutOfRangeException(nameof(newIndex));

            var item = items[oldIndex];
            items.RemoveAt(oldIndex);
            items.Insert(newIndex, item);
        }

        private void DoRemoveItem(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!items.Contains(item))
                throw new InvalidOperationException("Item is not on the list!");

            DetachItem(item);
            items.Remove(item);
        }

        private void DoRemoveItemAt(int index)
        {
            if (index < 0 || index >= items.Count)
                throw new IndexOutOfRangeException(nameof(index));

            var removedItem = items[index];
            DetachItem(removedItem);
            items.RemoveAt(index);
        }

        // IEnumerable<T> implementation --------------------------------------

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        // IList implementation -----------------------------------------------

        int IList.Add(object value)
        {
            if (value is T)
                return Add((T)value);
            else
                throw new ArgumentException(nameof(value));
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            if (value is T)
                return items.Contains((T)value);
            else
                throw new ArgumentException(nameof(value));
        }

        int IList.IndexOf(object value)
        {
            if (value is T)
                return items.IndexOf((T)value);
            else
                throw new ArgumentException(nameof(value));
        }

        void IList.Insert(int index, object value)
        {
            if (value is T)
                Insert((T)value, index);
            else
                throw new ArgumentException("value");
        }

        bool IList.IsFixedSize
        {
            get
            {
                return ((IList)items).IsFixedSize;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return ((IList)items).IsReadOnly;
            }
        }

        void IList.Remove(object value)
        {
            if (value is T)
                Remove((T)value);
            else
                throw new ArgumentException(nameof(value));
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return items[index];
            }
            set
            {
                if (value is T)
                {
                    RemoveAt(index);
                    Insert((T)value, index);
                }
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)items).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get
            {
                return items.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)items).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)items).SyncRoot;
            }
        }

        // Internal methods ---------------------------------------------------

        internal override void RegisterSubItem(ConfigItem subItem)
        {
            if (IsChildNodeName(subItem.Name))
                throw new InvalidOperationException("Cannot register subitem with the same node name as collection's child's node name!");

            base.RegisterSubItem(subItem);
        }

        internal override void RegisterValue(BaseValue value)
        {
            if (IsChildNodeName(value.Name))
                throw new InvalidOperationException("Cannot register value with the same node name as collection's child's node name!");

            base.RegisterValue(value);
        }

        // Protected types ----------------------------------------------------

        protected abstract class BaseChildInfo
        {
            internal BaseChildInfo(string name, Func<T> create, Type type)
            {
                Name = name;
                Create = create;
                Type = type;
            }

            public Func<T> Create { get; private set; }
            public string Name { get; private set; }
            public Type Type { get; private set; }
        }

        protected class ChildInfo<U> : BaseChildInfo where U : T
        {
            public ChildInfo(string name, Func<U> create)
                : base(name, create, typeof(U))
            {

            }
        }

        // Protected methods --------------------------------------------------

        protected override void InternalSave(XmlNode node)
        {
            base.InternalSave(node);

            for (int i = 0; i < items.Count; i++)
            {
                var childNodeName = items[i].Name;
                if (!IsChildNodeName(childNodeName))
                    throw new InvalidOperationException("Item stored in the collection doesn't have XmlName matching the names registered in the collection!");

                var childNode = node.OwnerDocument.CreateElement(childNodeName);
                items[i].Save(childNode);
                node.AppendChild(childNode);
            }
        }

        protected override void InternalLoad(XmlNode node)
        {
            base.InternalLoad(node);

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = node.ChildNodes[i];
                if (IsChildNodeName(childNode.Name))
                {
                    if (childNode is XmlElement element)
                    {
                        T item = InstantiateChildItem(childNode);
                        items.Add(item);
                        AttachItem(item);

                        item.Load(element);
                    }
                }
            }
        }

        protected T InstantiateChildItem(XmlNode xmlNode)
        {
            foreach (var item in ChildInfos)
                if (item.Name == xmlNode.Name)
                    return item.Create();

            throw new InvalidOperationException("Invalid child item!");
        }

        // Protected properties -----------------------------------------------

        protected abstract IEnumerable<BaseChildInfo> ChildInfos
        {
            get;
        }

        // Public methods -----------------------------------------------------

        public BaseTypedItemCollection(string xmlName, BaseItemContainer parent)
            : base(xmlName, parent)
        {
            Initialize();
        }

        public int Add(T item)
        {
            CheckCanAddItem(item);
            return DoAddItem(item);
        }

        public void Remove(T item)
        {
            CheckCanRemoveItem(item);
            DoRemoveItem(item);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= items.Count)
                throw new IndexOutOfRangeException(nameof(index));
            CheckCanRemoveItem(items[index]);
            DoRemoveItemAt(index);
        }

        public void Insert(T item, int index)
        {
            if (index < 0 || index >= items.Count)
                throw new IndexOutOfRangeException(nameof(index));
            CheckCanAddItem(item);
            DoInsertItem(item, index);
        }

        public bool IsChildNodeName(string name)
        {
            return ChildInfos.Any(i => i.Name == name);
        }

        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= items.Count)
                throw new IndexOutOfRangeException(nameof(oldIndex));
            if (newIndex < 0 || newIndex >= items.Count)
                throw new IndexOutOfRangeException(nameof(newIndex));

            DoMoveItem(oldIndex, newIndex);
        }

        public void Clear()
        {
            foreach (T item in items)
            {
                CheckCanRemoveItem(item);
            }

            DoClear();
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return items[index];
            }
        }
    }
}
