using System;
using System.Xml;

namespace Animator.Editor.Configuration
{
    public abstract class BaseValue : BaseObject
    {
        // Protected fields ---------------------------------------------------

        protected readonly BaseItemContainer owner;

        // Internal methods ---------------------------------------------------

        internal void Save(XmlNode rootNode)
        {
            InternalSave(rootNode);
        }

        internal void Load(XmlNode rootNode)
        {
            InternalLoad(rootNode);
        }

        // Protected methods --------------------------------------------------

        protected abstract void InternalSave(XmlNode rootNode);

        protected abstract void InternalLoad(XmlNode rootNode);

        // Public methods -----------------------------------------------------

        public BaseValue(string name, BaseItemContainer owner)
            : base(name)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            owner.RegisterValue(this);
        }

        public abstract void SetDefaults();

        public BaseItemContainer Owner
        {
            get
            {
                return owner;
            }
        }
    }
}