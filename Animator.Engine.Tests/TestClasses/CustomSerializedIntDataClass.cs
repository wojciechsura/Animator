using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class CustomSerializedIntDataClass : ManagedObject
    {
        #region IntValue managed property

        public int IntValue
        {
            get => (int)GetValue(IntValueProperty);
            set => SetValue(IntValueProperty, value);
        }

        public static readonly ManagedProperty IntValueProperty = ManagedProperty.Register(typeof(CustomSerializedIntDataClass),
            nameof(IntValue),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0, CustomSerializer = new CustomIntSerializer() });

        #endregion

        #region IntCollection managed collection

        public ManagedCollection<int> IntCollection
        {
            get => (ManagedCollection<int>)GetValue(IntCollectionProperty);
        }

        public static readonly ManagedProperty IntCollectionProperty = ManagedProperty.RegisterCollection(typeof(CustomSerializedIntDataClass),
            nameof(IntCollection),
            typeof(ManagedCollection<int>), 
            new ManagedCollectionMetadata { CustomSerializer = new CustomIntListSerializer() });

        #endregion
    }
}
