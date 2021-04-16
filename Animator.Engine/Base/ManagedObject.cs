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

        private readonly Dictionary<int, BasePropertyValue> propertyValues = new Dictionary<int, BasePropertyValue>();

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
                BasePropertyValue propertyValue = EnsurePropertyValue(property);

                var oldValue = propertyValue.EffectiveValue;

                var newValue = property.Metadata.CoerceValueHandler(this, propertyValue.FinalBaseValue);

                if (newValue != oldValue)
                    return (newValue, true);                
            }

            return (null, false);
        }

        private BasePropertyValue EnsurePropertyValue(ManagedProperty property)
        {
            if (!propertyValues.TryGetValue(property.GlobalIndex, out BasePropertyValue propertyValue))
            {
                propertyValue = new DirectPropertyValue(property.GlobalIndex, property.Metadata.DefaultValue);
                propertyValues[property.GlobalIndex] = propertyValue;
            }

            return propertyValue;
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
            object oldValue = propertyValue.AnimatedValue;

            if (oldValue != value)
            {
                propertyValue.AnimatedValue = value;

                (object coercedValue, bool coerced) = InternalCoerceValue(property);

                if (coerced && coercedValue != propertyValue.CoercedValue)
                {


                    if (oldValue != value)
                        OnPropertyValueChanged(property, oldValue, value);
                }
            }
        }

        // Public methods -----------------------------------------------------

        public ManagedObject()
        {

        }

        public ManagedProperty GetProperty(string propertyName)
        {
            return ManagedProperty.ByTypeAndName(GetType(), propertyName);
        }

        public object GetValue(ManagedProperty property)
        {
            var result = ProvidePropertyValue(property);

            if (result == ManagedProperty.UnsetValue)
                return null;

            return result;
        }

        public void SetValue(ManagedProperty property, object value)
        {
            ValidateValue(property, value);

            BasePropertyValue currentValue;

            propertyValues.TryGetValue(property.GlobalIndex, out currentValue);

            if (currentValue == null || currentValue.BaseValue != value)
            {
                var newValue = new DirectPropertyValue(property.GlobalIndex, value);

                propertyValues[property.GlobalIndex] = newValue;

                OnPropertyBaseValueChanged(property, currentValue?.BaseValue, value);
                OnPropertyValueChanged(property, currentValue?.BaseValue, value);
            }
        }

        // Public properties --------------------------------------------------

        public event ValueChangedDelegate PropertyValueChanged;

        public event ValueChangedDelegate PropertyBaseValueChanged;
    }
}
