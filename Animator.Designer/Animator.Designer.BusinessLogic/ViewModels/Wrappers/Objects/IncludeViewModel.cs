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
    public class IncludeViewModel : ObjectViewModel
    {
        private readonly List<ObjectViewModel> children = new();
        private readonly List<PropertyViewModel> properties = new();
        private readonly StringPropertyViewModel sourceProperty;

        private void HandleSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Source));
        }

        public IncludeViewModel(WrapperContext context, string ns) 
            : base(context)
        {
            Namespace = ns;
            sourceProperty = new StringPropertyViewModel(this, context, ns, "Source");
            sourceProperty.PropertyChanged += HandleSourceChanged;
            properties.Add(sourceProperty);

            Icon = "Include16.png";
        }

        public override IEnumerable<PropertyViewModel> Properties => properties;

        public override IEnumerable<ObjectViewModel> DisplayChildren => children;

        public string Source => sourceProperty.Value;

        public string Namespace { get; }        
    }
}
