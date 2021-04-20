using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Exceptions
{
    [Serializable]
    public class SerializerException : Exception
    {
        protected SerializerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public SerializerException() { }
        public SerializerException(string message, string xPath) : base(message)
        {
            XPath = xPath;
        }
        public SerializerException(string message, string xPath, Exception inner) : base(message, inner)
        {
            XPath = xPath;
        }

        public string XPath { get; }
    }
}
