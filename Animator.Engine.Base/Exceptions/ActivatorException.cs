using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Exceptions
{

    [Serializable]
    public class ActivatorException : Exception
    {
        public ActivatorException() { }
        public ActivatorException(string message) : base(message) { }
        public ActivatorException(string message, Exception inner) : base(message, inner) { }
    }
}
