using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ManagedReferencePropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedReferenceProperty referenceProperty;
        private ValueViewModel value;

        public ManagedReferencePropertyViewModel(ManagedReferenceProperty referenceProperty)
            : base(referenceProperty)
        {
            this.referenceProperty = referenceProperty;
        }

        public override ManagedProperty ManagedProperty => referenceProperty;

        public ValueViewModel Value
        {
            get => value;
            set
            {
                if (value is StringValueViewModel)
                    Set(ref this.value, value);
                else if (value is ReferenceValueViewModel)
                    Set(ref this.value, value);   
                else if (value is MarkupExtensionViewModel)
                    Set(ref this.value, value);
                else
                    throw new ArgumentException($"ManagedReferencePropertyViewModel does not support value of type {value}!");
            }
        }
    }
}
