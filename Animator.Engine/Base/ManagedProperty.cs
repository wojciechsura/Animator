using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public sealed class ManagedProperty
    {
        // Private types ------------------------------------------------------

        private class PropertyKey
        {
            private readonly string name;
            private readonly Type ownerType;
            private readonly int hashCode;

            public PropertyKey(Type ownerType, string name)
            {
                this.name = name;
                this.ownerType = ownerType;

                hashCode = name.GetHashCode() ^ ownerType.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is PropertyKey propertyKey)
                {
                    return name.Equals(propertyKey.name) &&
                        ownerType.Equals(propertyKey.ownerType);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return hashCode;
            }
        }

        // Private static fields ----------------------------------------------

        private static readonly Dictionary<PropertyKey, ManagedProperty> propertyDefinitions = new();
        private static int nextAvailableGlobalIndex;

        private static readonly Regex nameRegex = new Regex("^[a-zA-Z_][a-zA-Z_0-9]*$");

        // Private fields -----------------------------------------------------

        private readonly Type ownerClassType;
        private readonly string name;
        private readonly Type type;
        private readonly ManagedPropertyMetadata metadata;
        private readonly int globalIndex;

        // Private static methods ---------------------------------------------

        private static ManagedProperty FindWithInheritance(Type ownerClassType, string name)
        {
            while (ownerClassType != typeof(object))
            {
                var key = new PropertyKey(ownerClassType, name);
                if (propertyDefinitions.TryGetValue(key, out ManagedProperty result))
                    return result;

                ownerClassType = ownerClassType.BaseType;
            }

            return null;
        }

        // Private methods ----------------------------------------------------

        private ManagedProperty(Type ownerClassType, string name, Type type, ManagedPropertyMetadata metadata)
        {
            this.ownerClassType = ownerClassType ?? throw new ArgumentNullException(nameof(ownerClassType));

            if (!ValidatePropertyName(name))
                throw new ArgumentException(nameof(name));

            this.name = name;
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.globalIndex = nextAvailableGlobalIndex++;
        }

        private bool ValidatePropertyName(string name)
        {
            return nameRegex.IsMatch(name);
        }

        // Internal static methods --------------------------------------------

        internal static ManagedProperty ByTypeAndName(Type ownerClassType, string name)
        {
            return FindWithInheritance(ownerClassType, name);
        }

        // Public static methods ----------------------------------------------

        public static ManagedProperty Register(Type ownerClassType, string name, Type propertyType, ManagedPropertyMetadata metadata = null)
        {
            if (FindWithInheritance(ownerClassType, name) != null)
                throw new ArgumentException("Property already registered (possibly in base class)!", nameof(name));

            var parentType = ownerClassType;
            while (parentType != typeof(ManagedObject) && parentType != typeof(object))
                parentType = parentType.BaseType;

            if (parentType != typeof(ManagedObject))
                throw new ArgumentException("Owner class type must derive from ManagedObject!");

            if (metadata == null)
                metadata = ManagedPropertyMetadata.Default;

            var prop = new ManagedProperty(ownerClassType, name, propertyType, metadata);

            var propertyKey = new PropertyKey(ownerClassType, name);

            propertyDefinitions[propertyKey] = prop;

            return prop;
        }

        // Public properties --------------------------------------------------

        public string Name => name;
        public Type OwnerClassType => ownerClassType;
        public Type Type => type;
        public int GlobalIndex => globalIndex;
        public ManagedPropertyMetadata Metadata => metadata;

        // Public static properties -------------------------------------------

        public static object UnsetValue { get; } = new object();
    }
}
