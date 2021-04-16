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

        // Protected methods --------------------------------------------------

        protected abstract object ProvideBaseValue();

        // Internal methods ---------------------------------------------------

        internal BasePropertyValue(int propertyIndex)
        {
            this.propertyIndex = propertyIndex;
        }

        internal void SetAnimatedValue(object value)
        {
            isAnimated = true;
            animatedValue = value;
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

        internal object EffectiveValue
        {
            get
            {
                if (isAnimated)
                    return animatedValue;

                return ProvideBaseValue();
            }
        }
    }
}
