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
        private readonly Type type;
        private string defaultNamespace;

        public MarkupExtensionViewModel(string defaultNamespace, string ns, string name, Type type)
        {
            this.type = type;
            this.Name = name;
            this.defaultNamespace = defaultNamespace;
            this.Namespace = ns;

            foreach (var propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(prop => prop.Name))
            {
                var property = new StringPropertyViewModel(defaultNamespace, propInfo.Name);
                properties.Add(property);
            }
        }
       
        public StringPropertyViewModel this[string ns, string name]
        {
            get => properties.Single(prop => prop.Namespace == ns && prop.Name == name);
        }

        public StringPropertyViewModel this[string name]
        {
            get => properties.Single(prop => prop.Namespace == defaultNamespace && prop.Name == name);
        }

        public string Name { get; }

        public string Namespace { get; }
    }
}
