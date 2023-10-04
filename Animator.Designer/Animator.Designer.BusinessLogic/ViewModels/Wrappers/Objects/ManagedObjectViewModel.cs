using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class ManagedObjectViewModel : ObjectViewModel
    {
        // Private types ------------------------------------------------------

        private enum NamespaceType
        {
            Default,
            Engine
        }

        // Private constants --------------------------------------------------

        private const int MAX_VALUE_LENGTH = 64;

        private static readonly Dictionary<Type, (NamespaceType Namespace, string Property, string Color)> namePropDefinitions = new()
        {
            { typeof(Animator.Engine.Elements.SceneElement), (NamespaceType.Default, nameof(Animator.Engine.Elements.SceneElement.Name), "#000000") },
            { typeof(Animator.Engine.Elements.Resource), (NamespaceType.Default, nameof(Animator.Engine.Elements.Resource.Key), "#0000ff") },
            { typeof(Animator.Engine.Elements.AnimateProperty), (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateProperty.PropertyRef), "#ff8000") }
        };

        private static readonly Dictionary<Type, (NamespaceType Namespace, string Property)> valuePropDefinitions = new()
        {
            { typeof(Animator.Engine.Elements.Label), (NamespaceType.Default, nameof(Animator.Engine.Elements.Label.Text)) },
            { typeof(Animator.Engine.Elements.Image), (NamespaceType.Default, nameof(Animator.Engine.Elements.Image.Source)) },
            { typeof(Animator.Engine.Elements.SvgImage), (NamespaceType.Default, nameof(Animator.Engine.Elements.SvgImage.Source)) }
        };

        // Private fields -----------------------------------------------------

        private readonly ObservableCollection<PropertyViewModel> properties = new();
        private readonly MacroCollectionPropertyViewModel macros;
        private readonly ManagedPropertyViewModel contentProperty;
        private readonly ManagedSimplePropertyViewModel nameProperty;
        private readonly ManagedSimplePropertyViewModel valueProperty;
        private PropertiesProxyViewModel propertiesProxy;

        // Private methods ----------------------------------------------------

        private string TruncateValue(string value)
        {
            if (value == null)
                return null;

            if (value.Length > MAX_VALUE_LENGTH)
                value = $"{value[..61]}...";

            return value;
        }

        private void HandleNameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Name));
        }

        private void HandleValueChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Value));
        }

        private void HandleSimplePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ManagedSimplePropertyViewModel simpleProperty && e.PropertyName == nameof(ManagedSimplePropertyViewModel.Value))
            {
                if (propertiesProxy == null ||
                    (simpleProperty.Value is MarkupExtensionValueViewModel &&
                        !propertiesProxy.DisplayChildren.OfType<PropertyProxyViewModel>().Any(proxy => proxy.Property == simpleProperty)) ||
                    (simpleProperty.Value is not MarkupExtensionValueViewModel &&
                        propertiesProxy.DisplayChildren.OfType<PropertyProxyViewModel>().Any(proxy => proxy.Property == simpleProperty)))
                {
                    // Rebuild properties proxy
                    var isExpanded = propertiesProxy?.IsExpanded ?? false;
                    
                    propertiesProxy = BuildPropertiesProxy();
                    
                    if (propertiesProxy != null)
                        propertiesProxy.IsExpanded = isExpanded;

                    // Rebuild displayed children
                    OnPropertyChanged(nameof(DisplayChildren));
                }
            }
        }

        private void NotifyDisplayChildrenChanged()
        {
            OnPropertyChanged(nameof(DisplayChildren));
        }

        private IEnumerable<BaseObjectViewModel> GetDisplayChildren()
        {
            // Collect all reference or collection properies,
            // which currently have reference or collection value

            if (propertiesProxy != null)
                yield return propertiesProxy;

            if (contentProperty != null)
            {
                if (contentProperty.Value is MarkupExtensionValueViewModel markupExtension)
                {
                    yield return markupExtension.Value;
                }
                else if (contentProperty is ManagedReferencePropertyViewModel reference)
                {
                    if (reference.Value is ReferenceValueViewModel refValue && refValue.Value != null)
                        yield return refValue.Value;
                }
                else if (contentProperty is ManagedCollectionPropertyViewModel collection)
                {
                    if (collection.Value is CollectionValueViewModel collectionValue)
                        foreach (var item in collectionValue.Items)
                            yield return item;
                }
            }
        }

        private PropertiesProxyViewModel BuildPropertiesProxy()
        {
            int CompareProxyProperties(VirtualObjectViewModel x, VirtualObjectViewModel y)
            {
                string GetString(VirtualObjectViewModel obj)
                {
                    if (obj is PropertyProxyViewModel prop)
                        return prop.Property.Name;
                    else if (obj is MacrosProxyViewModel)
                        return Animator.Designer.Resources.Controls.DocumentControl.Strings.Macros;
                    else
                        throw new InvalidOperationException("Unsupported proxy object!");
                }

                string strX = GetString(x);
                string strY = GetString(y);

                return string.Compare(strX, strY);
            }

            List<VirtualObjectViewModel> proxyProperties = new();

            proxyProperties.Add(new MacrosProxyViewModel(context, this, macros));

            foreach (var property in properties.OrderBy(prop => prop.Name))
            {
                if (property == contentProperty)
                    continue;

                if (property is ManagedReferencePropertyViewModel or ManagedCollectionPropertyViewModel ||
                    (property is ManagedSimplePropertyViewModel simple && simple.Value is MarkupExtensionValueViewModel))
                {
                    var propertyProxy = new PropertyProxyViewModel(context, (ManagedPropertyViewModel)property);
                    proxyProperties.Add(propertyProxy);
                }
            }

            if (proxyProperties.Any())
            {
                proxyProperties.Sort((x, y) => CompareProxyProperties(x, y));

                return new PropertiesProxyViewModel(context, proxyProperties);
            }
            else
                return null;
        }

        private string GetNamespace(NamespaceType ns)
        {
            return ns switch
            {
                NamespaceType.Default => context.DefaultNamespace,
                NamespaceType.Engine => context.EngineNamespace,
                _ => throw new InvalidEnumArgumentException("Unsupported namespace type!")
            };
        }

        private void HandleContentPropertyValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ManagedPropertyViewModel.Value))
                NotifyDisplayChildrenChanged();
        }

        private void HandleContentPropertyCollectionChanged(object sender, EventArgs e)
        {
            NotifyDisplayChildrenChanged();
        }

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        // Public methods -----------------------------------------------------

        public ManagedObjectViewModel(WrapperContext context, string ns, string className, Type type)
            : base(context)
        {
            macros = new MacroCollectionPropertyViewModel(this, context, context.EngineNamespace, "Macros");

            this.ClassName = className;
            this.Namespace = ns;

            // Build properties

            foreach (var property in ManagedProperty.FindAllByType(type, true).OrderBy(prop => prop.Name))
            {
                if (property.Metadata.NotSerializable)
                    continue;

                switch (property)
                {
                    case ManagedSimpleProperty simple:
                        {
                            var prop = new ManagedSimplePropertyViewModel(this, context, simple);
                            prop.PropertyChanged += HandleSimplePropertyChanged;
                            properties.Add(prop);
                            break;
                        }
                    case ManagedCollectionProperty collection:
                        {
                            var prop = new ManagedCollectionPropertyViewModel(this, context, collection);
                            properties.Add(prop);
                            break;
                        }
                    case ManagedReferenceProperty reference:
                        {
                            var prop = new ManagedReferencePropertyViewModel(this, context, reference);
                            properties.Add(prop);
                            break;
                        }
                    default:
                        throw new InvalidOperationException("Unsupported managed property type!");
                }
            }

            int i = 0;
            while (i < properties.Count && string.Compare(properties[i].Name, Animator.Designer.Resources.Controls.DocumentControl.Strings.Macros) < 0)
                i++;

            properties.Insert(i, macros);

            // Content property

            var contentPropertyAttribute = type.GetCustomAttribute<ContentPropertyAttribute>(true);
            if (contentPropertyAttribute != null)
            {
                contentProperty = properties.OfType<ManagedPropertyViewModel>().Single(prop => prop.Name == contentPropertyAttribute.PropertyName);
                contentProperty.PropertyChanged += HandleContentPropertyValueChanged;
                contentProperty.CollectionChanged += HandleContentPropertyCollectionChanged;
            }
            else
                contentProperty = null;

            // Name property

            var current = type;

            while (current != typeof(ManagedObject) && !namePropDefinitions.ContainsKey(current))
                current = current.BaseType;

            if (current != typeof(ManagedObject) && current != null)
            {
                var namePropDefinition = namePropDefinitions[current];

                string nameNamespace = GetNamespace(namePropDefinition.Namespace);

                var property = properties.OfType<ManagedSimplePropertyViewModel>().FirstOrDefault(prop => prop.Name == namePropDefinition.Property && prop.Namespace == nameNamespace);
                if (property != null)
                {
                    nameProperty = property;
                    nameProperty.StringValueChanged += HandleNameChanged;
                }

                NameColor = namePropDefinition.Color;
            }

            // Value property

            current = type;

            while (current != typeof(ManagedObject) && !valuePropDefinitions.ContainsKey(current))
                current = current.BaseType;

            if (current != typeof(ManagedObject) && current != null)
            {
                var valuePropDefinition = valuePropDefinitions[current];

                string valueNamespace = GetNamespace(valuePropDefinition.Namespace);

                var property = properties.OfType<ManagedSimplePropertyViewModel>().FirstOrDefault(prop => prop.Name == valuePropDefinition.Property && prop.Namespace == valueNamespace);
                if (property != null)
                {
                    valueProperty = property;
                    valueProperty.StringValueChanged += HandleValueChanged;
                }
            }

            // Property proxies

            propertiesProxy = BuildPropertiesProxy();

            // Commands

            var notRootCondition = Condition.Lambda(this, vm => vm.Parent != null, false);
            DeleteCommand = new AppCommand(obj => DoDelete(), notRootCondition);            
        }

        // Public properties --------------------------------------------------

        public MacroCollectionPropertyViewModel Macros => macros;

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public ManagedPropertyViewModel ContentProperty => contentProperty;

        public string ClassName { get; }

        public string Name => (nameProperty?.Value as StringValueViewModel)?.Value;

        public string Value => TruncateValue((valueProperty?.Value as StringValueViewModel)?.Value);

        public string NameColor { get; }

        public string Namespace { get; }

        public override IEnumerable<BaseObjectViewModel> DisplayChildren => GetDisplayChildren();

        public ICommand DeleteCommand { get; }

        // Transported from the content property

        public ICommand AddInstanceCommand => contentProperty?.AddInstanceCommand;

        public ICommand SetToInstanceCommand => contentProperty?.SetToInstanceCommand;

        public ICommand InsertMacroCommand => contentProperty?.InsertMacroCommand;

        public ICommand SetToMarkupExtensionCommand => contentProperty?.SetToMarkupExtensionCommand;

        public IEnumerable<TypeViewModel> AvailableTypes => contentProperty?.AvailableTypes;

        public IEnumerable<TypeViewModel> AvailableMarkupExtensions => contentProperty?.AvailableMarkupExtensions;
    }
}
