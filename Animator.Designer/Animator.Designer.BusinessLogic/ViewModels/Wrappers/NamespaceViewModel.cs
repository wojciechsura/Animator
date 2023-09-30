using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class NamespaceViewModel
    {
        private readonly Dictionary<Type, HashSet<TypeViewModel>> availableTypes = new();

        public NamespaceViewModel(string empty, Assembly assembly, string @namespace)
        {
            Empty = empty;
            Assembly = assembly;
            Namespace = @namespace;

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

                        types.Add(TypeViewModel.For(type));

                        current = current.BaseType;
                    }
                }
            }
        }

        public string Prefix { get; }
        public string Namespace { get; }
        public Assembly Assembly { get; }
        public string Empty { get; }

        public IEnumerable<TypeViewModel> GetAvailableTypesFor(Type type)
        {
            if (availableTypes.TryGetValue(type, out var types))
                return types;

            return Enumerable.Empty<TypeViewModel>();
        }
    }
}
