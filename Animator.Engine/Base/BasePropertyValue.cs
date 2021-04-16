using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    internal abstract class BasePropertyValue
    {
        // Private fields -----------------------------------------------------

        private readonly int propertyIndex;
        private bool isAnimated = false;
        private object animatedValue = null;
        private bool isCoerced = false;
        private object coercedValue = null;

        // Protected methods --------------------------------------------------

        protected abstract object ProvideBaseValue();

        // Internal methods ---------------------------------------------------

        internal BasePropertyValue(int propertyIndex)
        {
            this.propertyIndex = propertyIndex;
        }

        internal void ClearAnimatedValue()
        {
            isAnimated = false;
            animatedValue = null;
        }

        // Internal properties ------------------------------------------------

        internal object BaseValue
        {
            get => ProvideBaseValue();
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

                return ProvideBaseValue();
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

                return ProvideBaseValue();
            }
        }
    }
}
