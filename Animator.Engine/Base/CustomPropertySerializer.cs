using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public abstract class CustomPropertySerializer
    {
        public abstract object Deserialize(string data);
        public abstract bool CanSerialize(object obj);
        public abstract string Serialize(object obj);
    }
}
