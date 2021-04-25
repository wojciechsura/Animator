using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Animator.Engine.Base.BasePropertyMetadata;

// TODO Make sure, that Value change notification is raised only when value actually changes

namespace Animator.Engine.Base
{
    public class PropertyBaseValueChangedEventArgs : EventArgs
    {
        public PropertyBaseValueChangedEventArgs(ManagedProperty property, object newValue)
        {
            Property = property;
            NewValue = newValue;
        }

        public ManagedProperty Property { get; }
        public object NewValue { get; }
    }

    public delegate void PropertyBaseValueChangedDelegate(ManagedObject sender, PropertyBaseValueChangedEventArgs args);

    public abstract class ManagedObject
    {
        // Private static fields ----------------------------------------------

        private static readonly HashSet<Type> staticallyInitializedTypes = new();

        // Private fields -----------------------------------------------------

        private readonly Dictionary<int, PropertyValue> propertyValues = new();
        private readonly Dictionary<int, ManagedCollection> collections = new();
        private readonly Dictionary<int, object> references = new();

        private ManagedObject parent;

        // Private static methods ---------------------------------------------

        private static void StaticInitializeRecursively(Type type)
        {
            do
            {
                // If type is initialized, its base types must have been initialized too,
                // don't waste time on them
                if (staticallyInitializedTypes.Contains(type))
                    return;

                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

                staticallyInitializedTypes.Add(type);

                type = type.BaseType;
            }
            while (type != typeof(ManagedObject) && type != typeof(object));
        }

        // Private methods ----------------------------------------------------

        private void SetBaseValue(ManagedSimpleProperty simpleProperty, object value, PropertyValueSource source = PropertyValueSource.Direct)
        {
            ValidateSimpleValue(simpleProperty, value);

            var propertyValue = EnsurePropertyValue(simpleProperty);
            propertyValue.ValueSource = source;

            if (propertyValue.BaseValue != value)
            {
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.BaseValue = value;
                propertyValue.ResetModifiers();

                if (source != PropertyValueSource.Default)
                {
                    // Coercion
                    // Force value change notification, so that eg. animation can pick off changed base value
                    CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);
                }
                else
                {
                    // No coercion, but inform about value change
                    if (!object.Equals(oldEffectiveValue, value))
                        InternalPropertyValueChanged(simpleProperty, oldEffectiveValue, value);
                }

                InternalPropertyBaseValueChanged(simpleProperty, value);
            }
        }

        private void ValidateSimpleValue(ManagedSimpleProperty simpleProperty, object value)
        {
            if (simpleProperty.Type.IsValueType && value == null)
                throw new ArgumentException($"{simpleProperty.OwnerClassType.Name}.{simpleProperty.Name} property type is value-type ({simpleProperty.Type.Name}), but provided value is null.");

            if (!value.GetType().IsAssignableTo(simpleProperty.Type))
                throw new ArgumentException($"Value of type {value.GetType().Name} cannot be assigned to property {simpleProperty.OwnerClassType.Name}.{simpleProperty.Name} of type {simpleProperty.Type.Name}.");
        }

        private void ValidateReferenceValue(ManagedReferenceProperty refProperty, object value)
        {
            if (value != null && !value.GetType().IsAssignableTo(refProperty.Type))
                throw new ArgumentException($"Value of type {value.GetType().Name} cannot be assigned to property {refProperty.OwnerClassType.Name}.{refProperty.Name} of type {refProperty.Type.Name}.");

            if (refProperty.Metadata.ValueValidationHandler != null && !refProperty.Metadata.ValueValidationHandler.Invoke(this, new ValueValidationEventArgs(value)))
                throw new ArgumentException($"Value {value} failed validation for property {refProperty.OwnerClassType.Name}{refProperty.Name}");
        }

        private (object, bool) InternalCoerceValue(ManagedSimpleProperty simpleProperty)
        {
            if (simpleProperty.Metadata.CoerceValueHandler != null)
            {
                PropertyValue propertyValue = EnsurePropertyValue(simpleProperty);

                var oldValue = propertyValue.EffectiveValue;

                var newValue = simpleProperty.Metadata.CoerceValueHandler(this, propertyValue.FinalBaseValue);

                if (newValue != oldValue)
                    return (newValue, true);
            }

            return (null, false);
        }

        private PropertyValue EnsurePropertyValue(ManagedSimpleProperty simpleProperty)
        {
            // If it already exists, there is nothing to do.

            if (propertyValues.TryGetValue(simpleProperty.GlobalIndex, out PropertyValue propertyValue))
                return propertyValue;

            propertyValue = new PropertyValue(simpleProperty.GlobalIndex);
            propertyValues[simpleProperty.GlobalIndex] = propertyValue;

            return InitializeBaseValue(simpleProperty, propertyValue);
        }

        private PropertyValue InitializeBaseValue(ManagedSimpleProperty simpleProperty, PropertyValue propertyValue)
        {
            // Check, if inherited value exists

            if (simpleProperty.Metadata.InheritedFromParent &&
                parent != null)
            {
                var parentProperty = parent.GetProperty(simpleProperty.Name);
                if (parentProperty != null && parentProperty is ManagedSimpleProperty && parentProperty.Type == simpleProperty.Type)
                {
                    propertyValue.BaseValue = parent.GetValue(parentProperty);
                    propertyValue.ValueSource = PropertyValueSource.Inherited;

                    return propertyValue;
                }
            }

            // Use default value otherwise

            propertyValue.BaseValue = simpleProperty.Metadata.DefaultValue;
            propertyValue.ValueSource = PropertyValueSource.Default;

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

                collection.CollectionChanged += (s, e) => InternalCollectionChanged(property, s, e);
            }

            return collection;
        }

        private void CoerceAfterFinalBaseValueChanged(ManagedSimpleProperty property, PropertyValue propertyValue, object oldEffectiveValue)
        {
            // Default value is never coerced
            if (propertyValue.IsPureDefault)
                return;

            (object coercedValue, bool coerced) = InternalCoerceValue(property);

            if (coerced && !object.Equals(coercedValue, propertyValue.BaseValue))
                propertyValue.CoercedValue = coercedValue;
            else
                propertyValue.ClearCoercedValue();

            if (!object.Equals(oldEffectiveValue, propertyValue.EffectiveValue))
                InternalPropertyValueChanged(property, oldEffectiveValue, propertyValue.EffectiveValue);
        }

        private void SetParent(ManagedObject newParent)
        {
            if (parent != newParent)            
            {
                InternalParentDetaching();

                parent = newParent;

                InternalParentAttached();
            }
        }

        private void InternalCollectionChanged(ManagedCollectionProperty property, ManagedCollection collection, CollectionChangedEventArgs e)
        {
            if (e.ItemsRemoved != null)
                foreach (ManagedObject removedElement in e.ItemsRemoved.OfType<ManagedObject>())
                    removedElement.Parent = null;

            if (e.ItemsAdded != null)
                foreach (ManagedObject addedElement in e.ItemsAdded.OfType<ManagedObject>())
                    addedElement.Parent = this;

            if (property.Metadata.CollectionChangedHandler != null)
                property.Metadata.CollectionChangedHandler.Invoke(this, new ManagedCollectionChangedEventArgs(collection, e.Change, e.ItemsAdded, e.ItemsRemoved));

            OnCollectionChanged(property, collection, e);
        }

        private void HandleParentPropertyValueChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            // If parent's changed property matches this object's property, which
            // is inherited and has default value, notify about its change too

            if (GetProperty(args.Property.Name) is ManagedSimpleProperty simpleProperty &&
                simpleProperty.Metadata.InheritedFromParent &&
                args.Property is ManagedSimpleProperty &&
                simpleProperty.Type == args.Property.Type)
            {
                if (propertyValues.TryGetValue(simpleProperty.GlobalIndex, out PropertyValue propertyValue))
                {
                    switch (propertyValue.ValueSource)
                    {
                        case PropertyValueSource.Default:
                            {
                                // This should never happen.
                                throw new InvalidOperationException($"Internal algorithms failure: property {simpleProperty.Name}, which inherits value from parent, currently have PropertyValue with Source set to Default. This shouldn't be possible.");
                            }
                        case PropertyValueSource.Direct:
                            {
                                // Since user manually set value to this property, there's nothing to do.
                                break;
                            }
                        case PropertyValueSource.Inherited:
                            {
                                if (!object.Equals(propertyValue.BaseValue, args.NewValue))
                                {
                                    SetBaseValue(simpleProperty, args.NewValue, PropertyValueSource.Inherited);
                                }
                                break;
                            }
                        default:
                            throw new InvalidEnumArgumentException("Unsupported ValueSource!");
                    }
                }
                else
                {
                    propertyValue = new PropertyValue(simpleProperty.GlobalIndex);
                    propertyValues[simpleProperty.GlobalIndex] = propertyValue;

                    SetBaseValue(simpleProperty, args.NewValue, PropertyValueSource.Inherited);
                }
            }
        }

        private void InternalParentDetaching()
        {
            if (parent != null)
            {
                // Clear all inheirted properties, since we have no parent from now on
                var inheritedValues = propertyValues.Values.Where(pv => pv.ValueSource == PropertyValueSource.Inherited);

                foreach (var inheritedValue in inheritedValues)
                {
                    var simpleProperty = (ManagedSimpleProperty)ManagedProperty.ByGlobalIndex(inheritedValue.PropertyIndex);

                    var oldValue = inheritedValue.FinalBaseValue;
                    var newValue = simpleProperty.Metadata.DefaultValue;

                    SetBaseValue(simpleProperty, newValue, PropertyValueSource.Default);                   
                }

                parent.PropertyValueChanged -= HandleParentPropertyValueChanged;
            }

            OnParentDetaching();
        }

        private void InternalParentAttached()
        {
            if (parent != null)
            {
                parent.PropertyValueChanged += HandleParentPropertyValueChanged;

                // We convert all PropertyValues, which currently serve the default value
                // to inherited ones (if it is possible)
                foreach (var simpleProperty in GetProperties(true)
                    .OfType<ManagedSimpleProperty>()
                    .Where(sp => sp.Metadata.InheritedFromParent))
                {
                    var parentProperty = parent.GetProperty(simpleProperty.Name);
                    if (parentProperty != null && parentProperty is ManagedSimpleProperty && parentProperty.Type == simpleProperty.Type)
                    {
                        var inheritedValue = parent.GetValue(parentProperty);

                        if (propertyValues.TryGetValue(simpleProperty.GlobalIndex, out PropertyValue propertyValue))                            
                        {
                            if (propertyValue.ValueSource == PropertyValueSource.Default)
                            {
                                // If there is a PropertyValue with source set to Default, replace its
                                // value with inherited one and modify the source.

                                SetBaseValue(simpleProperty, inheritedValue, PropertyValueSource.Inherited);
                            }
                        }
                        else
                        {
                            propertyValue = new PropertyValue(simpleProperty.GlobalIndex);
                            propertyValues[simpleProperty.GlobalIndex] = propertyValue;

                            SetBaseValue(simpleProperty, inheritedValue, PropertyValueSource.Inherited);
                        }
                    }
                }
            }

            OnParentAttached();
        }

        private void InternalPropertyBaseValueChanged(ManagedSimpleProperty simpleProperty, object newValue)
        {
            PropertyBaseValueChanged?.Invoke(this, new PropertyBaseValueChangedEventArgs(simpleProperty, newValue));
        }

        private void InternalPropertyValueChanged(ManagedSimpleProperty property, object oldValue, object newValue)
        {
            if (property.Metadata.ValueChangedHandler != null)
                property.Metadata.ValueChangedHandler.Invoke(this, new PropertyValueChangedEventArgs(property, oldValue, newValue));

            if (object.Equals(oldValue, newValue))
                throw new InvalidOperationException("ValueChange called, but old and new values are equal!");

            PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(property, oldValue, newValue));

            OnPropertyValueChanged(property, oldValue, newValue);
        }

        private void InternalReferenceValueChanged(ManagedReferenceProperty referenceProperty, object oldValue, object newValue)
        {
            if (oldValue is ManagedObject oldBaseElement)
                oldBaseElement.Parent = null;

            if (newValue is ManagedObject newBaseElement)
                newBaseElement.Parent = this;

            if (referenceProperty.Metadata.ValueChangedHandler != null)
                referenceProperty.Metadata.ValueChangedHandler.Invoke(this, new PropertyValueChangedEventArgs(referenceProperty, oldValue, newValue));

            OnReferenceValueChanged(referenceProperty, oldValue, newValue);
        }

        // Protected methods --------------------------------------------------

        protected virtual void OnParentDetaching()
        {

        }

        protected virtual void OnParentAttached()
        {

        }

        protected virtual void OnCollectionChanged(ManagedCollectionProperty property, ManagedCollection collection, CollectionChangedEventArgs e)
        {

        }

        protected virtual void OnPropertyValueChanged(ManagedSimpleProperty property, object oldValue, object newValue)
        {

        }        

        protected virtual void OnReferenceValueChanged(ManagedReferenceProperty referenceProperty, object oldValue, object newValue)
        {

        }

        // Public methods -----------------------------------------------------

        public ManagedObject()
        {
            StaticInitializeRecursively(GetType());    
        }

        public void CoerceValue(ManagedProperty property)
        {
            if (property is not ManagedSimpleProperty simpleProperty)
                throw new InvalidOperationException($"Cannot coerce non-simple property {property.Name} of {GetType().Name}!");

            var propertyValue = EnsurePropertyValue(simpleProperty);

            // Default value is never coerced
            if (propertyValue.ValueSource == PropertyValueSource.Default)
                return;

            var oldEffectiveValue = propertyValue.EffectiveValue;

            CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);
        }

        public void SetAnimatedValue(ManagedProperty property, object value)
        {
            if (property is not ManagedSimpleProperty simpleProperty)
                throw new InvalidOperationException($"Animated value cannot be set to non-simple property {property.Name} of {GetType().Name}!");
            if (simpleProperty.Metadata.NotAnimatable)
                throw new InvalidOperationException($"Animation is disabled for property {property.Name} of {GetType().Name}!");

            var propertyValue = EnsurePropertyValue(simpleProperty);

            if (!propertyValue.IsAnimated || !object.Equals(propertyValue.AnimatedValue, value))
            {
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.AnimatedValue = value;

                CoerceAfterFinalBaseValueChanged(simpleProperty, propertyValue, oldEffectiveValue);
            }
        }

        public void ClearAnimatedValue(ManagedProperty property)
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
            {
                if (!propertyValues.TryGetValue(property.GlobalIndex, out PropertyValue value))
                    return false;

                // Note: we treat inherited value as set

                return !value.IsPureDefault;
            }
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
                var propertyValue = EnsurePropertyValue(simpleProperty);
                return propertyValue.EffectiveValue;
            }
            else if (property is ManagedReferenceProperty referenceProperty)
            {
                if (references.TryGetValue(referenceProperty.GlobalIndex, out object value))
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

        public object GetBaseValue(ManagedProperty property)
        {
            if (property is ManagedSimpleProperty simpleProperty)
            {
                var propertyValue = EnsurePropertyValue(simpleProperty);
                return propertyValue.BaseValue;
            }
            else
                throw new ArgumentException("Base value is available only for simple properties!");
        }

        public object GetFinalBaseValue(ManagedProperty property)
        {
            if (property is ManagedSimpleProperty simpleProperty)
            {
                var propertyValue = EnsurePropertyValue(simpleProperty);
                return propertyValue.FinalBaseValue;
            }
            else
                throw new ArgumentException("Final base value is available only for simple properties!");
        }

        public void SetValue(ManagedProperty property, object value)
        {
            if (property is ManagedSimpleProperty simpleProperty)
            {
                SetBaseValue(simpleProperty, value);
            }
            else if (property is ManagedReferenceProperty referenceProperty)
            {
                ValidateReferenceValue(referenceProperty, value);

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
                    InternalReferenceValueChanged(referenceProperty, oldValue, value);
                }
            }
        }

        // Public properties --------------------------------------------------

        public ManagedObject Parent
        {
            get => parent;
            internal set => SetParent(value);
        }

        public event PropertyValueChangedDelegate PropertyValueChanged;

        public event PropertyBaseValueChangedDelegate PropertyBaseValueChanged;
    }
}
