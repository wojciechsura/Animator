using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Persistence
{
    public abstract class CustomActivator
    {
        public abstract object CreateInstance(Type type);
    }
}
