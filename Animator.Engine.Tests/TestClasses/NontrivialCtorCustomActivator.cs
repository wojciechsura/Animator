using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class NontrivialCtorCustomActivator : CustomActivator
    {
        public override object CreateInstance(Type type)
        {
            if (type == typeof(NontrivialCtorClass))
                return new NontrivialCtorClass(42);
            else
                return Activator.CreateInstance(type);
        }
    }
}
