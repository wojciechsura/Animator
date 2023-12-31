﻿using Animator.Engine.Base.Extensions;
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

        private ParentInfo parent;

        // Private methods ----------------------------------------------------

        private void SetBaseValue(ManagedSimpleProperty simpleProperty, object value, PropertyValueSource source = PropertyValueSource.Direct)
        {
            ValidateSimpleValue(simpleProperty, value);

            var propertyValue = EnsurePropertyValue(simpleProperty);

            if (propertyValue.BaseValue != value)
            {
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.SetBaseValue(value, source);
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
            else
            {
                propertyValue.SetSource(source);
            }
        }

        private void ValidateSimpleValue(ManagedSimpleProperty simpleProperty, object value)
        {
            if (simpleProperty.Type.IsValueType && value == null)
                throw new ArgumentException($"{simpleProperty.OwnerClassType.Name}.{simpleProperty.Name} property type is value-type ({simpleProperty.Type.Name}), but provided value is null.");

            if ((value != null && !value.GetType().IsAssignableTo(simpleProperty.Type)) ||
                (value == null && simpleProperty.Type != typeof(string)))
                throw new ArgumentException($"Value of type {value.GetType().Name} cannot be assigned to property {simpleProperty.OwnerClassType.Name}.{simpleProperty.Name} of type {simpleProperty.Type.Name}.");

            if (simpleProperty.Metadata.ValueValidationHandler != null && !simpleProperty.Metadata.ValueValidationHandler.Invoke(this, new ValueValidationEventArgs(value)))
                throw new ArgumentException($"Value {value} failed validation for property {simpleProperty.OwnerClassType.Name}{simpleProperty.Name}");
        }

        private void ValidateReferenceValue(ManagedReferenceProperty refProperty, object value)
        {
            if (value != null && !value.GetType().IsAssignableTo(refProperty.Type))
                throw new ArgumentException($"Value of type {value.GetType().Name} cannot be assigned to property {refProperty.OwnerClassType.Name}.{refProperty.Name} of type {refProperty.Type.Name}.");

            if (refProperty.Metadata.ValueValidationHandler != null && !refProperty.Metadata.ValueValidationHandler.Invoke(this, new ValueValidationEventArgs(value)))
                throw new ArgumentException($"Value {value} failed validation for property {refProperty.OwnerClassType.Name}{refProperty.Name}");

            if (value is ManagedObject managedObject && managedObject.ParentInfo != null)
                throw new ArgumentException($"Value {value} is already assigned to property of other object ({managedObject.ParentInfo.Parent.GetType().Name}.{managedObject.ParentInfo.Property.Name}). Animator base engine allows object to be assigned only to a single parent at a time.");
        }

        private void ValidateCollectionValue(ManagedObject managedObject)
        {
            if (managedObject.ParentInfo != null)
                throw new ArgumentException($"Value {managedObject} is already assigned to property of other object ({managedObject.ParentInfo.Parent.GetType().Name}.{managedObject.ParentInfo.Property.Name}). Animator base engine allows object to be assigned only to a single parent at a time.");
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

            InitializeBaseValue(simpleProperty, propertyValue);
            return propertyValue;
        }

        private void InitializeBaseValue(ManagedSimpleProperty simpleProperty, PropertyValue propertyValue)
        {
            // Check, if inherited value exists

            if (simpleProperty.Metadata.InheritedFromParent &&
                parent != null)
            {
                var parentProperty = parent.Parent.GetProperty(simpleProperty.Name);
                if (parentProperty is ManagedSimpleProperty parentSimpleProperty &&
                    parentSimpleProperty.Metadata.Inheritable &&
                    parentProperty.Type == simpleProperty.Type)
                {
                    propertyValue.SetBaseValue(parent.Parent.GetValue(parentProperty), PropertyValueSource.Inherited);
                    return;
                }
            }

            // Use default value otherwise
            propertyValue.SetBaseValue(simpleProperty.Metadata.DefaultValue, PropertyValueSource.Default);
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

            if (coerced && !object.Equals(coercedValue, propertyValue.FinalBaseValue))
                propertyValue.SetCoercedValue(coercedValue);
            else
                propertyValue.ClearCoercedValue();

            if (!object.Equals(oldEffectiveValue, propertyValue.EffectiveValue))
                InternalPropertyValueChanged(property, oldEffectiveValue, propertyValue.EffectiveValue);
        }

        private void SetParent(ParentInfo newParent)
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
            if (e.ItemsAdded != null)
                foreach (ManagedObject addedElement in e.ItemsAdded.OfType<ManagedObject>())
                {
                    ValidateCollectionValue(addedElement);
                }

            if (e.ItemsRemoved != null)
                foreach (ManagedObject removedElement in e.ItemsRemoved.OfType<ManagedObject>())
                    removedElement.ParentInfo = null;

            if (e.ItemsAdded != null)
                foreach (ManagedObject addedElement in e.ItemsAdded.OfType<ManagedObject>())
                    addedElement.ParentInfo = new ParentInfo(this, property);

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
                    var simpleProperty = (ManagedSimpleProperty)ManagedProperty.FindByGlobalIndex(inheritedValue.PropertyIndex);

                    var oldValue = inheritedValue.FinalBaseValue;
                    var newValue = simpleProperty.Metadata.DefaultValue;

                    SetBaseValue(simpleProperty, newValue, PropertyValueSource.Default);                   
                }

                parent.Parent.PropertyValueChanged -= HandleParentPropertyValueChanged;
            }

            OnParentDetaching();
        }

        private void InternalParentAttached()
        {
            if (parent != null)
            {
                parent.Parent.PropertyValueChanged += HandleParentPropertyValueChanged;

                // We convert all PropertyValues, which currently serve the default value
                // to inherited ones (if it is possible)
                foreach (var simpleProperty in GetProperties(true)
                    .OfType<ManagedSimpleProperty>()
                    .Where(sp => sp.Metadata.InheritedFromParent))
                {
                    var parentProperty = parent.Parent.GetProperty(simpleProperty.Name);
                    if (parentProperty is ManagedSimpleProperty parentSimpleProperty && 
                        parentSimpleProperty.Metadata.Inheritable &&
                        parentProperty.Type == simpleProperty.Type)
                    {
                        var inheritedValue = parent.Parent.GetValue(parentProperty);

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
                oldBaseElement.ParentInfo = null;

            if (newValue is ManagedObject newBaseElement)
                newBaseElement.ParentInfo = new ParentInfo(this, referenceProperty);

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
            GetType().StaticInitializeRecursively();
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

            if (!propertyValue.IsAnimated || !object.Equals(propertyValue.EffectiveValue, value))
            {
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.SetAnimatedValue(value);

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
            return ManagedProperty.FindByTypeAndName(GetType(), propertyName);
        }

        public IEnumerable<ManagedProperty> GetProperties(bool includingBaseClasses)
        {
            return ManagedProperty.FindAllByType(GetType(), includingBaseClasses);
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
                if (simpleProperty.Metadata.ValueIsPermanent && propertyValues.TryGetValue(simpleProperty.GlobalIndex, out var propertyValue) && propertyValue.WasSet)
                    throw new InvalidOperationException($"Property {property.Name} of type {GetType().Name} has permanent value flag set in metadata. This allows setting its value only once.");

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

        public void ProcessChildren<T>(Action<T> action)
        {
            foreach (var prop in GetProperties(true).Where(p => IsPropertySet(p)))
            {
                if (prop is ManagedReferenceProperty refProp)
                {
                    var value = GetValue(prop);
                    if (value is T t)
                        action(t);
                }
                else if (prop is ManagedCollectionProperty collectionProp)
                {
                    var collection = GetValue(collectionProp) as ManagedCollection;

                    foreach (var obj in collection)
                        if (obj is T t)
                            action(t);
                }
            }
        }

        public List<T> FindChildren<T>(Func<T, bool> predicate)
            where T : ManagedObject
        {
            List<T> result = new();

            ProcessChildren<T>(child =>
            {
                if (predicate(child))
                    result.Add(child);
            });

            return result;
        }

        public void ProcessElementsRecursive<T>(Action<T> action)
            where T : ManagedObject
        {
            if (this is T t)
                action(t);

            ProcessChildren<T>(child => child.ProcessElementsRecursive(action));
        }

        public ManagedObject Clone()
        {
            var type = GetType();
            var clone = (ManagedObject) Activator.CreateInstance(type);

            foreach (var property in GetProperties(true))
            {
                if (IsPropertySet(property))
                {
                    if (property is ManagedSimpleProperty)
                    {
                        clone.SetValue(property, GetValue(property));
                    }
                    else if (property is ManagedReferenceProperty)
                    {
                        object value = GetValue(property);

                        if (value is ManagedObject managedObject)
                        {
                            clone.SetValue(property, managedObject.Clone());
                        }
                        else if (value is ICloneable cloneable)
                        {
                            clone.SetValue(property, cloneable.Clone());
                        }
                        else if (value == null)
                        {
                            clone.SetValue(property, null);
                        }
                        else
                            throw new InvalidOperationException("Cannot clone: managed reference property is of non-cloneable type!");
                    }
                    else if (property is ManagedCollectionProperty)
                    {
                        IList otherList = (IList)clone.GetValue(property);
                        otherList.Clear();

                        IList thisList = (IList)GetValue(property);

                        foreach (var item in thisList.Cast<ManagedObject>())
                        {
                            otherList.Add(item.Clone());
                        }
                    }
                    else
                        throw new InvalidOperationException("Unsupported property type!");
                }
            }

            return clone;
        }

        // Public properties --------------------------------------------------

        public ParentInfo ParentInfo
        {
            get => parent;
            internal set => SetParent(value);
        }

        public event PropertyValueChangedDelegate PropertyValueChanged;

        public event PropertyBaseValueChangedDelegate PropertyBaseValueChanged;
    }
}
