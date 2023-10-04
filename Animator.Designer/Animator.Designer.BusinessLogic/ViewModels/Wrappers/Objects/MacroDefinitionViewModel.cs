using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MacroDefinitionViewModel : ObjectViewModel
    {
        private readonly List<PropertyViewModel> properties = new();
        private readonly StringPropertyViewModel key;
        private readonly ReferencePropertyViewModel content;

        private IEnumerable<ObjectViewModel> GetChildren()
        {
            if (content.Value is ReferenceValueViewModel refValue)
                yield return refValue.Value;
        }

        private void HandleKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Key));
        }

        private void HandleContentChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(DisplayChildren));
        }

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        public MacroDefinitionViewModel(WrapperContext context)
            : base(context)
        {
            Name = "Macro";
            Namespace = context.EngineNamespace;

            key = new StringPropertyViewModel(this, context, context.EngineNamespace, "Key");
            key.PropertyChanged += HandleKeyChanged;
            properties.Add(key);

            var availableTypes = context.Namespaces
                .OfType<AssemblyNamespaceViewModel>()
                .SelectMany(n => n.GetAvailableTypesFor(typeof(Animator.Engine.Elements.Element)))
                .OrderBy(e => e.Name)                
                .ToList();

            content = new ReferencePropertyViewModel(this, context, context.DefaultNamespace, "Content", availableTypes);
            content.PropertyChanged += HandleContentChanged;
            properties.Add(content);

            DeleteCommand = new AppCommand(obj => DoDelete());

            Icon = "MacroDefinition16.png";
        }

        public override XmlElement Serialize(XmlDocument document)
        {
            XmlElement result;

            if (content.Value is DefaultValueViewModel)
            {
                result = document.CreateElement(nameof(Animator.Engine.Elements.Element));
            }
            else if (content.Value is ReferenceValueViewModel refValue)
            {
                result = refValue.Value.Serialize(document);
            }
            else
                throw new InvalidOperationException($"Unsupported macro definition content value: {content.Value}");

            var keyProp = CreateAttributeProp(document, "Key", Namespace);
            keyProp.Value = key.Value;
            
            result.Attributes.Append(keyProp);

            return result;
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public override IEnumerable<ObjectViewModel> DisplayChildren => GetChildren();

        public string Key => key.Value;

        public ICommand DeleteCommand { get; }
    }
}
