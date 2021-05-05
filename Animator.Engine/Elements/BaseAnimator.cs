using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// This is an abstract class serving as a base class for all animators.
    /// An animator is an object, which animates properties of elements on
    /// the scene.
    /// </summary>
    public abstract class BaseAnimator : BaseElement
    {
        public abstract void ApplyAnimation(float timeMs);

        public abstract void ResetAnimation();

        public BaseElement AnimatedObject
        {
            get
            {
                if (Parent is BaseAnimator baseAnimator)
                    return baseAnimator.AnimatedObject;
                else if (Parent is BaseElement animatedObject)
                    return animatedObject;
                else
                    throw new InvalidOperationException("Cannot retrieve animated object!");
            }
        }
    }
}
