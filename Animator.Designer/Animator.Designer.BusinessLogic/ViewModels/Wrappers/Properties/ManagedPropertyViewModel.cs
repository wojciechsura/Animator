using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class ManagedPropertyViewModel : PropertyViewModel
    {
        private void SetValue(ValueViewModel value)
        {
            if (this.value != null)
                value.Handler = null;

            OnSetValue(value);

            if (this.value != null)
                value.Handler = this;
        }
        
        protected ValueViewModel value;

        protected void OnStringValueChanged() => StringValueChanged?.Invoke(this, EventArgs.Empty);
        protected void OnReferenceValueChanged() => ReferenceValueChanged?.Invoke(this, EventArgs.Empty);
        protected void OnCollectionChanged() => CollectionChanged?.Invoke(this, EventArgs.Empty);
        protected abstract void OnSetValue(ValueViewModel value);

        public ManagedPropertyViewModel(WrapperContext context, string defaultNamespace, ManagedProperty property)
            : base(context)
        {
            Name = property.Name;
            Namespace = defaultNamespace;
        }

        public abstract ManagedProperty ManagedProperty { get; }

        public override string Name { get; }

        public override string Namespace { get; }

        public ValueViewModel Value
        {
            get => value;
            set => OnSetValue(value);
        }

        public event EventHandler StringValueChanged;
        public event EventHandler ReferenceValueChanged;
        public event EventHandler CollectionChanged;
    }
}
