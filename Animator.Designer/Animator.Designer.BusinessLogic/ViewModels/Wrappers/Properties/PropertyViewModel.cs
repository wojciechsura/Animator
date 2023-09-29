﻿using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class PropertyViewModel : BaseViewModel, IValueHandler
    {
        private bool isSelected;
        private bool isExpanded;

        protected readonly WrapperContext context;

        public abstract string Name { get; }
        public abstract string Namespace { get; }

        public PropertyViewModel(WrapperContext context)
        {
            this.context = context;
        }

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
