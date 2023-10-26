using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all keyframe classes. A keyframe
    /// class describes fixed value for a property in 
    /// specific time.
    /// </summary>
    public abstract partial class Keyframe : StoryboardEntry
    {
        // Internal methods ---------------------------------------------------

        internal override void AddKeyframesRecursive(List<Keyframe> keyframes)
        {
            keyframes.Add(this);
        }

        // Public methods -----------------------------------------------------

        public abstract object GetValue();

        public abstract object EvalValue(float fromTimeMs, object fromValue, float currentTimeMs);
    }
}
