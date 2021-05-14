using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Editor.Configuration
{
    public abstract class BaseTypedValue<T> : BaseValue, INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private T value;
        private readonly T defaultValue;

        // Private methods ----------------------------------------------------

        private void SaveToSubnode(XmlNode rootNode)
        {
            if (Value != null)
            {
                XmlElement element = rootNode.OwnerDocument.CreateElement(Name);
                element.InnerText = Value.ToString();
                rootNode.AppendChild(element);
            }
        }

        private void LoadFromSubnode(XmlNode rootNode)
        {
            var element = rootNode[Name];
            InternalLoadFromNode(element);
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected abstract string SerializeValue(T value);

        protected abstract T DeserializeValue(string text);

        protected void InternalLoadFromNode(XmlNode node)
        {
            if (node != null)
            {
                try {
                    Value = DeserializeValue(node.InnerText);
                }
                catch
                {
                    Value = defaultValue;
                }
            }
            else
            {
                Value = defaultValue;
            }
        }

        protected override void InternalLoad(XmlNode rootNode)
        {
            LoadFromSubnode(rootNode);
        }

        protected override void InternalSave(XmlNode rootNode)
        {
            SaveToSubnode(rootNode);
        }

        // Public methods -----------------------------------------------------

        public BaseTypedValue(string xmlName, BaseItemContainer owner, T defaultValue = default)
            : base(xmlName, owner)
        {
            this.defaultValue = defaultValue;
            this.value = defaultValue;
        }

        public override sealed void SetDefaults()
        {
            Value = defaultValue;
        }

        // Public properties --------------------------------------------------

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!object.Equals(this.value, value))
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
