using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public delegate object CoerceValueDelegate(ManagedObject obj, object baseValue);

    public class ManagedSimplePropertyMetadata : BasePropertyMetadata
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

        public ManagedSimplePropertyMetadata(object defaultValue = null, CoerceValueDelegate coerceValueHandler = null, CustomPropertySerializer serializer = null)
        {
            DefaultValue = defaultValue;
            CoerceValueHandler = coerceValueHandler;
            Serializer = serializer;
        }

        // Public properties --------------------------------------------------

        public object DefaultValue { get; }
        public CoerceValueDelegate CoerceValueHandler { get; }
        public CustomPropertySerializer Serializer { get; }
    }
}
