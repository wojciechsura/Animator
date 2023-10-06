using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public abstract class BaseObjectViewModel : BaseViewModel
    {
        private bool isSelected;
        private bool isExpanded;

        protected readonly WrapperContext context;

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

        public BaseObjectViewModel(WrapperContext context)
        {
            this.context = context;
        }

        public virtual void NotifyAvailableTypesChanged()
        {
            foreach (var prop in Properties)
                prop.NotifyAvailableTypesChanged();
        }

        public TProperty Property<TProperty>(string name)
            where TProperty : PropertyViewModel =>
            Properties
                .OfType<TProperty>()
                .SingleOrDefault(prop => prop.Namespace == context.DefaultNamespace && prop.Name == name);

        public TProperty Property<TProperty>(string ns, string name)
            where TProperty : PropertyViewModel =>
            Properties
                .OfType<TProperty>()
                .SingleOrDefault(prop => prop.Namespace == ns && prop.Name == name);

        public abstract IEnumerable<PropertyViewModel> Properties { get; }

        public abstract IEnumerable<BaseObjectViewModel> DisplayChildren { get; }

        public PropertyViewModel this[string name]
        {
            get => Properties.SingleOrDefault(prop => prop.Namespace == context.DefaultNamespace && prop.Name == name);
        }

        public PropertyViewModel this[string ns, string name]
        {
            get => Properties.SingleOrDefault(prop => prop.Namespace == ns && prop.Name == name);
        }

        public string Icon { get; init; } = "Generic16.png";
    }
}
