using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ValueChangedEventArgs : EventArgs
    {
        public ValueChangedEventArgs(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object OldValue { get; }
        public object NewValue { get; }
    }

    public delegate void ValueChangedDelegate(object sender, ValueChangedEventArgs args);

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

        // Protected methods --------------------------------------------------

        protected virtual object ProvideProperty(ManagedProperty property)
        {
            if (propertyValues.TryGetValue(property.GlobalIndex, out BasePropertyValue value))
            {
                return value.EffectiveValue;
            }

            return ManagedProperty.UnsetValue;
        }

        // Internal methods ---------------------------------------------------

        internal void SetAnimatedValue(ManagedProperty property, object value)
        {
            if (propertyValues.TryGetValue(property.GlobalIndex, out BasePropertyValue propertyValue))
            {
                propertyValue.SetAnimatedValue(value);
            }
            else
            {
                propertyValue = new DirectPropertyValue(property.GlobalIndex, property.Metadata.DefaultValue);
                propertyValue.SetAnimatedValue(value);
            }
             
            // TODO notify about value change
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
            var result = ProvideProperty(property);

            if (result == ManagedProperty.UnsetValue)
                return null;

            return result;
        }

        public void SetValue(ManagedProperty property, object value)
        {
            ValidateValue(property, value);

            BasePropertyValue currentValue;

            propertyValues.TryGetValue(property.GlobalIndex, out currentValue);

            var newValue = new DirectPropertyValue(property.GlobalIndex, value);

            propertyValues[property.GlobalIndex] = newValue;

            // TODO notify about base value change
        }

        // Public properties --------------------------------------------------

        public event ValueChangedDelegate PropertyValueChanged;

        public event ValueChangedDelegate PropertyBaseValueChanged;
    }
}
