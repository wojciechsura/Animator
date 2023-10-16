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
        private readonly BaseObjectViewModel visualParent;

        protected VirtualObjectViewModel(BaseObjectViewModel visualParent, WrapperContext context) 
            : base(context)
        {
            this.visualParent = visualParent;
        }

        public override BaseObjectViewModel VisualParent => visualParent;
    }
}
