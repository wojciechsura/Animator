using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Editor.Configuration
{ 
    public class ConfigValue<T> : BaseTypedValue<T>
    {
        // Private methods ----------------------------------------------------

        protected override string SerializeValue(T value)
        {
            return value.ToString();
        }

        protected override T DeserializeValue(string text)
        {
            if (typeof(T).IsEnum)
                return ((T)Enum.Parse(typeof(T), text));
            else
                return (T)Convert.ChangeType(text, typeof(T));
        }

        // Public methods -----------------------------------------------------

        public ConfigValue(string xmlName, 
            BaseItemContainer owner,             
            T defaultValue = default(T)) 
            : base(xmlName, owner, defaultValue)
        {
        }
    }
}
