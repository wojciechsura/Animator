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

        public GenerateViewModel()
        {
            properties.Add(new MultilineStringPropertyViewModel("Generator"));
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;
    }
}
