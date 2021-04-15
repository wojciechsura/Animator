using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    internal class ExpressionPropertyValue : BasePropertyValue
    {
        private readonly BaseExpression expression;

        protected override object ProvideValue()
        {
            return expression.GetValue();
        }

        public ExpressionPropertyValue(BaseExpression expression)
        {
            this.expression = expression;
        }
    }
}
