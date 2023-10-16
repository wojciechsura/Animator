using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class AssemblyNamespaceViewModel : NamespaceViewModel
    {
        private readonly Dictionary<Type, HashSet<Type>> availableTypes;

        private AssemblyNamespaceViewModel(string prefix, string namespaceUri, Assembly assembly, string @namespace, Dictionary<Type, HashSet<Type>> availableTypes)
            : base(prefix, namespaceUri)
        {
            Assembly = assembly;
            Namespace = @namespace;
            this.availableTypes = availableTypes;
        }

        public AssemblyNamespaceViewModel(string prefix, string namespaceUri, Assembly assembly, string @namespace)
            : base(prefix, namespaceUri)
        {
            Assembly = assembly;
            Namespace = @namespace;
            availableTypes = new();

            foreach (var type in Assembly.GetTypes()
                .Where(t => !t.IsAbstract))
            {
                if (type.Namespace == @namespace)
                {
                    var current = type;

                    while (current != typeof(object))
                    {
                        if (!availableTypes.TryGetValue(current, out var types))
                        {
                            types = new();
                            availableTypes.Add(current, types);
                        }

                        types.Add(type);

                        current = current.BaseType;
                    }
                }
            }
        }

        public override NamespaceViewModel CloneWithPrefix(string newPrefix)
        {
            return new AssemblyNamespaceViewModel(newPrefix, NamespaceUri, Assembly, Namespace, availableTypes);
        }

        public IEnumerable<Type> GetAvailableTypesFor(Type type)
        {
            if (availableTypes.TryGetValue(type, out var types))
                return types;

            return Enumerable.Empty<Type>();
        }

        public Assembly Assembly { get; }
        public string Namespace { get; }
    }
}
