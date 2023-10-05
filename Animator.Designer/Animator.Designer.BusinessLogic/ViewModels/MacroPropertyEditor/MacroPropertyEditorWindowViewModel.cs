using Animator.Designer.BusinessLogic.Services.Dialogs;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.MacroPropertyEditor
{
    public class MacroPropertyEditorWindowViewModel : BaseViewModel
    {
        private readonly MacroViewModel editedMacro;
        private readonly IMacroPropertyEditorWindowAccess access;
        private readonly ObservableCollection<StringPropertyViewModel> macroProperties = new();
        private readonly IDialogService dialogService;
        private StringPropertyViewModel selectedProperty;

        private void DoAddProperty()
        {
            var existingNames = macroProperties.Select(prop => prop.Name).ToList();

            (bool result, string name) = dialogService.ShowNewMacroPropertyDialog(existingNames);

            if (result)
            {
                var property = editedMacro.AddProperty(name);
                macroProperties.Add(property);
            }
        }

        private void DoDeleteProperty()
        {
            editedMacro.DeleteProperty(SelectedProperty.Name);
            macroProperties.Remove(SelectedProperty);
        }

        public MacroPropertyEditorWindowViewModel(MacroViewModel editedMacro, IMacroPropertyEditorWindowAccess access, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            this.editedMacro = editedMacro;
            this.access = access;

            foreach (var property in editedMacro.AdditionalProperties.OfType<StringPropertyViewModel>())
                macroProperties.Add(property);

            var selectedPropertyNotNullCondition = Condition.Lambda(this, vm => vm.SelectedProperty != null, false);

            AddPropertyCommand = new AppCommand(obj => DoAddProperty());
            DeletePropertyCommand = new AppCommand(obj => DoDeleteProperty(), selectedPropertyNotNullCondition);
        }

        public ObservableCollection<StringPropertyViewModel> MacroProperties => macroProperties;

        public StringPropertyViewModel SelectedProperty
        {
            get => selectedProperty;
            set => Set(ref selectedProperty, value);
        }

        public ICommand AddPropertyCommand { get; }
        public ICommand DeletePropertyCommand { get; }
        public ICommand CloseCommand { get; }
    }
}
