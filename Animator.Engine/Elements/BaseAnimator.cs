using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class BaseAnimator : BaseElement
    {
        public abstract void ApplyAnimation(float timeMs);

        public abstract void ResetAnimation();
    }
}
