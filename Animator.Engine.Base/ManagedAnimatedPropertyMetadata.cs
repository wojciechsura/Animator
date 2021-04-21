using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public delegate object CoerceValueDelegate(ManagedObject obj, object baseValue);

    public class ManagedAnimatedPropertyMetadata : BasePropertyMetadata
    {
        // Private static fields ----------------------------------------------

        private static readonly Dictionary<Type, ManagedAnimatedPropertyMetadata> defaultMetadata = new();

        // Public static methods ----------------------------------------------

        public static ManagedAnimatedPropertyMetadata DefaultFor(Type type)
        {
            if (!defaultMetadata.TryGetValue(type, out ManagedAnimatedPropertyMetadata metadata))
            {
                object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                metadata = new ManagedAnimatedPropertyMetadata(defaultValue);

                defaultMetadata[type] = metadata;
            }

            return metadata;
        }

        // Public methods -----------------------------------------------------

        public ManagedAnimatedPropertyMetadata(object defaultValue = null, CoerceValueDelegate coerceValueHandler = null, TypeSerializer customSerializer = null)
        {
            DefaultValue = defaultValue;
            CoerceValueHandler = coerceValueHandler;
            CustomSerializer = customSerializer;
        }

        // Public properties --------------------------------------------------

        public object DefaultValue { get; }
        public CoerceValueDelegate CoerceValueHandler { get; }
        public TypeSerializer CustomSerializer { get; }
    }
}
