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

        public PropertiesProxyViewModel(WrapperContext context, IEnumerable<PropertyProxyViewModel> properties) 
            : base(context)
        {
            this.DisplayChildren = properties;

            Icon = "Properties16.png";
        }

        public override IReadOnlyList<PropertyViewModel> Properties => emptyProperties;

        public override IEnumerable<BaseObjectViewModel> DisplayChildren { get; }
    }
}
