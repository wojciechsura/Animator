using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedPropertyMetadata
    {
        // Public methods -----------------------------------------------------

        public ManagedPropertyMetadata(object defaultValue = null)
        {
            DefaultValue = defaultValue;
        }

        // Public static properties -------------------------------------------

        public static ManagedPropertyMetadata Default { get; } = new ManagedPropertyMetadata();

        // Public properties --------------------------------------------------

        public object DefaultValue { get; } = null;
        public bool Inherited { get; init; } = false;
    }
}
