using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class GenerateViewModel : ObjectViewModel
    {
        private readonly List<ObjectViewModel> children = new();
        private readonly List<PropertyViewModel> properties = new();

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        public GenerateViewModel(WrapperContext context, string ns)
            : base(context)
        {
            Namespace = ns;
            properties.Add(new MultilineStringPropertyViewModel(this, context, context.DefaultNamespace, "Generator"));

            DeleteCommand = new AppCommand(obj => DoDelete());

            Icon = "Generator16.png";
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Namespace { get; }

        public ICommand DeleteCommand { get; }

        public override IEnumerable<ObjectViewModel> DisplayChildren => children;
    }
}
