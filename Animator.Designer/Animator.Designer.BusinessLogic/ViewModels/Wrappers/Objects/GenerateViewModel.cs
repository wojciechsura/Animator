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
        private readonly ObservableCollection<PropertyViewModel> properties = new();

        public GenerateViewModel(string defaultNamespace, string engineNamespace, string ns)
            : base(defaultNamespace, engineNamespace)
        {
            Namespace = ns;
            properties.Add(new MultilineStringPropertyViewModel(null, "Generator"));
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Namespace { get; }
    }
}
