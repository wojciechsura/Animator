using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public abstract class BaseObjectViewModel
    {
        public TProperty Property<TProperty>(string name)
            where TProperty : PropertyViewModel =>        
            Properties
                .OfType<TProperty>()
                .SingleOrDefault(prop => prop.Name == name);
        
        public abstract IReadOnlyList<PropertyViewModel> Properties { get; }

        public PropertyViewModel this[string name]
        {
            get => Properties.SingleOrDefault(prop => prop.Name == name);
        }
    }
}
