using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ManagedProperty
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

        private static void ValidateDuplicatedName(Type ownerClassType, string name)
        {
            if (FindWithInheritance(ownerClassType, name) != null)
                throw new ArgumentException("Property already registered (possibly in base class)!", nameof(name));
        }

        private static void ValidateInheritanceFromManagedObject(Type ownerClassType)
        {
            var parentType = ownerClassType;
            while (parentType != typeof(ManagedObject) && parentType != typeof(object))
                parentType = parentType.BaseType;

            if (parentType != typeof(ManagedObject))
                throw new ArgumentException("Owner class type must derive from ManagedObject!");
        }

        private static void ValidateListType(Type propertyType)
        {
            if (!propertyType.IsAssignableTo(typeof(IList)))
                throw new ArgumentException("When registering a collection, property type must implement IList interface!");            
        }

        // Private methods ----------------------------------------------------

        private bool ValidatePropertyName(string name)
        {
            return nameRegex.IsMatch(name);
        }

        // Internal static methods --------------------------------------------

        internal static ManagedProperty ByTypeAndName(Type ownerClassType, string name)
        {
            return FindWithInheritance(ownerClassType, name);
        }

        // Internal methods ---------------------------------------------------

        internal ManagedProperty(Type ownerClassType, string name, Type type)
        {
            this.ownerClassType = ownerClassType ?? throw new ArgumentNullException(nameof(ownerClassType));

            if (!ValidatePropertyName(name))
                throw new ArgumentException(nameof(name));

            this.name = name;
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.globalIndex = nextAvailableGlobalIndex++;
        }

        // Public static methods ----------------------------------------------

        public static ManagedSimpleProperty Register(Type ownerClassType, string name, Type propertyType, ManagedSimplePropertyMetadata metadata = null)
        {
            ValidateDuplicatedName(ownerClassType, name);
            ValidateInheritanceFromManagedObject(ownerClassType);

            if (metadata == null)
                metadata = ManagedSimplePropertyMetadata.Default;

            var prop = new ManagedSimpleProperty(ownerClassType, name, propertyType, metadata);

            var propertyKey = new PropertyKey(ownerClassType, name);

            propertyDefinitions[propertyKey] = prop;

            return prop;
        }

        public static ManagedProperty RegisterCollection(Type ownerClassType, string name, Type propertyType, ManagedCollectionMetadata metadata = null)
        {
            ValidateDuplicatedName(ownerClassType, name);
            ValidateInheritanceFromManagedObject(ownerClassType);
            ValidateListType(propertyType);

            if (metadata == null)
                metadata = ManagedCollectionMetadata.DefaultFor(propertyType);

            var prop = new ManagedCollectionProperty(ownerClassType, name, propertyType, metadata);

            var propertyKey = new PropertyKey(ownerClassType, name);

            propertyDefinitions[propertyKey] = prop;

            return prop;
        }

        // Public properties --------------------------------------------------

        public string Name => name;
        public Type OwnerClassType => ownerClassType;
        public Type Type => type;
        public int GlobalIndex => globalIndex;

        // Public static properties -------------------------------------------

        public static object UnsetValue { get; } = new object();
    }
}
