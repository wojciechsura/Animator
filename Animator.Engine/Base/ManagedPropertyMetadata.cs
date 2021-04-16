using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public delegate object CoerceValueDelegate(ManagedObject obj, object baseValue);

    public class ManagedPropertyMetadata
    {
        // Public methods -----------------------------------------------------

        public ManagedPropertyMetadata(object defaultValue = null, CoerceValueDelegate coerceValueHandler = null)
        {
            DefaultValue = defaultValue;
            CoerceValueHandler = coerceValueHandler;
        }

        // Public static properties -------------------------------------------

        public static ManagedPropertyMetadata Default { get; } = new ManagedPropertyMetadata();

        // Public properties --------------------------------------------------

        public object DefaultValue { get; } = null;
        public CoerceValueDelegate CoerceValueHandler { get; }
    }
}
