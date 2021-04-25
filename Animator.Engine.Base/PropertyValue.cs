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

        // Internal properties ------------------------------------------------

        internal PropertyValueSource ValueSource
        {
            get => valueSource;
            set => valueSource = value;
        }

        internal object BaseValue
        {
            get => baseValue;
            set => baseValue = value;
        }

        internal object AnimatedValue
        {
            get => animatedValue;
            set 
            {
                isAnimated = true;
                animatedValue = value;
            }
        }

        internal object CoercedValue
        {
            get => coercedValue;
            set
            {
                if (valueSource == PropertyValueSource.Default)
                    throw new InvalidOperationException("Attempt to coerce default value!");

                isCoerced = true;
                coercedValue = value;
            }
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
    }
}
