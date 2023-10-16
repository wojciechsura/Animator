using Animator.Designer.BusinessLogic.Helpers;
using Animator.Designer.BusinessLogic.Infrastructure;
using Animator.Designer.BusinessLogic.Types;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class PropertyViewModel : BaseViewModel, IValueHandler
    {
        private bool isSelected;
        private bool isExpanded;

        private NamespaceType GetNamespaceType(Type type)
        {
            var ns = type.ToNamespaceDefinition().ToString();

            if (ns == context.DefaultNamespace)
                return NamespaceType.Default;
            else if (ns == context.EngineNamespace)
                return NamespaceType.Engine;
            else
                return NamespaceType.Other;
        }

        protected readonly WrapperContext context;

        protected (bool result, ManagedObjectViewModel obj) DeserializeObjectFromClipboard()
        {
            // Clipboard must contain text

            if (!System.Windows.Clipboard.ContainsText())
                return (false, null);

            // The text must be XML

            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(Clipboard.GetText());
            }
            catch
            {
                return (false, null);
            }

            // The XML must be a valid serialized ManagedObject

            var serialier = new MovieSerializer();

            ObjectViewModel result;
            WrapperContext newContext;
            try
            {
                (result, newContext) = serialier.Deserialize(document);
            }
            catch
            {
                return (false, null);
            }

            // Object must be a ManagedObjectViewModel

            if (result is not ManagedObjectViewModel managedResult)
                return (false, null);

            // Merge deserialized context with current one

            context.Merge(newContext);

            return (true, managedResult);
        }

        protected TypeViewModel BuildTypeViewModel(Type type, ICommand command)
        {
            return new TypeViewModel(type, command, TypeIconHelper.GetIcon(GetNamespaceType(type), type.Name));
        }

        public abstract void RequestSwitchToString();

        public abstract void RequestDelete(ObjectViewModel obj);

        public abstract string Name { get; }
        public abstract string Namespace { get; }

        public PropertyViewModel(ObjectViewModel parent, WrapperContext context)
        {
            Parent = parent;
            this.context = context;            
        }

        public virtual void NotifyAvailableTypesChanged()
        {
            OnPropertyChanged(nameof(AvailableTypes));
            OnPropertyChanged(nameof(AvailableMarkupExtensions));
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

        public virtual IEnumerable<ResourceKeyViewModel> AvailableResources { get; } = null;

        public virtual IEnumerable<MacroKeyViewModel> AvailableMacros { get; } = null;

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
        public ICommand SetToFromResourceCommand { get; init; }
        public ICommand SetToSpecificMacroCommand { get; init; }
        public ICommand AddSpecificMacroCommand { get; init; }

        public ICommand PasteCommand { get; init; }
    }
}
