using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Persistence
{
    public class NamespaceDefinition
    {
        public NamespaceDefinition(string assembly, string @namespace)
        {
            Assembly = assembly;
            Namespace = @namespace;
        }

        public string Assembly { get; }
        public string Namespace { get; }
    }
}
