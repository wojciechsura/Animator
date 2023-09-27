using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class IncludeViewModel : BaseObjectViewModel
    {
        private readonly ObservableCollection<PropertyViewModel> properties = new();
        private readonly StringPropertyViewModel sourceProperty;

        private void HandleSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Source));
        }

        public IncludeViewModel(string defaultNamespace, string engineNamespace, string ns) 
            : base(defaultNamespace, engineNamespace)
        {
            Namespace = ns;
            sourceProperty = new StringPropertyViewModel(ns, "Source");
            sourceProperty.PropertyChanged += HandleSourceChanged;
            properties.Add(sourceProperty);
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Source => sourceProperty.Value;

        public string Namespace { get; }        
    }
}
