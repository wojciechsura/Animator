using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ManagedReferencePropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedReferenceProperty referenceProperty;

        private void HandleStringValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StringValueViewModel.Value))
                OnStringValueChanged();
        }

        private void HandleReferenceValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReferenceValueViewModel.Value))
                OnReferenceValueChanged();
        }

        private void SetDefault()
        {
            var value = new DefaultValueViewModel(null);
            Value = value;
        }

        private void SetToString()
        {
            Value = new StringValueViewModel(string.Empty);
        }

        protected override void OnSetValue(ValueViewModel value)
        {
            // Unhook existing value change handlers

            if (Value is StringValueViewModel currentString)
            {
                currentString.PropertyChanged -= HandleStringValueChanged;
            }
            else if (Value is ReferenceValueViewModel currentReference)
            {
                currentReference.PropertyChanged -= HandleReferenceValueChanged;
            }

            // Hook value change handlers and set new value

            if (value is StringValueViewModel)
            {
                value.PropertyChanged += HandleStringValueChanged;
                Set(ref this.value, value, nameof(Value));
            }
            else if (value is ReferenceValueViewModel)
            {
                value.PropertyChanged += HandleReferenceValueChanged;
                Set(ref this.value, value, nameof(Value));
            }
            else if (value is MarkupExtensionValueViewModel)
            {
                Set(ref this.value, value, nameof(Value));
            }
            else if (value is DefaultValueViewModel)
            {
                Set(ref this.value, value, nameof(Value));
            }
            else
                throw new ArgumentException($"ManagedReferencePropertyViewModel does not support value of type {value}!");
        }

        public ManagedReferencePropertyViewModel(WrapperContext context, string defaultNamespace, ManagedReferenceProperty referenceProperty)
            : base(context, defaultNamespace, referenceProperty)
        {
            this.referenceProperty = referenceProperty;
            value = new DefaultValueViewModel(null);

            SetDefaultCommand = new AppCommand(obj => SetDefault());
            SetToStringCommand = new AppCommand(obj => SetToString());
        }

        public override ManagedProperty ManagedProperty => referenceProperty;        
   }
}
