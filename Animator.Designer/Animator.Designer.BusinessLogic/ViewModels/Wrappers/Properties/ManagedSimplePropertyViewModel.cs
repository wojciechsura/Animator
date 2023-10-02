using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private void SetDefault()
        {
            var defaultValue = simpleProperty.Metadata.DefaultValue;
            Value = new DefaultValueViewModel(defaultValue);
        }

        private void SetToString()
        {
            Value = new StringValueViewModel(string.Empty);
        }

        protected override void OnSetValue(ValueViewModel value)
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
                Set(ref this.value, value, nameof(Value));
            }
            else if (value is MarkupExtensionValueViewModel)
                Set(ref this.value, value, nameof(Value));
            else if (value is DefaultValueViewModel)
                Set(ref this.value, value, nameof(Value));
            else
                throw new ArgumentException($"ManagedSimplePropertyViewModel does not support value of type {value}!");
        }

        public ManagedSimplePropertyViewModel(ObjectViewModel parent, WrapperContext context, string defaultNamespace, ManagedSimpleProperty property)
            : base(parent, context, defaultNamespace, property)
        {
            this.simpleProperty = property;
            SetDefault();

            var valueIsStringCondition = Condition.Lambda(this, vm => vm.Value is StringValueViewModel, false);
            var valueIsDefaultCondition = Condition.Lambda(this, vm => vm.Value is DefaultValueViewModel, false);

            SetDefaultCommand = new AppCommand(obj => SetDefault(), !valueIsDefaultCondition);
            SetToStringCommand = new AppCommand(obj => SetToString(), !valueIsStringCondition);
        }

        public override ManagedProperty ManagedProperty => simpleProperty;        
    }
}
