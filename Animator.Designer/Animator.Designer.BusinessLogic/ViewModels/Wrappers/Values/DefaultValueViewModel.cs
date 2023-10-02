using Animator.Engine.Base.Persistence.Types;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class DefaultValueViewModel : ValueViewModel
    {
        private void DoSwitchToStringIfPossible()
        {
            Handler.RequestSwitchToString();
        }

        public DefaultValueViewModel(object defaultValue, bool canSwitchToString) 
        {
            if (defaultValue == null)
                Value = null;
            else if (TypeSerialization.CanSerialize(defaultValue, defaultValue.GetType()))
                Value = TypeSerialization.Serialize(defaultValue);
            else
                Value = defaultValue.ToString();

            SwitchToStringIfPossibleCommand = new AppCommand(obj => DoSwitchToStringIfPossible(), Condition.Simple(canSwitchToString));
        }

        public string Value { get; }

        public ICommand SwitchToStringIfPossibleCommand { get; }
    }
}
