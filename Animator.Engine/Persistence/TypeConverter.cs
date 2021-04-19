using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Persistence
{
    public static class TypeConverter
    {
        public static object Deserialize(string value, Type type)
        {
            if (!type.IsValueType && String.IsNullOrEmpty(value))
                return null;

            // TODO extend
            return Convert.ChangeType(value, type);
        }

        public static string Serialize(object value)
        {
            // TODO extend
            return value?.ToString() ?? String.Empty;
        }
    }
}
