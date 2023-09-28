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
    public class ManagedReferencePropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedReferenceProperty referenceProperty;
        private ValueViewModel value;

        private void SetValue(ValueViewModel value)
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
                Set(ref this.value, value);
            }
            else if (value is ReferenceValueViewModel)
            {
                value.PropertyChanged += HandleReferenceValueChanged;
                Set(ref this.value, value);
            }
            else if (value is MarkupExtensionViewModel)
            {
                Set(ref this.value, value);
            }
            else
                throw new ArgumentException($"ManagedReferencePropertyViewModel does not support value of type {value}!");
        }

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

        public ManagedReferencePropertyViewModel(string defaultNamespace, ManagedReferenceProperty referenceProperty)
            : base(defaultNamespace, referenceProperty)
        {
            this.referenceProperty = referenceProperty;
        }

        public override ManagedProperty ManagedProperty => referenceProperty;

        public ValueViewModel Value
        {
            get => value;
            set => SetValue(value);            
        }
   }
}
