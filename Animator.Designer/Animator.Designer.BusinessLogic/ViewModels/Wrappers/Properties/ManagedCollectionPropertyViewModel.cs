﻿using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
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
    public class ManagedCollectionPropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedCollectionProperty collectionProperty;
        private ValueViewModel value;

        private IEnumerable<BaseObjectViewModel> GetDisplayChildren()
        {
            if (value is CollectionValueViewModel collection)
                foreach (var item in collection.Items)
                    yield return item;
        }

        private void HandleCollectionChanged(object sender, EventArgs e)
        {
            OnCollectionChanged();

            OnPropertyChanged(nameof(DisplayChildren));
        }

        private void HandleStringValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StringValueViewModel.Value))
                OnStringValueChanged();
        }

        public ManagedCollectionPropertyViewModel(string defaultNamespace, ManagedCollectionProperty collectionProperty)
            : base(defaultNamespace, collectionProperty)
        {
            this.collectionProperty = collectionProperty;
        }

        public ValueViewModel Value
        {
            get => value;
            set => SetValue(value);            
        }

        private void SetValue(ValueViewModel value)
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
                Set(ref this.value, value);
            }
            else if (value is CollectionValueViewModel newCollection)
            {
                newCollection.CollectionChanged += HandleCollectionChanged;
                Set(ref this.value, value);
            }
            else if (value is MarkupExtensionViewModel)
                Set(ref this.value, value);
            else
                throw new ArgumentException($"ManagedCollectionPropertyViewModel does not support value of type {value}!");

            OnPropertyChanged(nameof(DisplayChildren));
        }

        public override ManagedProperty ManagedProperty => collectionProperty;

        public IEnumerable<BaseObjectViewModel> DisplayChildren => GetDisplayChildren();
    }
}