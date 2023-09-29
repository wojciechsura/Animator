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
    public class ManagedSimplePropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedSimpleProperty simpleProperty;

        private void HandleStringValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StringValueViewModel.Value))
                OnStringValueChanged();
        }

        protected override void SetValue(ValueViewModel value)
        {
            // Unhook existing event handlers

            if (Value is StringValueViewModel currentStringValue)
            {
                currentStringValue.PropertyChanged -= HandleStringValueChanged;
            }

            // Hook new event handlers and set value

            if (value is StringValueViewModel)
            {
                value.PropertyChanged += HandleStringValueChanged;
                Set(ref this.value, value);
            }
            else if (value is MarkupExtensionValueViewModel)
                Set(ref this.value, value);
            else if (value is DefaultValueViewModel)
                throw new ArgumentException("Use SetDefault method instead of setting DefaultValueViewModel!");
            else
                throw new ArgumentException($"ManagedSimplePropertyViewModel does not support value of type {value}!");
        }

        public ManagedSimplePropertyViewModel(string defaultNamespace, ManagedSimpleProperty property)
            : base(defaultNamespace, property)
        {
            this.simpleProperty = property;
            SetDefault(property);
        }

        public void SetDefault(ManagedSimpleProperty property)
        {
            var defaultValue = property.Metadata.DefaultValue;
            value = new DefaultValueViewModel(defaultValue);
        }

        public override ManagedProperty ManagedProperty => simpleProperty;
    }
}
