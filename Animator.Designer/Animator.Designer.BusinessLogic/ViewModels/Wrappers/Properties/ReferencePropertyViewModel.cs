using Animator.Designer.BusinessLogic.Helpers;
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

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ReferencePropertyViewModel : PropertyViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly Type baseType;
        private ValueViewModel value;

        // Private methods ----------------------------------------------------

        private void SetDefault()
        {
            Value = new DefaultValueViewModel(null, false);
        }

        private void SetValue(ValueViewModel value)
        {
            // Clear parent
            if (this.value != null)
                this.value.Parent = null;

            // Set new value
            if (value is ReferenceValueViewModel or DefaultValueViewModel)
            {
                Set(ref this.value, value, nameof(Value));
                value.Parent = this;
            }
            else
            {
                throw new ArgumentException($"ManagedReferencePropertyViewModel does not support value of type {value}!");
            }

            context.NotifyPropertyChanged();
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

        private void DoPaste()
        {
            (bool result, ManagedObjectViewModel pasted) = DeserializeObjectFromClipboard();
            if (!result)
            {
                return;
            }

            if (pasted.Type.IsAssignableTo(baseType))
                Value = new ReferenceValueViewModel(pasted);
        }

        // Public methods -----------------------------------------------------

        public ReferencePropertyViewModel(ObjectViewModel parent, 
            WrapperContext context, 
            string ns, 
            string name, 
            Type baseType)
            : base(parent, context)
        {
            Namespace = ns;
            Name = name;
            this.baseType = baseType;

            SetDefault();

            var valueIsDefaultCondition = Condition.Lambda(this, vm => vm.Value is DefaultValueViewModel, false);

            SetDefaultCommand = new AppCommand(obj => SetDefault(), !valueIsDefaultCondition);
            SetToInstanceCommand = new AppCommand(obj => SetToInstance((Type)obj));
            PasteCommand = new AppCommand(obj => DoPaste());
        }

        public override void NotifyAvailableTypesChanged()
        {
            base.NotifyAvailableTypesChanged();

            if (value is ReferenceValueViewModel refValue)
            {
                refValue.Value.NotifyAvailableTypesChanged();
            }
        }

        public override void RequestDelete(ObjectViewModel obj)
        {
            SetDefault();
        }

        public override void RequestSwitchToString()
        {
            throw new NotSupportedException();
        }

        // Public properties --------------------------------------------------

        public override string Name { get; }

        public override string Namespace { get; }

        public ValueViewModel Value
        {
            get => value;
            set => SetValue(value);
        }

        public override IEnumerable<TypeViewModel> AvailableTypes
        {
            get => context.Namespaces
                .OfType<AssemblyNamespaceViewModel>()
                .SelectMany(n => n.GetAvailableTypesFor(baseType))
                .OrderBy(e => e.Name)
                .Select(t => BuildTypeViewModel(t, SetToInstanceCommand));
        }
    }
}
