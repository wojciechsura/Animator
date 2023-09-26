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
        private ManagedObjectViewModel value;

        public ReferenceValueViewModel()
        {
            
        }

        public ManagedObjectViewModel Value
        {
            get => value;
            set => Set(ref this.value, value);
        }
    }
}
