using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    internal abstract class BasePropertyValue
    {
        private object animatedValue = null;
        private bool isAnimated = false;

        protected abstract object ProvideValue();

        public void ClearAnimatedValue()
        {
            animatedValue = null;
            isAnimated = false;
        }

        public object AnimatedValue
        {
            get => animatedValue;
            set
            {
                animatedValue = value;
                isAnimated = true;
            }
        }

        public bool IsAnimated => isAnimated;

        public object BaseValue => ProvideValue();
    }
}
