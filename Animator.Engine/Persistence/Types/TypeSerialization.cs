using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Persistence.Types
{
    public static class TypeSerialization
    {
        public static bool CanDeserialize(string value, Type type)
        {
            if (TypeSerializerRepository.Supports(type))
            {
                var serializer = TypeSerializerRepository.GetSerializerFor(type);                
                return serializer.CanDeserialize(value);
            }

            return false;
        }

        public static object Deserialize(string value, Type type)
        {
            if (TypeSerializerRepository.Supports(type))
                return TypeSerializerRepository.GetSerializerFor(type).Deserialize(value);

            // TODO Attribute for custom type converter

            throw new InvalidCastException($"Unsupported serialization from value: {value} to type {type.Name}");
        }

        public static string Serialize(object value)
        {
            if (TypeSerializerRepository.Supports(value.GetType()))
                return TypeSerializerRepository.GetSerializerFor(value.GetType()).Serialize(value);

            throw new InvalidCastException($"Unsupported serialization of object type {value.GetType().Name} to string!");
        }
    }
}
