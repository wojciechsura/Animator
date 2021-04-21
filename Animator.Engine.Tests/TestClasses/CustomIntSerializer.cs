using Animator.Engine.Base;
using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class CustomIntSerializer : TypeSerializer
    {
        public override bool CanDeserialize(string value) => int.TryParse(value, out _);

        public override bool CanSerialize(object obj) => obj is int;

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
