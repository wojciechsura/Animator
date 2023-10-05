using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Animator.Engine.Base.Extensions;
using Newtonsoft.Json.Linq;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class ManagedPropertyViewModel : PropertyViewModel
    {
        // Private methods ----------------------------------------------------

        private void SetToMarkupExtension(Type type)
        {
            var namespaceDefinition = type.ToNamespaceDefinition();
            var markupExtensionViewModel = new MarkupExtensionViewModel(context, namespaceDefinition.ToString(), type.Name, type);
            var markupExtensionValue = new MarkupExtensionValueViewModel(markupExtensionViewModel);
            Value = markupExtensionValue;
        }

        private void SetValue(ValueViewModel value)
        {
            if (this.value != null) 
            {                
                value.Handler = null;
                value.Parent = null;
            }

            OnSetValue(value);

            if (this.value != null)
            {
                value.Parent = this;
                value.Handler = this;
            }
        }
        
        // Protected fields ---------------------------------------------------

        protected ValueViewModel value;

        // Protected methods --------------------------------------------------

        protected IEnumerable<ResourceKeyViewModel> InternalGetAvailableResources(Func<ManagedObjectViewModel, bool> matchesCurrentType)
        {
            var currentObject = Parent;

            var result = new List<ResourceKeyViewModel>();

            while (currentObject != null)
            {
                var resourcesProperty = currentObject.Property<ManagedCollectionPropertyViewModel>(context.DefaultNamespace, "Resources");
                if (resourcesProperty != null)
                {
                    var resourcesValue = resourcesProperty.Value as CollectionValueViewModel;
                    if (resourcesValue != null)
                    {
                        foreach (var resource in resourcesValue.Items
                            .OfType<ManagedObjectViewModel>()
                            .Where(resource => matchesCurrentType(resource)))
                        {
                            var keyProp = resource.Property<ManagedSimplePropertyViewModel>(context.DefaultNamespace, "Key");
                            if (keyProp != null && keyProp.Value is StringValueViewModel strValue)
                                result.Add(new ResourceKeyViewModel(strValue.Value, SetToFromResourceCommand));
                        }
                    }
                }

                // Object -> Value -> Property -> Object
                currentObject = currentObject.Parent?.Parent?.Parent;
            }

            result.Sort((x, y) => string.Compare(x.Key, y.Key));
            return result;
        }

        protected void OnCollectionChanged() => CollectionChanged?.Invoke(this, EventArgs.Empty);

        protected abstract void OnSetValue(ValueViewModel value);

        protected void OnStringValueChanged() => StringValueChanged?.Invoke(this, EventArgs.Empty);

        protected void SetToFromResource(string key)
        {
            var type = typeof(Animator.Engine.Elements.FromResource);

            var namespaceDefinition = type.ToNamespaceDefinition();
            var markupExtensionViewModel = new MarkupExtensionViewModel(context, namespaceDefinition.ToString(), type.Name, type);
            var markupExtensionValue = new MarkupExtensionValueViewModel(markupExtensionViewModel);

            var keyProperty = markupExtensionViewModel.Property<ClearableStringPropertyViewModel>(context.DefaultNamespace, nameof(Animator.Engine.Elements.FromResource.Key));
            keyProperty.Value = new StringValueViewModel(key);

            Value = markupExtensionValue;
        }

        // Public methods -----------------------------------------------------

        public ManagedPropertyViewModel(ObjectViewModel parent, WrapperContext context, ManagedProperty property)
            : base(parent, context)
        {
            Name = property.Name;
            Namespace = context.DefaultNamespace;

            SetToMarkupExtensionCommand = new AppCommand(obj => SetToMarkupExtension((Type)obj));
        }

        public override IEnumerable<TypeViewModel> AvailableMarkupExtensions
        {
            get
            {
                return context.Namespaces
                    .OfType<AssemblyNamespaceViewModel>()
                    .SelectMany(ns => ns.GetAvailableTypesFor(typeof(Animator.Engine.Base.BaseMarkupExtension)))
                    .OrderBy(mx => mx.Name)
                    .Select(type => new TypeViewModel(type, SetToMarkupExtensionCommand));
            }
        }

        // Public properties --------------------------------------------------

        public event EventHandler CollectionChanged;

        public abstract ManagedProperty ManagedProperty { get; }

        public override string Name { get; }

        public override string Namespace { get; }

        public event EventHandler StringValueChanged;

        public ValueViewModel Value
        {
            get => value;
            set => SetValue(value);
        }
    }
}
