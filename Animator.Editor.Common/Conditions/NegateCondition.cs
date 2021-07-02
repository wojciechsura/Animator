using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.Common.Conditions
{
    public class NegateCondition : BaseCondition
    {
        // Private fields ----------------------------------------------------

        private readonly BaseCondition condition;

        // Private methods ---------------------------------------------------

        private void HandleInnerConditionValueChanged(object sender, ValueChangedEventArgs e) => OnValueChanged(!(e.Value));

        // Public methods ----------------------------------------------------

        public NegateCondition(BaseCondition newCondition)
        {
            condition = newCondition ?? throw new ArgumentNullException("newCondition");
            condition.ValueChanged += HandleInnerConditionValueChanged;
        }

        public override bool GetValue() => !condition.GetValue();
    }
}
