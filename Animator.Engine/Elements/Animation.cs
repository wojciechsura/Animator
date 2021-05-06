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
    public abstract class Animation : Element
    {
        public abstract void ApplyAnimation(float timeMs);

        public abstract void ResetAnimation();

        public SceneElement AnimatedObject
        {
            get
            {
                if (Parent is Animation baseAnimation)
                    return baseAnimation.AnimatedObject;
                else if (Parent is SceneElement animatedObject)
                    return animatedObject;
                else
                    throw new InvalidOperationException("Cannot retrieve animated object!");
            }
        }
    }
}
