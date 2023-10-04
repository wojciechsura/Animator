using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MarkupExtensionViewModel : ObjectViewModel
    {
        private readonly List<ObjectViewModel> children = new();
        private readonly List<StringPropertyViewModel> properties = new();
        private readonly Type type;

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        public MarkupExtensionViewModel(WrapperContext context, string ns, string name, Type type)
            : base(context)
        {
            this.type = type;
            this.Name = name;
            this.Namespace = ns;

            foreach (var propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(prop => prop.Name))
            {
                var property = new StringPropertyViewModel(this, context, context.DefaultNamespace, propInfo.Name);
                properties.Add(property);
            }

            DeleteCommand = new AppCommand(obj => DoDelete());

            Icon = "MarkupExtension16.png";
        }

        public override IEnumerable<PropertyViewModel> Properties => properties;

        public override IEnumerable<ObjectViewModel> DisplayChildren => children;

        public ICommand DeleteCommand { get; }

        public string Name { get; }

        public string Namespace { get; }
    }
}
