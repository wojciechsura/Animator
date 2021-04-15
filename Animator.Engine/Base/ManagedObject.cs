using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    
    public abstract class ManagedObject
    {
        // Private fields -----------------------------------------------------

        private readonly SortedList<int, BasePropertyValue> propertyValues = new();

        // Protected methods --------------------------------------------------

        protected virtual object ProvidePropertyValue(ManagedProperty property)
        {
            if (propertyValues.TryGetValue(property.GlobalIndex, out BasePropertyValue propertyValue))
            {
                if (propertyValue.IsAnimated)
                    return propertyValue.AnimatedValue;

                return propertyValue.BaseValue;
            }

            return ManagedProperty.UnsetValue;
        }

        // Internal methods ---------------------------------------------------

        internal void SetAnimatedValue(ManagedProperty property, object value)
        {
            BasePropertyValue propertyValue;

            if (!propertyValues.TryGetValue(property.GlobalIndex, out propertyValue))
                propertyValue = new PureAnimatedValue();

            propertyValue.AnimatedValue = value;
        }

        internal void ClearAnimatedValue(ManagedProperty property)
        {
            BasePropertyValue value;

            if (propertyValues.TryGetValue(property.GlobalIndex, out value))
            {
                if (value is PureAnimatedValue)
                {
                    // This value was set only during animation, so we have
                    // to remove it completely

                    propertyValues.Remove(property.GlobalIndex);
                }
                else
                {
                    value.ClearAnimatedValue();
                }
            }
        }

        internal BasePropertyValue GetPropertyValue(ManagedProperty property)
        {
            if (propertyValues.TryGetValue(property.GlobalIndex, out BasePropertyValue propertyValue))
                return propertyValue;

            return null;
        }

        // Public methods -----------------------------------------------------

        public ManagedObject()
        {

        }

        public object GetValue(ManagedProperty property)
        {
            var value = ProvidePropertyValue(property);

            if (value != ManagedProperty.UnsetValue)
            {
                // Return retrieved value

                return value;
            }
            else
            {
                // Default value defined in metadata, if any

                if (property.Metadata.DefaultValue != null)
                    return property.Metadata.DefaultValue;

                // Default value for type

                if (property.Type.IsValueType)
                    return Activator.CreateInstance(property.Type);

                return null;
            }
        }

        public void SetValue(ManagedProperty property, object value)
        {
            var propertyValue = new DirectPropertyValue(value);

            propertyValues[property.GlobalIndex] = propertyValue;
        }

        public void SetExpression(ManagedProperty property, BaseExpression expression)
        {
            var propertyValue = new ExpressionPropertyValue(expression);

            propertyValues[property.GlobalIndex] = propertyValue;
        }        

        public void ResetProperty(ManagedProperty property)
        {
            propertyValues.Remove(property.GlobalIndex);
        }

        public ManagedProperty GetProperty(string propertyName)
        {
            return ManagedProperty.ByTypeAndName(GetType(), propertyName);
        }
    }
}
