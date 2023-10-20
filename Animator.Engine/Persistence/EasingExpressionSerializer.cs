using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Persistence
{
    public class EasingExpressionSerializer : TypeSerializer
    {
        public override bool CanDeserialize(string value)
        {
            return true;
        }

        public override bool CanSerialize(object obj)
        {
            return obj is EasingExpression;
        }

        public override object Deserialize(string str)
        {
            var result = new EasingExpression();
            result.Formula = str;
            return result;
        }

        public override string Serialize(object obj)
        {
            var expression = (EasingExpression)obj;
            return expression.Formula;
        }
    }
}
