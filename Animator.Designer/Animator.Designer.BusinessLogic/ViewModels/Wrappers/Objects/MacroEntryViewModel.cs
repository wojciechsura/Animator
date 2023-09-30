using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MacroEntryViewModel : BaseObjectViewModel
    {
        private readonly List<PropertyViewModel> properties = new();
        private readonly StringPropertyViewModel key;
        private readonly ReferencePropertyViewModel content;

        private IEnumerable<BaseObjectViewModel> GetChildren()
        {
            if (content.Value is ReferenceValueViewModel refValue)
                yield return refValue.Value;
        }

        private void HandleKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Key));
        }

        private void HandleContentChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(DisplayChildren));
        }

        public MacroEntryViewModel(WrapperContext context, string defaultNamespace, string engineNamespace)
            : base(context, defaultNamespace, engineNamespace)
        {
            Namespace = engineNamespace;

            key = new StringPropertyViewModel(context, engineNamespace, "Key");
            key.PropertyChanged += HandleKeyChanged;
            properties.Add(key);

            content = new ReferencePropertyViewModel(context, defaultNamespace, "Content");
            content.PropertyChanged += HandleContentChanged;
            properties.Add(content);

            Icon = "MacroDefinition16.png";
        }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public override IEnumerable<BaseObjectViewModel> DisplayChildren => GetChildren();

        public string Namespace { get; }

        public string Key => key.Value;
    }
}
