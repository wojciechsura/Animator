using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    [DebuggerDisplay("Property value; Base = {baseValue}, Animated = {animatedValue}, Coerced = {coercedValue}")]
    internal class PropertyValue
    {
        // Private fields -----------------------------------------------------

        private PropertyValueSource valueSource;

        private readonly int propertyIndex;

        private bool wasSet = false;

        private object baseValue = null;

        private bool isAnimated = false;
        private object animatedValue = null;

        private bool isCoerced = false;
        private object coercedValue = null;

        // Internal methods ---------------------------------------------------

        internal PropertyValue(int propertyIndex)
        {
            this.propertyIndex = propertyIndex;
        }

        internal PropertyValue(int propertyIndex, object value)
            : this(propertyIndex)
        {
            baseValue = value;
        }

        internal void ClearAnimatedValue()
        {
            isAnimated = false;
            animatedValue = null;
        }

        internal void ClearCoercedValue()
        {
            isCoerced = false;
            coercedValue = null;
        }

        internal void ResetModifiers()
        {
            ClearAnimatedValue();
            ClearCoercedValue();
        }

        internal void SetBaseValue(object value, PropertyValueSource source)
        {
            baseValue = value;
            valueSource = source;

            if (source == PropertyValueSource.Direct)
                wasSet = true;
        }

        internal void SetAnimatedValue(object value)
        {
            isAnimated = true;
            animatedValue = value;
        }

        internal void SetCoercedValue(object value)
        {
            isCoerced = true;
            coercedValue = value;
        }

        internal void SetSource(PropertyValueSource source)
        {
            if (source == PropertyValueSource.Direct)
                wasSet = true;
        }

        // Internal properties ------------------------------------------------

        internal PropertyValueSource ValueSource
        {
            get => valueSource;
        }

        internal object BaseValue
        {
            get => baseValue;
        }

        internal object AnimatedValue
        {
            get => animatedValue;
        }

        internal object CoercedValue
        {
            get => coercedValue;
        }

        internal object FinalBaseValue
        {
            get => isAnimated ? animatedValue : baseValue;
        }

        internal object EffectiveValue
        {
            get
            {
                if (isCoerced)
                    return coercedValue;
                if (isAnimated)
                    return animatedValue;

                return baseValue;
            }
        }

        public int PropertyIndex => propertyIndex;

        public bool IsCoerced => isCoerced;

        public bool IsAnimated => isAnimated;

        public bool IsPureDefault => valueSource == PropertyValueSource.Default && !isAnimated;

        public bool WasSet => wasSet;
    }
}
