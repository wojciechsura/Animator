using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    public class NontrivialCtorClass : ManagedObject
    {
        public NontrivialCtorClass(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
