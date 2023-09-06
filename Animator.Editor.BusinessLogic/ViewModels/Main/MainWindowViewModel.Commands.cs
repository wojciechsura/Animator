using Animator.Editor.BusinessLogic.ViewModels.Document;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly SimpleCondition documentExistsCondition;
        private readonly BaseCondition canUndoCondition;
        private readonly BaseCondition canRedoCondition;
        private readonly BaseCondition selectionAvailableCondition;
        private readonly BaseCondition regularSelectionAvailableCondition;
        private readonly BaseCondition searchPerformedCondition;

        // Public properties --------------------------------------------------

        // File

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand SaveFrameAsCommand { get; }

        // Edit

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand PasteCommand { get; }

        // Search

        public ICommand SearchCommand { get; }
        public ICommand ReplaceCommand { get; }
        public ICommand FindNextCommand { get; }

        // Navigation

        public ICommand NavigateCommand { get; }
    }
}
