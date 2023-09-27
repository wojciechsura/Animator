using Animator.Designer.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class PropertyViewModel : BaseViewModel
    {
        private bool isSelected;
        private bool isExpanded;

        public abstract string Name { get; }
        public abstract string Namespace { get; }

        public bool IsSelected
        {
            get => isSelected;
            set => Set(ref isSelected, value);
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set => Set(ref isExpanded, value);
        }
    }
}
