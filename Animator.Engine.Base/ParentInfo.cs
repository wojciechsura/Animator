using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public record class ParentInfo(ManagedObject Parent, ManagedProperty Property);
}
