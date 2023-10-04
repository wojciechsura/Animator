using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class GenerateViewModel : ObjectViewModel
    {
        private readonly List<ObjectViewModel> children = new();
        private readonly List<PropertyViewModel> properties = new();
        private readonly MultilineStringPropertyViewModel generator;

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        public GenerateViewModel(WrapperContext context)
            : base(context)
        {
            Name = "Generate";
            Namespace = context.EngineNamespace;

            generator = new MultilineStringPropertyViewModel(this, context, context.DefaultNamespace, "Generator");
            properties.Add(generator);

            DeleteCommand = new AppCommand(obj => DoDelete());

            Icon = "Generator16.png";
        }

        public override XmlElement Serialize(XmlDocument document)
        {
            var result = CreateRootElement(document);
            result.InnerXml = generator.Value;
            return result;
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public ICommand DeleteCommand { get; }

        public override IEnumerable<ObjectViewModel> DisplayChildren => children;
    }
}
