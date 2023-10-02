using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class ReferenceValueViewModel : ValueViewModel
    {
        private ObjectViewModel value;

        public ReferenceValueViewModel(ObjectViewModel value)
        {
            this.value = value;
            value.Parent = this;
        }

        public ObjectViewModel Value => value;
    }
}
