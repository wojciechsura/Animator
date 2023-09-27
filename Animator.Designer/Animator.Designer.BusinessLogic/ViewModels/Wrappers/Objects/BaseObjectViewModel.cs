﻿using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public abstract class BaseObjectViewModel : BaseViewModel
    {
        private bool isSelected;
        private bool isExpanded;

        protected readonly string defaultNamespace;
        protected readonly string engineNamespace;

        public BaseObjectViewModel(string defaultNamespace, string engineNamespace)
        {
            this.defaultNamespace = defaultNamespace;
            this.engineNamespace = engineNamespace;
        }

        public TProperty Property<TProperty>(string name)
            where TProperty : PropertyViewModel =>
            Properties
                .OfType<TProperty>()
                .SingleOrDefault(prop => prop.Namespace == defaultNamespace && prop.Name == name);

        public TProperty Property<TProperty>(string ns, string name)
            where TProperty : PropertyViewModel =>        
            Properties
                .OfType<TProperty>()
                .SingleOrDefault(prop => prop.Namespace == ns && prop.Name == name);
        
        public abstract IReadOnlyList<PropertyViewModel> Properties { get; }

        public PropertyViewModel this[string name]
        {
            get => Properties.SingleOrDefault(prop => prop.Namespace == defaultNamespace && prop.Name == name);
        }

        public PropertyViewModel this[string ns, string name]
        {
            get => Properties.SingleOrDefault(prop => prop.Namespace == ns && prop.Name == name);
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