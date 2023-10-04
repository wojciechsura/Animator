using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Animator.Engine.Base.Extensions;
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

        private void SetDefault()
        {
            var value = new DefaultValueViewModel(null, false);
            Value = value;
        }

        private void SetToInstance(Type type)
        {
            // Find namespace model
            var namespaceViewModel = context.Namespaces
                .OfType<AssemblyNamespaceViewModel>()
                .FirstOrDefault(cns => cns.Assembly == type.Assembly && cns.Namespace == type.Namespace);

            // Sanity check
            if (namespaceViewModel == null)
                throw new InvalidOperationException("Namespace for created object is missing in WrapperContext!");

            var ns = type.ToNamespaceDefinition().ToString();

            var obj = new ManagedObjectViewModel(context, ns, type.Name, type);
            Value = new ReferenceValueViewModel(obj);
        }

        private void InsertMacro()
        {
            var obj = new MacroViewModel(context);
            Value = new ReferenceValueViewModel(obj);
        }

        private void InsertInclude()
        {
            var obj = new IncludeViewModel(context);
            Value = new ReferenceValueViewModel(obj);
        }

        private void InsertGenerator()
        {
            var obj = new GenerateViewModel(context);
            Value = new ReferenceValueViewModel(obj);
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

            // Hook value change handlers and set new value

            if (value is StringValueViewModel)
            {
                value.PropertyChanged += HandleStringValueChanged;
                Set(ref this.value, value, nameof(Value));
            }
            else if (value is ReferenceValueViewModel or MarkupExtensionValueViewModel or DefaultValueViewModel)
            {
                Set(ref this.value, value, nameof(Value));
            }
            else
                throw new ArgumentException($"ManagedReferencePropertyViewModel does not support value of type {value}!");
        }

        public ManagedReferencePropertyViewModel(ObjectViewModel parent, WrapperContext context, ManagedReferenceProperty referenceProperty)
            : base(parent, context, referenceProperty)
        {
            this.referenceProperty = referenceProperty;
            SetDefault();

            var valueIsStringCondition = Condition.Lambda(this, vm => vm.Value is StringValueViewModel, false);
            var valueIsDefaultCondition = Condition.Lambda(this, vm => vm.Value is DefaultValueViewModel, false);

            SetDefaultCommand = new AppCommand(obj => SetDefault(), !valueIsDefaultCondition);
            SetToStringCommand = new AppCommand(obj => SetToString(), !valueIsStringCondition);
            SetToInstanceCommand = new AppCommand(obj => SetToInstance((Type)obj));
            InsertMacroCommand = new AppCommand(obj => InsertMacro());
            InsertIncludeCommand = new AppCommand(obj => InsertInclude());
            InsertGeneratorCommand = new AppCommand(obj => InsertGenerator());
        }

        public override void RequestDelete(ObjectViewModel obj)
        {
            SetDefault();
        }

        public override void RequestSwitchToString()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<TypeViewModel> AvailableTypes
        {
            get
            {
                var refPropertyType = referenceProperty.Type;
                return context.Namespaces
                    .OfType<AssemblyNamespaceViewModel>()
                    .SelectMany(ns => ns.GetAvailableTypesFor(refPropertyType))
                    .OrderBy(tvm => tvm.Name)
                    .Select(t => new TypeViewModel(t, SetToInstanceCommand));
            }
        }

        public override ManagedProperty ManagedProperty => referenceProperty;
    }
}
