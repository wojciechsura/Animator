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

        internal object BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
            }
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
                isCoerced = true;
                coercedValue = value;
            }
        }

        internal object FinalBaseValue
        {
            get
            {
                if (isAnimated)
                    return animatedValue;

                return baseValue;
            }
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

        public bool IsCoerced => isCoerced;

        public bool IsAnimated => isAnimated;
    }
}
