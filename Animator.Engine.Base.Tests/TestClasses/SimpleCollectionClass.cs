using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    [ContentProperty(nameof(Items))]
    public class SimpleCollectionClass : ManagedObject
    {

        #region Items managed collection

        public ManagedCollection<SimplePropertyClass> Items
        {
            get => (ManagedCollection<SimplePropertyClass>)GetValue(ItemsProperty);
        }

        public static readonly ManagedProperty ItemsProperty = ManagedProperty.RegisterCollection(typeof(SimpleCollectionClass),
            nameof(Items),
            typeof(ManagedCollection<SimplePropertyClass>));

        #endregion        
    }
}
