using Animator.Editor.BusinessLogic.Models.Commands;
using Spooksoft.VisualStateManager.Commands;
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
    public class CommandRepositoryService : ICommandRepositoryService
    {
        // Private fields -----------------------------------------------------

        private readonly List<CommandModel> commands = new List<CommandModel>();

        // Private methods ----------------------------------------------------

        private bool IncrementalMatch(string target, string text)
        {
            int i = 0, j = 0;

            var lowerTarget = target.ToLowerInvariant();
            var lowerText = text.ToLowerInvariant();

            while (i < lowerTarget.Length && j < lowerText.Length)
            {
                if (lowerTarget[i] == lowerText[j])
                    j++;

                i++;
            }

            return j == lowerText.Length;
        }

        // Public methods -----------------------------------------------------

        public CommandRepositoryService()
        {

        }

        public ICommand RegisterCommand(string title, string iconResource, Action<object> action, BaseCondition condition = null)
        {
            var command = new AppCommand(action, condition);
            commands.Add(new CommandModel(title, iconResource, command));
            return command;
        }

        public List<CommandModel> FindMatching(string text)
        {
            return commands
                .Where(c => IncrementalMatch(c.Title, text))
                .ToList();            
        }
    }
}
