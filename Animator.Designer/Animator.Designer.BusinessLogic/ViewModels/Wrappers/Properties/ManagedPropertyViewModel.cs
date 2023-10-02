using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Newtonsoft.Json.Linq;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class ManagedPropertyViewModel : PropertyViewModel
    {
        private void SetValue(ValueViewModel value)
        {
            if (this.value != null) 
            {                
                value.Handler = null;
                value.Parent = null;
            }

            OnSetValue(value);

            if (this.value != null)
            {
                value.Parent = this;
                value.Handler = this;
            }
        }

        protected ValueViewModel value;

        protected void OnStringValueChanged() => StringValueChanged?.Invoke(this, EventArgs.Empty);
        protected void OnReferenceValueChanged() => ReferenceValueChanged?.Invoke(this, EventArgs.Empty);
        protected void OnCollectionChanged() => CollectionChanged?.Invoke(this, EventArgs.Empty);
        protected abstract void OnSetValue(ValueViewModel value);

        public ManagedPropertyViewModel(ObjectViewModel parent, WrapperContext context, string defaultNamespace, ManagedProperty property)
            : base(parent, context)
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
            set => SetValue(value);
        }

        public event EventHandler StringValueChanged;
        public event EventHandler ReferenceValueChanged;
        public event EventHandler CollectionChanged;
    }
}
