using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.Common.Conditions
{
    public class CompositeCondition : BaseCondition
    {
        // Private fields ------------------------------------------------------

        private readonly CompositionKind compositionKind;
        private readonly List<BaseCondition> conditions;
        private bool value;

        // Private methods -----------------------------------------------------

        private void HandleValueChanged(object sender, ValueChangedEventArgs e)
        {
            ReevaluateValue();
        }

        private bool InternalEvalValue()
        {
            if (conditions.Count == 0)
                return false;
            else
            {
                switch (compositionKind)
                {
                    case CompositionKind.And:
                        {
                            bool result = true;

                            int i = 0;
                            while (i < conditions.Count && result)
                                result &= conditions[i++].GetValue();

                            return result;
                        }
                    case CompositionKind.Or:
                        {
                            bool result = false;

                            int i = 0;
                            while (i < conditions.Count && !result)
                                result |= conditions[i++].GetValue();

                            return result;
                        }
                    default:
                        throw new InvalidEnumArgumentException("Unsupported composition kind!");
                }
            }
        }

        private void ReevaluateValue()
        {
            bool newValue = InternalEvalValue();

            if (newValue != value)
            {
                value = newValue;
                OnValueChanged(value);
            }
        }

        // Public types --------------------------------------------------------

        public enum CompositionKind
        {
            And,
            Or
        }

        // Public methods ------------------------------------------------------

        public CompositeCondition(CompositionKind kind = CompositionKind.And)
        {
            compositionKind = kind;
            conditions = new List<BaseCondition>();
            value = false;
        }

        public CompositeCondition(CompositionKind kind,
            params BaseCondition[] compositeConditions)
        {
            compositionKind = kind;
            conditions = new List<BaseCondition>();
            value = false;

            foreach (BaseCondition condition in compositeConditions)
            {
                Add(condition);
            }
        }

        public void Add(BaseCondition condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");

            if (!conditions.Contains(condition))
            {
                condition.ValueChanged += HandleValueChanged;
                conditions.Add(condition);

                ReevaluateValue();
            }
        }

        public void Remove(BaseCondition condition)
        {
            if (conditions.Contains(condition))
            {
                condition.ValueChanged -= HandleValueChanged;
                conditions.Remove(condition);

                ReevaluateValue();
            }
        }

        public override bool GetValue()
        {
            return value;
        }
    }
}
