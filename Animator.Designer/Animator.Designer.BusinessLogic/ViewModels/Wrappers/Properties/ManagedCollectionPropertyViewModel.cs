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

        public ManagedCollectionPropertyViewModel(WrapperContext context, string defaultNamespace, ManagedCollectionProperty collectionProperty)
            : base(context, defaultNamespace, collectionProperty)
        {
            this.collectionProperty = collectionProperty;
            value = new CollectionValueViewModel();

            SetToStringCommand = new AppCommand(obj => SetToString());
            SetToCollectionCommand = new AppCommand(obj => SetToCollection());
        }

        public override ManagedProperty ManagedProperty => collectionProperty;
    }
}
