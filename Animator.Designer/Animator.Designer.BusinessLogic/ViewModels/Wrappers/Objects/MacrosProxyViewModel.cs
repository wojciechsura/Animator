using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MacrosProxyViewModel : VirtualObjectViewModel
    {
        private readonly List<PropertyViewModel> properties = new List<PropertyViewModel>();
        private readonly MacroCollectionPropertyViewModel macros;
        private readonly ManagedObjectViewModel parent;

        private IEnumerable<BaseObjectViewModel> GetDisplayChildren()
        {
            foreach (var macro in macros.Value.Items)
                yield return macro;
        }

        private void HandleMacrosChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(DisplayChildren));
        }

        public MacrosProxyViewModel(WrapperContext context, ManagedObjectViewModel parent, MacroCollectionPropertyViewModel macros) 
            : base(context)
        {
            this.parent = parent;
            this.macros = macros;
            macros.CollectionChanged += HandleMacrosChanged;

            Icon = "MacroDefinitions16.png";
        }

        public override IEnumerable<PropertyViewModel> Properties => properties;

        public override IEnumerable<BaseObjectViewModel> DisplayChildren => GetDisplayChildren();

        public ICommand AddMacroDefinitionCommand => macros.AddMacroDefinitionCommand;
    }
}
