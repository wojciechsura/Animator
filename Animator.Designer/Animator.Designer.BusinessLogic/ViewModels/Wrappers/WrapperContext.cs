using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class WrapperContext
    {
        private readonly List<NamespaceViewModel> namespaces = new();

        public WrapperContext(string engineNamespace, string defaultNamespace)
        {
            namespaces = new();
            EngineNamespace = engineNamespace;
            DefaultNamespace = defaultNamespace;
        }

        public void AddNamespace(NamespaceViewModel ns)
            => namespaces.Add(ns);

        public IReadOnlyList<NamespaceViewModel> Namespaces => namespaces;

        public string DefaultNamespace { get; }
        public string EngineNamespace { get; }
    }
}
