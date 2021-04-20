using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public abstract class CustomCollectionSerializer
    {
        public abstract IList Deserialize(string data);
        public abstract string Serialize(IList list);
    }
}
