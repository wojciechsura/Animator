using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class PropertyProxyViewModel : BaseObjectViewModel
    {
        private readonly List<PropertyViewModel> properties = new();

        public PropertyProxyViewModel(string defaultNamespace, string engineNamespace, string name, List<BaseObjectViewModel> children)
            : base(defaultNamespace, engineNamespace)
        {
            Name = name;
            DisplayChildren = children;

            Icon = "Property16.png";
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Name { get; }
        public override IEnumerable<BaseObjectViewModel> DisplayChildren { get; }
    }
}
