using Animator.Editor.BusinessLogic.Models.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Animator.Editor.BusinessLogic.Services.Commands
{
    public interface ICommandRepositoryService
    {
        ICommand RegisterCommand(string title, string iconResource, Action<object> action, BaseCondition condition = null);
        List<CommandModel> FindMatching(string text);
    }
}
