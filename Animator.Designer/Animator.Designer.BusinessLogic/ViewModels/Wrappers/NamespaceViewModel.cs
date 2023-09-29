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
        public NamespaceViewModel(string empty, Assembly assembly, string @namespace)
        {
            Empty = empty;
            Assembly = assembly;
            Namespace = @namespace;
        }

        public string Prefix { get; }
        public string Namespace { get; }
        public Assembly Assembly { get; }
        public string Empty { get; }
    }
}
