using Animator.Designer.BusinessLogic.Models.AddNamespace;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.Services.Dialogs
{
    public interface IDialogService
    {
        void ShowExceptionDialog(Exception e);
        (bool result, string name) ShowNewMacroPropertyDialog(List<string> existingNames);
        (bool result, string path) ShowOpenDialog(string filter = null, string title = null, string filename = null);
        (bool result, string path) ShowSaveDialog(string filter = null, string title = null, string filename = null);
        void ShowMacroPropertyEditor(MacroViewModel macro);
        (bool result, AddNamespaceResultModel model) ShowAddNamespaceDialog(WrapperContext context);
    }
}
