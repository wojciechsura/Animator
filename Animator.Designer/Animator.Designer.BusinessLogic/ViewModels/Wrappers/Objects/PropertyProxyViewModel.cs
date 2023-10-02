using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class PropertyProxyViewModel : VirtualObjectViewModel
    {
        private readonly List<PropertyViewModel> properties = new();

        public PropertyProxyViewModel(WrapperContext context, string defaultNamespace, string engineNamespace, string name, List<ObjectViewModel> children)
            : base(context, defaultNamespace, engineNamespace)
        {
            Name = name;
            DisplayChildren = children;

            Icon = "Property16.png";
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Name { get; }
        public override IEnumerable<ObjectViewModel> DisplayChildren { get; }
    }
}
