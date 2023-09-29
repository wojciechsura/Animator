using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class GenerateViewModel : BaseObjectViewModel
    {
        private readonly List<BaseObjectViewModel> children = new();
        private readonly List<PropertyViewModel> properties = new();

        public GenerateViewModel(WrapperContext context, string defaultNamespace, string engineNamespace, string ns)
            : base(context, defaultNamespace, engineNamespace)
        {
            Namespace = ns;
            properties.Add(new MultilineStringPropertyViewModel(context, defaultNamespace, "Generator"));

            Icon = "Generator16.png";
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Namespace { get; }

        public override IEnumerable<BaseObjectViewModel> DisplayChildren => children;
    }
}
