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

        public IncludeViewModel(string defaultNamespace, string engineNamespace, string ns) 
            : base(defaultNamespace, engineNamespace)
        {
            Namespace = ns;
            properties.Add(new StringPropertyViewModel(ns, "Source"));
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Namespace { get; }
    }
}
