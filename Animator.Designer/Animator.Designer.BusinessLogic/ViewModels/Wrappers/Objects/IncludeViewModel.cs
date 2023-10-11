using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class IncludeViewModel : ObjectViewModel
    {
        private readonly List<ObjectViewModel> children = new();
        private readonly List<PropertyViewModel> properties = new();
        private readonly StringPropertyViewModel sourceProperty;

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        private void HandleSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Source));
        }

        public IncludeViewModel(WrapperContext context) 
            : base(context)
        {
            Name = "Include";
            Namespace = context.EngineNamespace;
            sourceProperty = new StringPropertyViewModel(this, context, context.EngineNamespace, "Source");
            sourceProperty.PropertyChanged += HandleSourceChanged;
            properties.Add(sourceProperty);

            DeleteCommand = new AppCommand(obj => DoDelete());

            Icon = "Include16.png";
        }

        public override XmlElement Serialize(XmlDocument document)
        {
            var result = CreateRootElement(document);

            var sourceProp = CreateAttributeProp(document, "Source", Namespace);
            sourceProp.Value = sourceProperty.Value;
            result.Attributes.Append(sourceProp);

            return result;
        }

        public override IEnumerable<PropertyViewModel> Properties => properties;

        public override IEnumerable<ObjectViewModel> DisplayChildren => children;

        public ICommand DeleteCommand { get; }

        public string Source => sourceProperty.Value;      
    }
}
