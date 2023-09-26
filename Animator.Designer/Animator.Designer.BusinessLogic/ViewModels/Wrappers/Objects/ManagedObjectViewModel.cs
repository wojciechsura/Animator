using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class ManagedObjectViewModel : BaseObjectViewModel
    {
        private readonly ObservableCollection<PropertyViewModel> properties = new();
        private readonly ObservableCollection<MacroEntryViewModel> macros = new();
        private readonly string name;
        private readonly string fullClassName;
        private readonly ManagedPropertyViewModel contentProperty;

        public ManagedObjectViewModel(string name, string fullClassName, Type type)
        {
            this.name = name;
            this.fullClassName = fullClassName;

            foreach (var property in ManagedProperty.FindAllByType(type, true))
            {
                if (property.Metadata.NotSerializable)
                    continue;

                switch (property)
                {
                    case ManagedSimpleProperty simple:
                        {
                            var prop = new ManagedSimplePropertyViewModel(simple);
                            properties.Add(prop);
                            break;
                        }
                    case ManagedCollectionProperty collection:
                        {
                            var prop = new ManagedCollectionPropertyViewModel(collection);
                            properties.Add(prop);
                            break;
                        }
                    case ManagedReferenceProperty reference:
                        {
                            var prop = new ManagedReferencePropertyViewModel(reference);
                            properties.Add(prop);
                            break;
                        }
                    default:
                        throw new InvalidOperationException("Unsupported managed property type!");
                }
            }

            var contentPropertyAttribute = type.GetCustomAttribute<ContentPropertyAttribute>(true);
            if (contentPropertyAttribute != null)
                contentProperty = properties.OfType<ManagedPropertyViewModel>().Single(prop => prop.Name == contentPropertyAttribute.PropertyName);
            else
                contentProperty = null;
        }

        public IList<MacroEntryViewModel> Macros => macros;

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public ManagedPropertyViewModel ContentProperty => contentProperty;
    }
}
