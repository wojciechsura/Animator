﻿using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class PropertyProxyViewModel : VirtualObjectViewModel
    {
        private readonly List<PropertyViewModel> properties = new();
        private readonly ManagedPropertyViewModel property;

        private IEnumerable<BaseObjectViewModel> GetDisplayChildren()
        {
            if (property.Value is ReferenceValueViewModel refValue)
            {
                return new ObjectViewModel[] { refValue.Value };
            }
            else if (property.Value is CollectionValueViewModel collection)
            {
                return collection.Items;
            }
            else if (property.Value is MarkupExtensionValueViewModel markup)
            {
                return new ObjectViewModel[] { markup.Value };
            }
            else
                return Enumerable.Empty<BaseObjectViewModel>();
        }

        private void OnDisplayChildrenChanged()
        {
            OnPropertyChanged(nameof(DisplayChildren));
        }

        private void HandlePropertyValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ManagedPropertyViewModel.Value))
                OnDisplayChildrenChanged();
        }

        private void HandleCollectionChanged(object sender, EventArgs e)
        {
            OnDisplayChildrenChanged();
        }

        public PropertyProxyViewModel(WrapperContext context, ManagedPropertyViewModel property)
            : base(context)
        {
            this.property = property;

            property.PropertyChanged += HandlePropertyValueChanged;
            property.CollectionChanged += HandleCollectionChanged;

            Icon = "Property16.png";
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public string Name => property.Name;

        public override IEnumerable<BaseObjectViewModel> DisplayChildren => GetDisplayChildren();

        // Transported from the property

        public ICommand AddInstanceCommand => property.AddInstanceCommand;

        public ICommand SetToInstanceCommand => property.SetToInstanceCommand;

        public IEnumerable<TypeViewModel> AvailableTypes => property.AvailableTypes;
    }
}
