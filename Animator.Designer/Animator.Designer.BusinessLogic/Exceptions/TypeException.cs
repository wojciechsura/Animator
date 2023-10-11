using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.Exceptions
{
    public class TypeException : InfrastructureException
    {
        public TypeException(string message) : base(message) 
        {
        
        }

        public TypeException(string message, Exception innerException) : base(message, innerException) 
        {
        
        }
    }
}
