using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class SimpleCollectionPropertyClass : ManagedObject
    {

        #region Collection managed collection

        public List<int> Collection
        {
            get => (List<int>)GetValue(CollectionProperty);
        }

        public static readonly ManagedProperty CollectionProperty = ManagedProperty.RegisterCollection(typeof(SimpleCollectionPropertyClass),
            nameof(Collection),
            typeof(List<int>));

        #endregion
    }
}
