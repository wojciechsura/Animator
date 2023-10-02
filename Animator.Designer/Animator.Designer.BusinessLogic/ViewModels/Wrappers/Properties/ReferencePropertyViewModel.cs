using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ReferencePropertyViewModel : PropertyViewModel
    {
        private ValueViewModel value;

        private void HandleReferenceValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReferenceValueViewModel.Value))
                OnReferenceValueChanged();
        }

        private void SetDefault()
        {
            Value = new DefaultValueViewModel(null, false);
        }

        private void SetValue(ValueViewModel value)
        {
            // Clear parent
            if (this.value != null)
                this.value.Parent = null;

            // Unhook existing value change handlers
            if (Value is ReferenceValueViewModel currentReference)
            {
                currentReference.PropertyChanged -= HandleReferenceValueChanged;
            }

            // Set new value
            if (value is ReferenceValueViewModel)
            {
                Set(ref this.value, value);
                value.PropertyChanged += HandleReferenceValueChanged;
                value.Parent = this;
            }
            else if (value is DefaultValueViewModel)
            {
                Set(ref this.value, value);
                value.Parent = this;
            }
            else
            {
                throw new ArgumentException($"ManagedReferencePropertyViewModel does not support value of type {value}!");
            }
        }

        protected void OnReferenceValueChanged() =>
            ReferenceValueChanged?.Invoke(this, EventArgs.Empty);

        public override void RequestDelete(BaseObjectViewModel obj)
        {
            SetDefault();
        }

        public ReferencePropertyViewModel(ObjectViewModel parent, WrapperContext context, string ns, string name)
            : base(parent, context)
        {
            Namespace = ns;
            Name = name;
        }

        public override void RequestSwitchToString()
        {
            throw new NotSupportedException();
        }
        public override string Name { get; }

        public override string Namespace { get; }

        public event EventHandler ReferenceValueChanged;

        public ValueViewModel Value
        {
            get => value;
            set => SetValue(value);
        }
    }
}
