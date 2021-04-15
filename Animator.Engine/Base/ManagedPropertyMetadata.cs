using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedPropertyMetadata
    {
        public static ManagedPropertyMetadata Default { get; } = new ManagedPropertyMetadata();

        public bool Inherited { get; init; } = false;
        public object DefaultValue { get; init; } = null;
    }
}
