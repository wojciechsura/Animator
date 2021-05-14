using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.Common.Conditions
{
    public class SwitchCondition<T>
            where T : struct
    {
        private T current;
        private readonly Dictionary<T, Condition> conditions;

        public SwitchCondition()
        {
            conditions = new Dictionary<T, Condition>();
            current = default(T);
        }

        public SwitchCondition(params T[] newValues)
        {
            conditions = new Dictionary<T, Condition>();
            current = default(T);

            foreach (var value in newValues)
                Add(value);
        }

        public void Add(T value)
        {
            if (conditions.ContainsKey(value))
                throw new ArgumentException("value");

            var condition = new Condition(current.Equals(value));
            conditions.Add(value, condition);

            if (conditions.Count == 1)
            {
                current = value;
                condition.Value = true;
            }
        }

        public IEnumerable<T> Values
        {
            get
            {
                return conditions.Keys
                    .ToList();
            }
        }

        public T Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value;

                foreach (var pair in conditions)
                {
                    pair.Value.Value = pair.Key.Equals(value);
                }
            }
        }

        public Condition this[T value]
        {
            get
            {
                if (conditions.ContainsKey(value))
                    return conditions[value];
                else
                    return null;
            }
        }
    }
}
