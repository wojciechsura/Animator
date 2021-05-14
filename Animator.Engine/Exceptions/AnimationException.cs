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
        
        public AnimationException(string message, string path) : base(message)
        {
            Path = path;
        }
        
        public AnimationException(string message, string path, Exception inner) : base(message, inner)
        {
            Path = path;
        }
        
        protected AnimationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public string Path { get; }
    }
}
