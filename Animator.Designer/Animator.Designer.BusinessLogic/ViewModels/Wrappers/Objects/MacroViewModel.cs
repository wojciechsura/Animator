using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MacroViewModel : ObjectViewModel
    {
        private readonly List<ObjectViewModel> children = new();
        private readonly ObservableCollection<PropertyViewModel> properties = new();
        /// <remarks>See <see cref="Animator.Engine.Base.ManagedProperty.nameRegex"/></remarks>
        private readonly Regex nameRegex = new Regex("^[a-zA-Z_][a-zA-Z_0-9]*$");

        private readonly StringPropertyViewModel keyProperty;

        private void HandleKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Key));
        }

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        public MacroViewModel(WrapperContext context) 
            : base(context)
        {
            Name = "Macro";
            Namespace = context.EngineNamespace;
            keyProperty = new StringPropertyViewModel(this, context, context.EngineNamespace, "Key");
            keyProperty.PropertyChanged += HandleKeyChanged;
            properties.Add(keyProperty);

            Icon = "PlaceMacro16.png";

            DeleteCommand = new AppCommand(obj => DoDelete());
        }

        public override XmlElement Serialize(XmlDocument document)
        {
            var result = CreateRootElement(document);

            var keyProp = CreateAttributeProp(document, "Key", Namespace);
            keyProp.Value = keyProperty.Value;
            result.Attributes.Append(keyProp);

            foreach (var property in properties)
            {
                if (property == keyProperty)
                    continue;

                if (property is StringPropertyViewModel stringProp)
                {
                    var propAttr = CreateAttributeProp(document, property.Name, property.Namespace);
                    propAttr.Value = stringProp.Value;

                    result.Attributes.Append(propAttr);
                }
                else
                    throw new InvalidOperationException("Unsupported property type for macro!");
            }

            return result;
        }

        public StringPropertyViewModel AddProperty(string propertyName)
        {
            if (properties.Any(prop => prop.Name == propertyName))
                throw new ArgumentException("Macro already contains property with given name!");

            if (!nameRegex.IsMatch(propertyName))
                throw new ArgumentException("Invalid property name!");

            var property = new StringPropertyViewModel(this, context, context.DefaultNamespace, propertyName);
            properties.Add(property);
            return property;
        }

        public void DeleteProperty(string propertyName)
        {
            var property = properties.FirstOrDefault(prop => prop.Name == propertyName && prop.Namespace == context.DefaultNamespace);
            if (property == null)
                throw new ArgumentException($"Macro doesn't contain property {propertyName}!");

            properties.Remove(property);
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;
        
        public override IEnumerable<ObjectViewModel> DisplayChildren => children;

        public ICommand DeleteCommand { get; }

        public string Key => keyProperty.Value;
    }
}
