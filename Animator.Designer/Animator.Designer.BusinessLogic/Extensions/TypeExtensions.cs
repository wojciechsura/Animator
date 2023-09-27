using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.Helpers
{
    public static class TypeExtensions
    {
        public static NamespaceDefinition ToNamespaceDefinition(this Type type)
        {
            var assembly = type.Assembly.GetName().Name;
            var ns = type.Namespace;

            return new NamespaceDefinition(assembly, ns);
        }
    }
}
