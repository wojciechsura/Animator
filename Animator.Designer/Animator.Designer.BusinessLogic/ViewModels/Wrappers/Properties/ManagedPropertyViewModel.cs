using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class ManagedPropertyViewModel : PropertyViewModel
    {
        public ManagedPropertyViewModel(ManagedProperty property)
        {
            Name = property.Name;
        }

        public abstract ManagedProperty ManagedProperty { get; }

        public override string Name { get; }
    }
}
