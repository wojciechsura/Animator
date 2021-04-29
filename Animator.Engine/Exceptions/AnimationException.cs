using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Exceptions
{

    [Serializable]
    public class AnimationException : Exception
    {
        public AnimationException() { }
        public AnimationException(string message) : base(message) { }
        public AnimationException(string message, Exception inner) : base(message, inner) { }
        protected AnimationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
