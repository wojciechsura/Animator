using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types
{
    public record TypeViewModel(Type Type, ICommand Command)
    {
        public string Name => Type.Name;
    }
}
