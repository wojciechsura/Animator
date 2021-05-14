using System;

namespace Animator.Editor.Configuration
{
    public class ConfigItem : BaseItemContainer
    {
        // Private fields -----------------------------------------------------

        private readonly BaseItemContainer parent;

        // Public methods -----------------------------------------------------

        public ConfigItem(string name, BaseItemContainer parent) 
            : base(name)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            parent.RegisterSubItem(this);
        }

        // Public properties --------------------------------------------------

        public BaseItemContainer Parent
        {
            get
            {
                return parent;
            }
        }
    }
}