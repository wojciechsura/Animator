using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class MarkupExtensionViewModel : ValueViewModel
    {
        private readonly List<StringPropertyViewModel> properties = new();

        private readonly string fullClassName;
        private readonly Type type;

        public MarkupExtensionViewModel(Type type, string fullClassName)
        {
            this.type = type;
            this.fullClassName = fullClassName;

            foreach (var propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var property = new StringPropertyViewModel(propInfo.Name);
                properties.Add(property);
            }
        }

        public StringPropertyViewModel this[string name]
        {
            get
            {
                return properties.Single(prop => prop.Name == name);
            }
        }
    }
}
