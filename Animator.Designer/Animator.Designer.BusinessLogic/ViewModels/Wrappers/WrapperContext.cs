using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class WrapperContext
    {
        public WrapperContext()
        {
            Namespaces = new();
        }

        public List<NamespaceViewModel> Namespaces { get; }
    }
}
