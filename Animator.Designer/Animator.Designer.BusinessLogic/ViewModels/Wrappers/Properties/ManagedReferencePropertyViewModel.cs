using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
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

        private void SetToInstance(Type obj)
        {
            throw new NotImplementedException();
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

            var valueIsStringCondition = Condition.Lambda(this, vm => vm.Value is StringValueViewModel, false);
            var valueIsDefaultCondition = Condition.Lambda(this, vm => vm.Value is DefaultValueViewModel, false);

            SetDefaultCommand = new AppCommand(obj => SetDefault(), !valueIsStringCondition);
            SetToStringCommand = new AppCommand(obj => SetToString(), !valueIsDefaultCondition);
            SetToInstanceCommand = new AppCommand(obj => SetToInstance((Type)obj));
        }

        public override ManagedProperty ManagedProperty => referenceProperty;

        public override IEnumerable<TypeViewModel> AvailableTypes
        {
            get
            {
                var refPropertyType = referenceProperty.Type;
                return context.Namespaces
                    .SelectMany(ns => ns.GetAvailableTypesFor(refPropertyType))
                    .OrderBy(tvm => tvm.Name)
                    .Select(t => new TypeViewModel(t, SetToInstanceCommand));
            }
        }
    }
}
