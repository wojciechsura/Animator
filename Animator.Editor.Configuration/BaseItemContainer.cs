using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Editor.Configuration
{
    public abstract class BaseItemContainer : BaseObject
    {
        // Private fields -----------------------------------------------------

        private List<ConfigItem> subItems;

        private List<BaseValue> values;

        // Internal methods ---------------------------------------------------

        internal virtual void RegisterSubItem(ConfigItem subItem)
        {
            if (subItem == null)
                throw new ArgumentNullException(nameof(subItem));
            if (subItems.Any(s => s.Name == subItem.Name))
                throw new InvalidOperationException("There is already a subitem with the same name!");
            if (values.Any(v => v.Name == subItem.Name))
                throw new InvalidOperationException("There is already a value with the same name!");

            subItems.Add(subItem);
        }

        internal virtual void RegisterValue(BaseValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (subItems.Any(s => s.Name == value.Name))
                throw new InvalidOperationException("There is already a subitem with the same name!");
            if (values.Any(v => v.Name == value.Name))
                throw new InvalidOperationException("There is already a value with the same name!");

            values.Add(value);
        }

        internal void Load(XmlNode node)
        {
            InternalLoad(node);
        }

        internal void Save(XmlNode node)
        {
            InternalSave(node);
        }

        // Protected methods --------------------------------------------------

        protected virtual void InternalLoad(XmlNode node)
        {
            foreach (BaseItemContainer item in subItems)
            {
                string tagName = item.Name;
                XmlNode itemNode = node[tagName];
                if (itemNode != null)
                    item.Load(itemNode);
                else
                    item.SetDefaults();
            }

            foreach (BaseValue value in values)
            {
                value.Load(node);
            }
        }

        protected virtual void InternalSave(XmlNode node)
        {
            foreach (BaseItemContainer item in subItems)
            {
                string tagName = item.Name;
                XmlElement element = node.OwnerDocument.CreateElement(tagName);
                item.Save(element);
                node.AppendChild(element);
            }

            foreach (var value in values)
            {
                value.Save(node);
            }
        }

        // Public methods -----------------------------------------------------

        public BaseItemContainer(string name)
            : base(name)
        {
            subItems = new List<ConfigItem>();
            values = new List<BaseValue>();
        }

        public void SetDefaults()
        {
            foreach (var subItem in subItems)
                subItem.SetDefaults();
            foreach (var value in values)
                value.SetDefaults();
        }


        public IEnumerable<BaseItemContainer> SubItems
        {
            get
            {
                return subItems;
            }
        }

        public IEnumerable<BaseValue> Values
        {
            get
            {
                return values;
            }
        }
    }
}
