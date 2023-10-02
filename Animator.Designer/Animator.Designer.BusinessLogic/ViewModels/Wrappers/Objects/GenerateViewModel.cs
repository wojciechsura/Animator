using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class GenerateViewModel : ObjectViewModel
    {
        private readonly List<ObjectViewModel> children = new();
        private readonly List<PropertyViewModel> properties = new();

        public GenerateViewModel(WrapperContext context, string ns)
            : base(context)
        {
            Namespace = ns;
            properties.Add(new MultilineStringPropertyViewModel(this, context, context.DefaultNamespace, "Generator"));

            Icon = "Generator16.png";
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Namespace { get; }

        public override IEnumerable<ObjectViewModel> DisplayChildren => children;
    }
}
