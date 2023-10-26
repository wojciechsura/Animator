using Animator.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// This is an abstract class serving as a base class for all animations.
    /// An animation is an object, which animates properties of elements on
    /// the scene.
    /// </summary>
    public abstract partial class Animation : Element
    {
        /// <summary>
        /// Applies animation to property of an element.
        /// </summary>
        /// <param name="timeMs">Time since the scene started.</param>
        /// <returns>True if applying animation caused change in one of this or child
        /// element's ManagedProperties.</returns>
        public abstract bool ApplyAnimation(float timeMs);

        public abstract void ResetAnimation();

        [DoNotDocument]
        public SceneElement AnimatedObject
        {
            get
            {
                if (ParentInfo?.Parent is Animation baseAnimation)
                    return baseAnimation.AnimatedObject;
                else if (ParentInfo?.Parent is SceneElement animatedObject)
                    return animatedObject;
                else
                    throw new InvalidOperationException("Cannot retrieve animated object!");
            }
        }
    }
}
