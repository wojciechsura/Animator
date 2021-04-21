using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Persistence.Types
{
    public abstract class TypeSerializer
    {
        public abstract object Deserialize(string str);
        public abstract string Serialize(object obj);
        public abstract bool CanDeserialize(string value);
    }
}
