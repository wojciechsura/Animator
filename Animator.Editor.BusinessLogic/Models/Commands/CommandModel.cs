using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Animator.Editor.BusinessLogic.Models.Commands
{
    public class CommandModel
    {
        public CommandModel(string title, string iconResource, ICommand command)
        {
            Title = title;
            IconResource = iconResource;
            Command = command;
        }

        public string Title { get; }
        public string IconResource { get; }
        public ICommand Command { get; }
    }
}
