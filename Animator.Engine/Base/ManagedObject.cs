using System;
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

    public delegate void ValueChangedDelegate(object sender, PropertyValueChangedEventArgs args);

    public abstract class ManagedObject
    {
        // Private fields -----------------------------------------------------

        private readonly Dictionary<int, PropertyValue> propertyValues = new Dictionary<int, PropertyValue>();

        // Private methods ----------------------------------------------------

        private void ValidateValue(ManagedProperty property, object value)
        {
            if (property.Type.IsValueType && value == null)
                throw new ArgumentException($"{property.OwnerClassType.Name}.{property.Name} property type is value-type ({property.Type.Name}), but provided value is null.");

            if (!value.GetType().IsAssignableTo(property.Type))
                throw new ArgumentException($"Value of type {value.GetType().Name} cannot be assigned to property {property.OwnerClassType.Name}.{property.Name} of type {property.Type.Name}.");
        }

        private (object, bool) InternalCoerceValue(ManagedProperty property)
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

        private PropertyValue EnsurePropertyValue(ManagedProperty property)
        {
            if (!propertyValues.TryGetValue(property.GlobalIndex, out PropertyValue propertyValue))
            {
                propertyValue = new PropertyValue(property.GlobalIndex, property.Metadata.DefaultValue);
                propertyValues[property.GlobalIndex] = propertyValue;
            }

            return propertyValue;
        }

        private void CoerceAfterFinalBaseValueChanged(ManagedProperty property, PropertyValue propertyValue, object oldEffectiveValue)
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

        protected virtual void OnPropertyBaseValueChanged(ManagedProperty property, object oldValue, object newValue)
        {
            PropertyBaseValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(property, oldValue, newValue));
        }

        // Internal methods ---------------------------------------------------

        internal void SetAnimatedValue(ManagedProperty property, object value)
        {
            var propertyValue = EnsurePropertyValue(property);

            if (!propertyValue.IsAnimated || propertyValue.AnimatedValue != value)
            {
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.AnimatedValue = value;
                
                CoerceAfterFinalBaseValueChanged(property, propertyValue, oldEffectiveValue);
            }
        }

        // Public methods -----------------------------------------------------

        public ManagedObject()
        {

        }

        public void CoerceValue(ManagedProperty property)
        {
            var propertyValue = EnsurePropertyValue(property);

            var oldEffectiveValue = propertyValue.EffectiveValue;

            CoerceAfterFinalBaseValueChanged(property, propertyValue, oldEffectiveValue);
        }

        public ManagedProperty GetProperty(string propertyName)
        {
            return ManagedProperty.ByTypeAndName(GetType(), propertyName);
        }

        public object GetValue(ManagedProperty property)
        {
            if (propertyValues.TryGetValue(property.GlobalIndex, out PropertyValue propertyValue))
                return propertyValue.EffectiveValue;
            else
                return property.Metadata.DefaultValue;
        }

        public object GetFinalBaseValue(ManagedProperty property)
        {
            if (propertyValues.TryGetValue(property.GlobalIndex, out PropertyValue propertyValue))
                return propertyValue.FinalBaseValue;
            else
                return property.Metadata.DefaultValue;
        }

        public void SetValue(ManagedProperty property, object value)
        {
            ValidateValue(property, value);

            var propertyValue = EnsurePropertyValue(property);

            if (propertyValue.BaseValue != value)
            {
                var oldBaseValue = propertyValue.BaseValue;
                var oldEffectiveValue = propertyValue.EffectiveValue;

                propertyValue.BaseValue = value;
                propertyValue.ResetModifiers();

                OnPropertyBaseValueChanged(property, oldBaseValue, value);

                // Coertion
                CoerceAfterFinalBaseValueChanged(property, propertyValue, oldEffectiveValue);
            }
        }

        // Public properties --------------------------------------------------

        public event ValueChangedDelegate PropertyValueChanged;

        public event ValueChangedDelegate PropertyBaseValueChanged;
    }
}
