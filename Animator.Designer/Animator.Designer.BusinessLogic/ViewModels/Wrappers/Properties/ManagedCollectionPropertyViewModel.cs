using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ManagedCollectionPropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedCollectionProperty collectionProperty;
        private ValueViewModel value;

        public ManagedCollectionPropertyViewModel(ManagedCollectionProperty collectionProperty)
            : base(collectionProperty)
        {
            this.collectionProperty = collectionProperty;
        }

        public ValueViewModel Value
        {
            get => value;
            set
            {
                if (value is StringValueViewModel)
                    Set(ref this.value, value);
                else if (value is CollectionValueViewModel)
                    Set(ref this.value, value);
                else                    
                    throw new ArgumentException($"ManagedCollectionPropertyViewModel does not support value of type {value}!");
            }
        }

        public override ManagedProperty ManagedProperty => collectionProperty;
    }
}
