using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Animator.Editor.BusinessLogic.Models.Navigation
{
    public class CommandNavigationModel : BaseNavigationModel
    {
        public CommandNavigationModel(string title, ImageSource icon, bool enabled, ICommand command)
            : base(title, icon, enabled)
        {
            Command = command;
        }

        public ICommand Command { get; }
    }
}
