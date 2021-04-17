using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public delegate object CoerceValueDelegate(ManagedObject obj, object baseValue);

    public class ManagedSimplePropertyMetadata
    {
        // Public methods -----------------------------------------------------

        public ManagedSimplePropertyMetadata(object defaultValue = null, CoerceValueDelegate coerceValueHandler = null)
        {
            DefaultValue = defaultValue;
            CoerceValueHandler = coerceValueHandler;
        }

        // Public static properties -------------------------------------------

        public static ManagedSimplePropertyMetadata Default { get; } = new ManagedSimplePropertyMetadata();

        // Public properties --------------------------------------------------

        public object DefaultValue { get; } = null;
        public CoerceValueDelegate CoerceValueHandler { get; }
    }
}
