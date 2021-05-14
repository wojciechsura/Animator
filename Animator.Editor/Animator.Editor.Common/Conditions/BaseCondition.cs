using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.Common.Conditions
{
    public delegate void ValueChangedHandler(object sender, ValueChangedEventArgs e);

    public abstract class BaseCondition
    {
        protected virtual void OnValueChanged(bool newValue) => ValueChanged?.Invoke(this, new ValueChangedEventArgs(newValue));

        public static BaseCondition operator & (BaseCondition first, BaseCondition second) => 
            new CompositeCondition(CompositeCondition.CompositionKind.And, first, second);

        public static BaseCondition operator | (BaseCondition first, BaseCondition second) => 
            new CompositeCondition(CompositeCondition.CompositionKind.Or, first, second);

        public static BaseCondition operator !(BaseCondition condition) => 
            new NegateCondition(condition);

        public abstract bool GetValue();

        public event ValueChangedHandler ValueChanged;
    }
}
