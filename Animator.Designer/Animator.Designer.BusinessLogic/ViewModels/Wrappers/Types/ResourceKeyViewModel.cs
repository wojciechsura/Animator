using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types
{
    public class ResourceKeyViewModel
    {
        public ResourceKeyViewModel(string key, ICommand command)
        {
            Key = key;
            Command = command;
        }

        public string Key { get; }
        public ICommand Command { get; }
    }
}
