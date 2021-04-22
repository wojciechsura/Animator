using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public delegate object CoerceValueDelegate(ManagedObject obj, object baseValue);

    public class ManagedSimplePropertyMetadata : BaseValuePropertyMetadata
    {
        // Private static fields ----------------------------------------------

        private static readonly Dictionary<Type, ManagedSimplePropertyMetadata> defaultMetadata = new();

        // Public static methods ----------------------------------------------

        public static ManagedSimplePropertyMetadata DefaultFor(Type type)
        {
            if (!defaultMetadata.TryGetValue(type, out ManagedSimplePropertyMetadata metadata))
            {
                object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                metadata = new ManagedSimplePropertyMetadata(defaultValue);

                defaultMetadata[type] = metadata;
            }

            return metadata;
        }

        // Public methods -----------------------------------------------------

        public ManagedSimplePropertyMetadata(object defaultValue = null,
            PropertyValueChangedDelegate valueChangedHandler = null,
            CoerceValueDelegate coerceValueHandler = null,
            TypeSerializer customSerializer = null)
            : base(valueChangedHandler, customSerializer)
        {
            DefaultValue = defaultValue;
            CoerceValueHandler = coerceValueHandler;
        }

        // Public properties --------------------------------------------------

        public object DefaultValue { get; }
        public CoerceValueDelegate CoerceValueHandler { get; }
        public bool NotAnimatable { get; init; } = false;

    }
}
