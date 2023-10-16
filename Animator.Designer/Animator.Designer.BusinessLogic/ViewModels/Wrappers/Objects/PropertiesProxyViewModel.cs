using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class PropertiesProxyViewModel : VirtualObjectViewModel
    {        
        private static readonly List<PropertyViewModel> emptyProperties = new();
        private IReadOnlyList<BaseObjectViewModel> children;

        public PropertiesProxyViewModel(ManagedObjectViewModel visualParent, WrapperContext context) 
            : base(visualParent, context)
        {
            Icon = "Properties16.png";
        }

        public void SetChildren(IReadOnlyList<BaseObjectViewModel> children)
        {
            this.children = children;
            OnPropertyChanged(nameof(DisplayChildren));
        }

        public override IReadOnlyList<PropertyViewModel> Properties => emptyProperties;

        public override IEnumerable<BaseObjectViewModel> DisplayChildren => children;
    }
}
