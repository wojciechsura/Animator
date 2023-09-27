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
        private static readonly List<PropertyViewModel> emptyProperties = new();

        public PropertyProxyViewModel(string defaultNamespace, string engineNamespace, IReadOnlyList<ManagedPropertyViewModel> properties) 
            : base(defaultNamespace, engineNamespace)
        {
            this.DisplayChildren = properties;
        }

        public override IReadOnlyList<PropertyViewModel> Properties => emptyProperties;

        public IReadOnlyList<ManagedPropertyViewModel> DisplayChildren { get; }
    }
}
