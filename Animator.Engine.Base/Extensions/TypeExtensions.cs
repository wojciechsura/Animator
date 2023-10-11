using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Extensions
{
    public static class TypeExtensions
    {
        private static readonly HashSet<Type> staticallyInitializedTypes = new();

        public static NamespaceDefinition ToNamespaceDefinition(this Type type)
        {
            var assembly = type.Assembly.GetName().Name;
            var ns = type.Namespace;

            return new NamespaceDefinition(assembly, ns);
        }

        public static void StaticInitializeRecursively(this Type type)
        {
            do
            {
                // If type is initialized, its base types must have been initialized too,
                // don't waste time on them
                if (staticallyInitializedTypes.Contains(type))
                    return;

                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

                staticallyInitializedTypes.Add(type);

                type = type.BaseType;
            }
            while (type != typeof(ManagedObject) && type != typeof(object));
        }
    }
}
