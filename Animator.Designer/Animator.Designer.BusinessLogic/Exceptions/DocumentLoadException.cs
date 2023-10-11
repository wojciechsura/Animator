using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.Exceptions
{

    [Serializable]
    public class DocumentLoadException : InfrastructureException
    {
        public DocumentLoadException(string message, string xPath) : base(message)
        {
            XPath = xPath;
        }

        public DocumentLoadException(string message, string xPath, Exception inner) : base(message, inner)
        {
            XPath = xPath;
        }

        protected DocumentLoadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) 
        {
        
        }

        public string XPath { get; }
    }
}
