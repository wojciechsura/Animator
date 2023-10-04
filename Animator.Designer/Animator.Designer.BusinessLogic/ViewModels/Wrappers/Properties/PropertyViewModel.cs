﻿using Animator.Designer.BusinessLogic.Infrastructure;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class PropertyViewModel : BaseViewModel, IValueHandler
    {
        private bool isSelected;
        private bool isExpanded;

        protected readonly WrapperContext context;

        public abstract void RequestSwitchToString();

        public abstract void RequestDelete(ObjectViewModel obj);

        public abstract string Name { get; }
        public abstract string Namespace { get; }

        public PropertyViewModel(ObjectViewModel parent, WrapperContext context)
        {
            Parent = parent;
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

        public virtual IEnumerable<TypeViewModel> AvailableTypes { get; } = null;

        public virtual IEnumerable<TypeViewModel> AvailableMarkupExtensions { get; } = null;

        public ObjectViewModel Parent { get; set; }

        public IList<string> AvailableOptions { get; init; }

        public ICommand SetDefaultCommand { get; init; }
        public ICommand SetToStringCommand { get; init; }
        public ICommand SetToCollectionCommand { get; init; }
        public ICommand SetToInstanceCommand { get; init; }
        public ICommand AddInstanceCommand { get; init; }
        public ICommand InsertMacroCommand { get; init; }
        public ICommand InsertIncludeCommand { get; init; }
        public ICommand InsertGeneratorCommand { get; init; }
        public ICommand AddMacroDefinitionCommand { get; init; }
        public ICommand SetToMarkupExtensionCommand { get; init; }
    }
}
