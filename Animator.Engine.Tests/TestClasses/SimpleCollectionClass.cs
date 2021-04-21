using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    [ContentProperty(nameof(SimpleCollectionClass.Items))]
    public class SimpleCollectionClass : ManagedObject
    {

        #region Items managed collection

        public List<SimplePropertyClass> Items
        {
            get => (List<SimplePropertyClass>)GetValue(ItemsProperty);
        }

        public static readonly ManagedProperty ItemsProperty = ManagedProperty.RegisterCollection(typeof(SimpleCollectionClass),
            nameof(Items),
            typeof(List<SimplePropertyClass>));

        #endregion        
    }
}
