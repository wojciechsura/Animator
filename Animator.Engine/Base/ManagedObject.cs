﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class PropertyValueChangedEventArgs : EventArgs
    {
        public PropertyValueChangedEventArgs(ManagedProperty property, object oldValue, object newValue)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ManagedProperty Property { get; }
        public object OldValue { get; }
        public object NewValue { get; }
    }

    public delegate void PropertyValueChangedDelegate(object sender, PropertyValueChangedEventArgs args);

    public class PropertyBaseValueInvalidatedEventArgs
    {
        public PropertyBaseValueInvalidatedEventArgs(ManagedProperty property)
        {
            Property = property;
        }

        public ManagedProperty Property { get; }
    }

    public delegate void PropertyBaseValueInvalidatedDelegate(object sender, PropertyBaseValueInvalidatedEventArgs args);

    public abstract class ManagedObject
    {
        // Private fields -----------------------------------------------------

        private readonly Dictionary<int, PropertyValue> propertyValues = new();
        private readonly Dictionary<int, object> collections = new();

        // Private methods ----------------------------------------------------

        private void ValidateValue(ManagedSimpleProperty property, object value)
        {
            if (property.Type.IsValueType && value == null)
                throw new ArgumentException($"{property.OwnerClassType.Name}.{property.Name} property type is value-type ({property.Type.Name}), but provided value is null.");

            if (!value.GetType().IsAssignableTo(property.Type))
                throw new ArgumentException($"Value of type {value.GetType().Name} cannot be assigned to property {property.OwnerClassType.Name}.{property.Name} of type {property.Type.Name}.");
        }

        private (object, bool) InternalCoerceValue(ManagedSimpleProperty property)
        {
            if (property.Metadata.CoerceValueHandler != null)
            {
                PropertyValue propertyValue = EnsurePropertyValue(property);

                var oldValue = propertyValue.EffectiveValue;

                var newValue = property.Metadata.CoerceValueHandler(this, propertyValue.FinalBaseValue);

                if (newValue != oldValue)
                    return (newValue, true);
            }

            return (null, false);
        }

        private PropertyValue EnsurePropertyValue(ManagedSimpleProperty property)
        {
            if (!propertyValues.TryGetValue(property.GlobalIndex, out PropertyValue propertyValue))
            {
                propertyValue = new PropertyValue(property.GlobalIndex, property.Metadata.DefaultValue);
                propertyValues[property.GlobalIndex] = propertyValue;
            }

            return propertyValue;
        }

        private object EnsureCollection(ManagedCollectionProperty property)
        {
            if (!collections.TryGetValue(property.GlobalIndex, out object collection))
            {
                collection = property.Metadata.CollectionInitializer();
                collections[property.GlobalIndex] = collection;
            }

            return collection;
        }

        private void CoerceAfterFinalBaseValueChanged(ManagedSimpleProperty property, PropertyValue propertyValue, object oldEffectiveValue)
        {
            (object coercedValue, bool coerced) = InternalCoerceValue(property);

            if (coerced && coercedValue != propertyValue.CoercedValue)
                propertyValue.CoercedValue = coercedValue;
            else
                propertyValue.ClearCoercedValue();

            if (oldEffectiveValue != propertyValue.EffectiveValue)
                OnPropertyValueChanged(property, oldEffectiveValue, propertyValue.EffectiveValue);
        }

        // Protected methods --------------------------------------------------

        protected virtual void OnPropertyValueChanged(ManagedProperty property, object oldValue, object newValue)
        {
            PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(property, oldValue, newValue));
        }

        protected virtual void OnPropertyBaseValueInvalidated(ManagedProperty property)
        {
            PropertyBaseValueInvalidated?.Invoke(this, new PropertyBaseValueInvalidatedEventArgs(property));
        }

        // Internal methods ---------------------------------------------------

        internal void SetAnimatedValue(ManagedProperty property, object value)
        {
            if (property is not ManagedSimpleProperty simpleProperty)
                throw new InvalidOperationException("Cannot ensure property value for non-simple property!");

            var propertyValue = EnsurePropertyValue(simpleProperty);

            if (!propertyValue.IsAnimated || propertyValue.AnimatedValue != value)
            {
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.AnimatedValue = value;
                
                CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);
            }
        }

        internal void ResetAnimatedValue(ManagedProperty property)
        {
            if (property is not ManagedSimpleProperty simpleProperty)
                throw new InvalidOperationException("Cannot ensure property value for non-simple property!");

            if (propertyValues.TryGetValue(property.GlobalIndex, out PropertyValue propertyValue) && propertyValue.IsAnimated)
            {
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.ClearAnimatedValue();

                CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);
            }

            // If there is no entry in PropertyValues for this value,
            // there can be no animated value too, so there's nothing to do
        }

        // Public methods -----------------------------------------------------

        public ManagedObject()
        {

        }

        public void CoerceValue(ManagedProperty property)
        {
            if (property is not ManagedSimpleProperty simpleProperty)
                throw new InvalidOperationException("Cannot ensure property value for non-simple property!");

            var propertyValue = EnsurePropertyValue(simpleProperty);

            var oldEffectiveValue = propertyValue.EffectiveValue;

            CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);
        }

        public ManagedProperty GetProperty(string propertyName)
        {
            return ManagedProperty.ByTypeAndName(GetType(), propertyName);
        }

        public object GetValue(ManagedProperty property)
        {
            if (property is ManagedSimpleProperty simpleProperty)
            {
                if (propertyValues.TryGetValue(property.GlobalIndex, out PropertyValue propertyValue))
                    return propertyValue.EffectiveValue;
                else
                    return simpleProperty.Metadata.DefaultValue;
            }
            else if (property is ManagedCollectionProperty collectionProperty)
            {
                var collection = EnsureCollection(collectionProperty);
                return collection;
            }
            else
            {
                throw new InvalidOperationException("Unsupported managed property type!");
            }
        }

        public object GetFinalBaseValue(ManagedProperty property)
        {
            if (property is ManagedSimpleProperty simpleProperty)
            {
                if (propertyValues.TryGetValue(property.GlobalIndex, out PropertyValue propertyValue))
                    return propertyValue.FinalBaseValue;
                else
                    return simpleProperty.Metadata.DefaultValue;
            }
            else
                throw new ArgumentException("Final base value is available only for simple properties!");
        }

        public void SetValue(ManagedProperty property, object value)
        {
            if (property is not ManagedSimpleProperty simpleProperty)
                throw new ArgumentException("Setting values is available only for simple properties!");

            ValidateValue(simpleProperty, value);

            var propertyValue = EnsurePropertyValue(simpleProperty);

            if (propertyValue.BaseValue != value)
            {
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.BaseValue = value;
                propertyValue.ResetModifiers();

                // Coertion
                CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);

                OnPropertyBaseValueInvalidated(property);
            }
        }

        // Public properties --------------------------------------------------

        public event PropertyValueChangedDelegate PropertyValueChanged;

        public event PropertyBaseValueInvalidatedDelegate PropertyBaseValueInvalidated;
    }
}
