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
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ManagedCollectionPropertyViewModel : ManagedPropertyViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly ManagedCollectionProperty collectionProperty;

        // Private methods ----------------------------------------------------

        private void AddInstance(Type type)
        {
            if (!type.IsAssignableTo(collectionProperty.ItemType))
                throw new InvalidOperationException("Invalid object type!");
            if (value is not CollectionValueViewModel)
                throw new InvalidOperationException("Switch to collection mode first!");

            // Find namespace model
            var namespaceViewModel = context.Namespaces
                .OfType<AssemblyNamespaceViewModel>()
                .FirstOrDefault(cns => cns.Assembly == type.Assembly && cns.Namespace == type.Namespace);

            // Sanity check
            if (namespaceViewModel == null)
                throw new InvalidOperationException("Namespace for created object is missing in WrapperContext!");

            var ns = type.ToNamespaceDefinition().ToString();

            var obj = new ManagedObjectViewModel(context, ns, type.Name, type);
            (value as CollectionValueViewModel).Items.Add(obj);
        }

        private void InsertMacro()
        {
            if (value is not CollectionValueViewModel)
                throw new InvalidOperationException("Switch to collection mode first!");

            var obj = new MacroViewModel(context);
            (value as CollectionValueViewModel).Items.Add(obj);
        }

        private void InsertInclude()
        {
            if (value is not CollectionValueViewModel)
                throw new InvalidOperationException("Switch to collection mode first!");

            var obj = new IncludeViewModel(context);
            (value as CollectionValueViewModel).Items.Add(obj);
        }

        private void InsertGenerator()
        {
            if (value is not CollectionValueViewModel)
                throw new InvalidOperationException("Switch to collection mode first!");

            var obj = new GenerateViewModel(context);
            (value as CollectionValueViewModel).Items.Add(obj);
        }

        private void HandleCollectionChanged(object sender, EventArgs e)
        {
            OnCollectionChanged();
            context.NotifyPropertyChanged();
        }

        private void HandleStringValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StringValueViewModel.Value))
            {
                OnStringValueChanged();
                context.NotifyPropertyChanged();
            }
        }

        private void SetToCollection()
        {
            Value = new CollectionValueViewModel();
        }

        private void SetToString()
        {
            Value = new StringValueViewModel(string.Empty);
        }

        // Protected methods --------------------------------------------------

        protected override void OnSetValue(ValueViewModel value)
        {
            // Unhook existing event handlers

            if (this.value is StringValueViewModel currentString)
            {
                currentString.PropertyChanged -= HandleStringValueChanged;
            }
            if (this.value is CollectionValueViewModel currentCollection)
            {
                currentCollection.CollectionChanged -= HandleCollectionChanged;
            }

            // Hook new event handlers and set value

            if (value is StringValueViewModel strProperty)
            {
                strProperty.PropertyChanged += HandleStringValueChanged;
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

            context.NotifyPropertyChanged();
        }

        // Public methods -----------------------------------------------------

        public ManagedCollectionPropertyViewModel(ObjectViewModel parent, WrapperContext context, ManagedCollectionProperty collectionProperty)
            : base(parent, context, collectionProperty)
        {
            this.collectionProperty = collectionProperty;
            SetToCollection();

            var valueIsStringCondition = Condition.Lambda(this, vm => vm.Value is StringValueViewModel, false);
            var valueIsCollectionCondition = Condition.Lambda(this, vm => vm.Value is CollectionValueViewModel, false);

            SetToStringCommand = new AppCommand(obj => SetToString(), !valueIsStringCondition);
            SetToCollectionCommand = new AppCommand(obj => SetToCollection(), !valueIsCollectionCondition);
            AddInstanceCommand = new AppCommand(obj => AddInstance((Type)obj), valueIsCollectionCondition);
            InsertMacroCommand = new AppCommand(obj => InsertMacro(), valueIsCollectionCondition);
            InsertIncludeCommand = new AppCommand(obj => InsertInclude(), valueIsCollectionCondition);
            InsertGeneratorCommand = new AppCommand(obj => InsertGenerator(), valueIsCollectionCondition);            
        }

        public override void NotifyAvailableTypesChanged()
        {
            base.NotifyAvailableTypesChanged();

            if (value is CollectionValueViewModel collectionValue)
            {
                foreach (var item in collectionValue.Items)
                    item.NotifyAvailableTypesChanged();
            }
        }

        public override void RequestDelete(ObjectViewModel obj)
        {
            if (value is CollectionValueViewModel collection)
            {
                if (collection.Items.Contains(obj))
                {
                    collection.Items.Remove(obj);
                }
                else
                    throw new InvalidOperationException("Cannot delete: collection doesn't contain this object!");
            }
            else
                throw new InvalidOperationException("Cannot delete: value of this property is not a collection!");
        }

        public override void RequestSwitchToString()
        {
            throw new NotSupportedException();
        }

        // Public properties --------------------------------------------------

        public override IEnumerable<TypeViewModel> AvailableTypes
        {
            get
            {
                var collectionType = collectionProperty.ItemType;

                return context.Namespaces
                    .OfType<AssemblyNamespaceViewModel>()
                    .SelectMany(ns => ns.GetAvailableTypesFor(collectionType))
                    .OrderBy(tvm => tvm.Name)
                    .Select(t => new TypeViewModel(t, AddInstanceCommand));
            }
        }

        public override ManagedProperty ManagedProperty => collectionProperty;
    }
}
