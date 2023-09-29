using Animator.Designer.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public abstract class ValueViewModel : BaseViewModel
    {       
        public IValueHandler Handler { get; set; }
    }
}
