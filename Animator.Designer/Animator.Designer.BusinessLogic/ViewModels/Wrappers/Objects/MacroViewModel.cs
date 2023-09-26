using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MacroViewModel : BaseObjectViewModel
    {
        private readonly ObservableCollection<PropertyViewModel> properties = new();
        /// <remarks>See <see cref="Animator.Engine.Base.ManagedProperty.nameRegex"/></remarks>
        private readonly Regex nameRegex = new Regex("^[a-zA-Z_][a-zA-Z_0-9]*$");

        public MacroViewModel()
        {
            properties.Add(new StringPropertyViewModel("x:Key"));
        }

        public StringPropertyViewModel AddProperty(string propertyName)
        {
            if (properties.Any(prop => prop.Name == propertyName))
                throw new ArgumentException("Macro already contains property with given name!");

            if (!nameRegex.IsMatch(propertyName))
                throw new ArgumentException("Invalid property name!");

            var property = new StringPropertyViewModel(propertyName);
            properties.Add(property);
            return property;
        }

        public void DeleteProperty(string propertyName)
        {
            var property = properties.FirstOrDefault(prop => prop.Name == propertyName);
            if (property == null)
                throw new ArgumentException($"Macro doesn't contain property {propertyName}!");

            properties.Remove(property);
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;
    }
}
