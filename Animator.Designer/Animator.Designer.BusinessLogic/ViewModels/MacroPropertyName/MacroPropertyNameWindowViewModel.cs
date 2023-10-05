using Animator.Designer.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.MacroPropertyName
{
    public class MacroPropertyNameWindowViewModel : BaseViewModel
    {
        private readonly List<string> existingNames;
        private readonly IMacroPropertyNameWindowAccess access;
        private string name;

        private void DoOk()
        {
            access.Close(true);
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        private bool NameReused(string name)
            => existingNames.Contains(name);

        public MacroPropertyNameWindowViewModel(List<string> existingNames, IMacroPropertyNameWindowAccess access)
        {
            this.existingNames = existingNames;
            this.access = access;

            var nameNotDuplicatedCondition = Condition.ChainedLambda(this, vm => !vm.NameReused(vm.Name), false);

            OkCommand = new AppCommand(obj => DoOk(), nameNotDuplicatedCondition);
            CancelCommand = new AppCommand(obj => DoCancel());
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
    }
}
