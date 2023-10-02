using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public abstract class VirtualObjectViewModel : BaseObjectViewModel
    {
        protected VirtualObjectViewModel(WrapperContext context, string defaultNamespace, string engineNamespace) 
            : base(context, defaultNamespace, engineNamespace)
        {

        }
    }
}
