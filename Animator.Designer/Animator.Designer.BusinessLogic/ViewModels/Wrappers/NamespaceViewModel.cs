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
        public NamespaceViewModel(string prefix, string namespaceUri)
        {
            Prefix = prefix;
            NamespaceUri = namespaceUri;            
        }

        public string Prefix { get; }
        public string NamespaceUri { get; }
    }
}
