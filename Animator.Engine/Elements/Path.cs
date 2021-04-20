using Animator.Engine.Base;
using Animator.Engine.Elements.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{    
    public class Path : ManagedObject
    {
        #region Items managed collection

        public List<PathElement> Items
        {
            get => (List<PathElement>)GetValue(ItemsProperty);
        }

        public static readonly ManagedProperty ItemsProperty = ManagedProperty.RegisterCollection(typeof(Path),
            nameof(Items),
            typeof(List<PathElement>),
            new ManagedCollectionMetadata(() => new List<PathElement>(), new PathElementsSerializer()));

        #endregion
    }
}
