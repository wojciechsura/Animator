using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types
{
    public class TypeViewModel
    {
        public TypeViewModel(Type type, ICommand command)
        {
            Type = type;
            Command = command;
        }

        public string Name => Type.Name;

        public Type Type { get; }

        public ICommand Command { get; }
    }
}
