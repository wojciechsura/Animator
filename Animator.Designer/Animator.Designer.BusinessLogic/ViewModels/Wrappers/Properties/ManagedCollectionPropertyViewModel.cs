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
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ManagedCollectionPropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedCollectionProperty collectionProperty;

        private void HandleCollectionChanged(object sender, EventArgs e)
        {
            OnCollectionChanged();
        }

        private void HandleStringValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StringValueViewModel.Value))
                OnStringValueChanged();
        }

        private void SetToString()
        {
            Value = new StringValueViewModel(string.Empty);
        }

        private void SetToCollection()
        {
            Value = new CollectionValueViewModel();
        }

        private void AddInstance(Type type)
        {
            throw new NotImplementedException();
        }

        protected override void OnSetValue(ValueViewModel value)
        {
            // Unhook existing event handlers

            if (Value is StringValueViewModel currentString)
            {
                currentString.PropertyChanged -= HandleStringValueChanged;
            }
            if (Value is CollectionValueViewModel currentCollection)
            {
                currentCollection.CollectionChanged -= HandleCollectionChanged;
            }

            // Hook new event handlers and set value

            if (value is StringValueViewModel)
            {
                value.PropertyChanged += HandleStringValueChanged;
                Set(ref this.value, value, nameof(Value));
            }
            else if (value is CollectionValueViewModel newCollection)
            {
                newCollection.CollectionChanged += HandleCollectionChanged;
                Set(ref this.value, value, nameof(Value));
            }
            else if (value is MarkupExtensionValueViewModel)
            {
                Set(ref this.value, value, nameof(Value));
            }
            else
                throw new ArgumentException($"ManagedCollectionPropertyViewModel does not support value of type {value}!");
        }

        public ManagedCollectionPropertyViewModel(ObjectViewModel parent, WrapperContext context, string defaultNamespace, ManagedCollectionProperty collectionProperty)
            : base(parent, context, defaultNamespace, collectionProperty)
        {
            this.collectionProperty = collectionProperty;
            value = new CollectionValueViewModel();

            var valueIsStringCondition = Condition.Lambda(this, vm => vm.Value is StringValueViewModel, false);
            var valueIsCollectionCondition = Condition.Lambda(this, vm => vm.Value is CollectionValueViewModel, false);

            SetToStringCommand = new AppCommand(obj => SetToString(), !valueIsStringCondition);
            SetToCollectionCommand = new AppCommand(obj => SetToCollection(), !valueIsCollectionCondition);
            AddInstanceCommand = new AppCommand(obj => AddInstance((Type)obj));
        }

        public override ManagedProperty ManagedProperty => collectionProperty;

        public override IEnumerable<TypeViewModel> AvailableTypes
        {
            get
            {
                var collectionType = collectionProperty.ItemType;

                return context.Namespaces
                    .SelectMany(ns => ns.GetAvailableTypesFor(collectionType))
                    .OrderBy(tvm => tvm.Name)
                    .Select(t => new TypeViewModel(t, AddInstanceCommand));
            }
        }
    }
}
