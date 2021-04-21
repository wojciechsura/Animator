using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class SimplePropertyClass : ManagedObject
    {
        #region IntValue managed property

        public int IntValue
        {
            get => (int)GetValue(IntValueProperty);
            set => SetValue(IntValueProperty, value);
        }

        public static ManagedProperty IntValueProperty = ManagedProperty.Register(typeof(SimplePropertyClass), 
            nameof(IntValue), 
            typeof(int), 
            new ManagedAnimatedPropertyMetadata(0));

        #endregion
    }
}
