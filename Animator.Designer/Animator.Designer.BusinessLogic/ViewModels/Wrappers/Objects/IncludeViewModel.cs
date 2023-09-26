using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class IncludeViewModel : BaseObjectViewModel
    {
        private readonly ObservableCollection<PropertyViewModel> properties = new();

        public IncludeViewModel() 
        {
            properties.Add(new StringPropertyViewModel("Source"));
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;
    }
}
