using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class IntFieldCustomSerializer : CustomPropertySerializer
    {
        public override object Deserialize(string data)
        {
            return -int.Parse(data);
        }

        public override string Serialize(object obj)
        {
            return (-(int)obj).ToString();
        }
    }
}
