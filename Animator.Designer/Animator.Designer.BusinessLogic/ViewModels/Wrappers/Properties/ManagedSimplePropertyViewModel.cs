using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ManagedSimplePropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedSimpleProperty simpleProperty;
        private ValueViewModel value;

        public ManagedSimplePropertyViewModel(ManagedSimpleProperty property)
            : base(property)
        {
            this.simpleProperty = property;
            SetDefault(property);
        }

        public void SetDefault(ManagedSimpleProperty property)
        {
            var defaultValue = property.Metadata.DefaultValue;
            value = new DefaultValueViewModel(defaultValue);
        }

        public override ManagedProperty ManagedProperty => simpleProperty;

        public ValueViewModel Value
        {
            get => value;
            set
            {
                if (value is StringValueViewModel)
                    Set(ref this.value, value);
                else if (value is DefaultValueViewModel)
                    throw new ArgumentException("Use SetDefault method instead of setting DefaultValueViewModel!");
                else
                    throw new ArgumentException($"ManagedSimplePropertyViewModel does not support value of type {value}!");
            }
        }
    }
}
