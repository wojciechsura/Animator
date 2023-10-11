using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.Exceptions
{

    [Serializable]
    public class InfrastructureException : Exception
    {
        public InfrastructureException() { }
        public InfrastructureException(string message) : base(message) { }
        public InfrastructureException(string message, Exception inner) : base(message, inner) { }
        protected InfrastructureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
