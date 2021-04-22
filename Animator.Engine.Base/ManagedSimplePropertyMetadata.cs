using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public delegate object CoerceValueDelegate(ManagedObject obj, object baseValue);

    public class PropertyValueChangedEventArgs : EventArgs
    {
        public PropertyValueChangedEventArgs(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object OldValue { get; }
        public object NewValue { get; }
    }

    public delegate void PropertyValueChangedDelegate(ManagedObject sender, PropertyValueChangedEventArgs args);

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

        public ManagedSimplePropertyMetadata(object defaultValue = null, PropertyValueChangedDelegate valueChangedHandler = null, CoerceValueDelegate coerceValueHandler = null, TypeSerializer customSerializer = null)
        {
            DefaultValue = defaultValue;
            ValueChangedHandler = valueChangedHandler;
            CoerceValueHandler = coerceValueHandler;
            CustomSerializer = customSerializer;
        }

        // Public properties --------------------------------------------------

        public object DefaultValue { get; }
        public PropertyValueChangedDelegate ValueChangedHandler { get; }
        public CoerceValueDelegate CoerceValueHandler { get; }
        public TypeSerializer CustomSerializer { get; }
        public bool NotAnimatable { get; init; } = false;

    }
}
