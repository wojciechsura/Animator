using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class WrapperContext
    {
        private List<NamespaceViewModel> namespaces = new List<NamespaceViewModel>();

        public WrapperContext()
        {
            namespaces = new();
        }

        public void AddNamespace(NamespaceViewModel ns)
            => namespaces.Add(ns);

        public IReadOnlyList<NamespaceViewModel> Namespaces => namespaces;
    }
}
