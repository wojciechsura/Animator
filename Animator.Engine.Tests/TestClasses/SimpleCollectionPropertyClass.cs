using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class SimpleCollectionPropertyClass : ManagedObject
    {
        public List<int> Collection
        {
            get => (List<int>)GetValue(CollectionProperty);
        }

        public static readonly ManagedProperty CollectionProperty = ManagedProperty.RegisterCollection(typeof(SimpleCoercedPropertyClass), nameof(CollectionProperty), typeof(List<int>));
    }
}
