using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Persistence
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ContentPropertyAttribute : Attribute
    {
        public ContentPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
