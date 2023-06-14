using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    internal class StringDataClass : ManagedObject
    {
        #region StringValue managed property

        public string StringValue
        {
            get => (string)GetValue(StringValueProperty);
            set => SetValue(StringValueProperty, value);
        }

        public static readonly ManagedProperty StringValueProperty = ManagedProperty.Register(typeof(StringDataClass),
            nameof(StringValue),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = string.Empty });

        #endregion

    }
}
