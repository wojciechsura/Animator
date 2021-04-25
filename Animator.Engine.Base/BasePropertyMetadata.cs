using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class PropertyValueChangedEventArgs : EventArgs
    {
        public PropertyValueChangedEventArgs(ManagedProperty property, object oldValue, object newValue)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ManagedProperty Property { get; }
        public object OldValue { get; }
        public object NewValue { get; }
    }

    public delegate void PropertyValueChangedDelegate(ManagedObject sender, PropertyValueChangedEventArgs args);

    public class BasePropertyMetadata
    {
        public bool NotSerializable { get; init; } = false;
    }
}
