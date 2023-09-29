using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MarkupExtensionViewModel : BaseObjectViewModel
    {
        private readonly List<BaseObjectViewModel> children = new();
        private readonly List<StringPropertyViewModel> properties = new();
        private readonly Type type;

        public MarkupExtensionViewModel(WrapperContext context, string defaultNamespace, string engineNamespace, string ns, string name, Type type)
            : base(context, defaultNamespace, engineNamespace)
        {
            this.type = type;
            this.Name = name;
            this.Namespace = ns;

            foreach (var propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(prop => prop.Name))
            {
                var property = new StringPropertyViewModel(context, defaultNamespace, propInfo.Name);
                properties.Add(property);
            }

            Icon = "MarkupExtension16.png";
        }

        public override IEnumerable<PropertyViewModel> Properties => properties;

        public override IEnumerable<BaseObjectViewModel> DisplayChildren => children;

        public string Name { get; }

        public string Namespace { get; }
    }
}
