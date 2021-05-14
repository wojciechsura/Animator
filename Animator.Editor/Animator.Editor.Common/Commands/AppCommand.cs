using Animator.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Editor.Common.Commands
{
    public class AppCommand : ICommand
    {
        private readonly BaseCondition condition;
        private readonly Action<object> action;

        private void HandleConditionValueChanged(object sender, ValueChangedEventArgs e)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public AppCommand(Action<object> action, BaseCondition condition = null)
        {
            this.action = action;
            this.condition = condition;
            if (condition != null)
                condition.ValueChanged += HandleConditionValueChanged;
        }

        public bool CanExecute(object parameter)
        {
            return condition?.GetValue() ?? true;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
