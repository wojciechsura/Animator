using Animator.Designer.BusinessLogic.Infrastructure;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public abstract class ObjectViewModel : BaseObjectViewModel, IParentedItem<ValueViewModel>
    {
        protected ObjectViewModel(WrapperContext context, string defaultNamespace, string engineNamespace) 
            : base(context, defaultNamespace, engineNamespace)
        {

        }

        public ValueViewModel Parent { get; set; }
    }
}
