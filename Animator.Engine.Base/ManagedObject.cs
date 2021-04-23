using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Animator.Engine.Base.BasePropertyMetadata;

namespace Animator.Engine.Base
{
    public abstract class ManagedObject
    {
        // Private fields -----------------------------------------------------

        private readonly Dictionary<int, PropertyValue> propertyValues = new();
        private readonly Dictionary<int, ManagedCollection> collections = new();
        private readonly Dictionary<int, object> references = new();

        // Private methods ----------------------------------------------------

        private void ValidateValue(ManagedSimpleProperty property, object value)
        {
            if (property.Type.IsValueType && value == null)
                throw new ArgumentException($"{property.OwnerClassType.Name}.{property.Name} property type is value-type ({property.Type.Name}), but provided value is null.");

            if (!value.GetType().IsAssignableTo(property.Type))
                throw new ArgumentException($"Value of type {value.GetType().Name} cannot be assigned to property {property.OwnerClassType.Name}.{property.Name} of type {property.Type.Name}.");
        }

        private void ValidateValue(ManagedReferenceProperty property, object value)
        {
            if (value != null && !value.GetType().IsAssignableTo(property.Type))
                throw new ArgumentException($"Value of type {value.GetType().Name} cannot be assigned to property {property.OwnerClassType.Name}.{property.Name} of type {property.Type.Name}.");

            if (property.Metadata.ValueValidationHandler != null && !property.Metadata.ValueValidationHandler.Invoke(this, new ValueValidationEventArgs(value)))
                throw new ArgumentException($"Value {value} failed validation for property {property.OwnerClassType.Name}{property.Name}");
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

        private ManagedCollection EnsureCollection(ManagedCollectionProperty property)
        {
            if (!collections.TryGetValue(property.GlobalIndex, out ManagedCollection collection))
            {
                if (property.Metadata.CollectionInitializer != null)
                    collection = property.Metadata.CollectionInitializer();
                else
                    collection = (ManagedCollection)Activator.CreateInstance(property.Type);

                if (!collection.GetType().IsAssignableTo(property.Type))
                    throw new InvalidOperationException($"Instantiated collection ({collection.GetType().Name} doesn't match property type ({property.Type.Name})!");

                collections[property.GlobalIndex] = collection;

                collection.CollectionChanged += (s, e) => OnCollectionChanged(property, s, e);
            }

            return collection;
        }

        private void CoerceAfterFinalBaseValueChanged(ManagedSimpleProperty property, PropertyValue propertyValue, object oldEffectiveValue)
        {
            (object coercedValue, bool coerced) = InternalCoerceValue(property);

            if (coerced && !object.Equals(coercedValue, propertyValue.BaseValue))
                propertyValue.CoercedValue = coercedValue;
            else
                propertyValue.ClearCoercedValue();

            if (!object.Equals(oldEffectiveValue, propertyValue.EffectiveValue))
            {
                OnPropertyValueChanged(property, oldEffectiveValue, propertyValue.EffectiveValue);
            }
        }

        // Protected methods --------------------------------------------------

        protected virtual void OnCollectionChanged(ManagedCollectionProperty property, ManagedCollection collection, CollectionChangedEventArgs e)
        {
            if (property.Metadata.CollectionChangedHandler != null)
                property.Metadata.CollectionChangedHandler.Invoke(this, new ManagedCollectionChangedEventArgs(collection, e.Change, e.ItemsAdded, e.ItemsRemoved));
        }

        protected virtual void OnPropertyValueChanged(ManagedSimpleProperty property, object oldValue, object newValue)
        {
            if (property.Metadata.ValueChangedHandler != null)
                property.Metadata.ValueChangedHandler.Invoke(this, new PropertyValueChangedEventArgs(oldValue, newValue));
        }

        protected virtual void OnReferenceValueChanged(ManagedReferenceProperty referenceProperty, object oldValue, object newValue)
        {
            if (referenceProperty.Metadata.ValueChangedHandler != null)
                referenceProperty.Metadata.ValueChangedHandler.Invoke(this, new PropertyValueChangedEventArgs(oldValue, newValue));
        }

        // Internal methods ---------------------------------------------------

        internal void SetAnimatedValue(ManagedProperty property, object value)
        {
            if (property is not ManagedSimpleProperty simpleProperty)
                throw new InvalidOperationException($"Animated value cannot be set to non-simple property {property.Name} of {GetType().Name}!");
            if (simpleProperty.Metadata.NotAnimatable)
                throw new InvalidOperationException($"Animation is disabled for property {property.Name} of {GetType().Name}!");

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
                throw new InvalidOperationException($"Animated value cannot be set to non-simple property {property.Name} of {GetType().Name}!");

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
                throw new InvalidOperationException($"Cannot coerce non-simple property {property.Name} of {GetType().Name}!");

            var propertyValue = EnsurePropertyValue(simpleProperty);

            var oldEffectiveValue = propertyValue.EffectiveValue;

            CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);
        }

        public ManagedProperty GetProperty(string propertyName)
        {
            return ManagedProperty.ByTypeAndName(GetType(), propertyName);
        }

        public IEnumerable<ManagedProperty> GetProperties(bool includingBaseClasses)
        {
            return ManagedProperty.ByType(GetType(), includingBaseClasses);
        }

        public bool IsPropertySet(ManagedProperty property)
        {
            if (property is ManagedSimpleProperty)
                return propertyValues.ContainsKey(property.GlobalIndex);
            else if (property is ManagedCollectionProperty)
                return collections.ContainsKey(property.GlobalIndex);
            else if (property is ManagedReferenceProperty)
                return references.ContainsKey(property.GlobalIndex);
            else
                throw new InvalidOperationException("Unsupported managed property type!");
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
            else if (property is ManagedReferenceProperty referenceProperty)
            {
                if (references.TryGetValue(property.GlobalIndex, out object value))
                    return value;
                else
                    return null;
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
            if (property is ManagedSimpleProperty simpleProperty)
            {
                ValidateValue(simpleProperty, value);

                var propertyValue = EnsurePropertyValue(simpleProperty);

                if (propertyValue.BaseValue != value)
                {
                    var oldEffectiveValue = propertyValue.EffectiveValue;

                    propertyValue.BaseValue = value;
                    propertyValue.ResetModifiers();

                    // Coertion
                    CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);

                    // Possible notification about base value change goes here
                }
            }
            else if (property is ManagedReferenceProperty referenceProperty)
            {
                ValidateValue(referenceProperty, value);



                if (references.TryGetValue(property.GlobalIndex, out object oldValue))
                {
                    if (oldValue != null && referenceProperty.Metadata.ValueIsPermanent)
                        throw new InvalidOperationException($"Property {property.Name} of type {GetType().Name} has permanent value flag set in metadata. This allows setting its value only once.");
                }
                else
                    oldValue = null;

                references[property.GlobalIndex] = value;

                if (!object.Equals(oldValue, value))
                {
                    OnReferenceValueChanged(referenceProperty, oldValue, value);
                }
            }
        }
    }
}
