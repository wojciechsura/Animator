using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class ManagedPropertyViewModel : PropertyViewModel
    {
        protected void OnStringValueChanged() => StringValueChanged?.Invoke(this, EventArgs.Empty);
        protected void OnReferenceValueChanged() => ReferenceValueChanged?.Invoke(this, EventArgs.Empty);
        protected void OnCollectionChanged() => CollectionChanged?.Invoke(this, EventArgs.Empty);

        public ManagedPropertyViewModel(string defaultNamespace, ManagedProperty property)
        {
            Name = property.Name;
            Namespace = defaultNamespace;
        }

        public abstract ManagedProperty ManagedProperty { get; }

        public override string Name { get; }

        public override string Namespace { get; }

        public event EventHandler StringValueChanged;
        public event EventHandler ReferenceValueChanged;
        public event EventHandler CollectionChanged;
    }
}
