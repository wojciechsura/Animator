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

        private void SetValue(ValueViewModel value)
        {
            // Unhook existing value change handlers

            if (Value is ReferenceValueViewModel currentReference)
            {
                currentReference.PropertyChanged -= HandleReferenceValueChanged;
            }

            // Hook value change handlers and set new value

            if (value is ReferenceValueViewModel)
            {
                value.PropertyChanged += HandleReferenceValueChanged;
                Set(ref this.value, value);
            }
            else
                throw new ArgumentException($"ManagedReferencePropertyViewModel does not support value of type {value}!");
        }

        private void HandleReferenceValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReferenceValueViewModel.Value))
                OnReferenceValueChanged();
        }

        protected void OnReferenceValueChanged() =>
            ReferenceValueChanged?.Invoke(this, EventArgs.Empty);

        public ReferencePropertyViewModel(WrapperContext context, string ns, string name)
            : base(context)
        {
            Namespace = ns;
            Name = name;
        }

        public ValueViewModel Value
        {
            get => value;
            set => SetValue(value);
        }

        public override string Name { get; }
        public override string Namespace { get; }

        public event EventHandler ReferenceValueChanged;
    }
}
