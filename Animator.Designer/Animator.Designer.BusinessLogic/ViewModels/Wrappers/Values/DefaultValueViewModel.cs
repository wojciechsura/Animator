using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class DefaultValueViewModel : ValueViewModel
    {
        private object defaultValue;

        public DefaultValueViewModel(object defaultValue) 
        {
            this.defaultValue = defaultValue;
        }
    }
}
