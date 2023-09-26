using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MacroEntryViewModel : BaseObjectViewModel
    {
        private readonly List<PropertyViewModel> properties = new();
        private BaseObjectViewModel content;

        public MacroEntryViewModel()
        {
            properties.Add(new StringPropertyViewModel("x:Key"));
        }

        public BaseObjectViewModel Content
        {
            get => content;
            set => Set(ref content, value);
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;
    }
}
