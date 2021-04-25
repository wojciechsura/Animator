using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    public class SimpleCollectionPropertyClass : ManagedObject
    {

        #region Collection managed collection

        public ManagedCollection<int> Collection
        {
            get => (ManagedCollection<int>)GetValue(CollectionProperty);
        }

        public static readonly ManagedProperty CollectionProperty = ManagedProperty.RegisterCollection(typeof(SimpleCollectionPropertyClass),
            nameof(Collection),
            typeof(ManagedCollection<int>));

        #endregion
    }
}
