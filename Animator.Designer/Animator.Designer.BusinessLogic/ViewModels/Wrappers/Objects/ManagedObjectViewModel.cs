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
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class ManagedObjectViewModel : ObjectViewModel
    {
        // Private types ------------------------------------------------------

        private enum NamespaceType
        {
            Default = 1,
            Engine
        }

        // Private constants --------------------------------------------------

        private const int MAX_VALUE_LENGTH = 64;

        private static readonly Dictionary<(NamespaceType Namespace, string Name), string> icons = new()
        {
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Image)), "Image16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Layer)), "Layer16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Rectangle)), "Rectangle16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Circle)), "Circle16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Ellipse)), "Ellipse16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Line)), "Line16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.SvgImage)), "SvgImage16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Label)), "Label16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Path)), "Path16.png" },

            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateInt)), "AnimateInt16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateBool)), "AnimateBool16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateFloat)), "AnimateFloat16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimatePoint)), "AnimatePoint16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateColor)), "AnimateColor16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateStopwatch)), "AnimateStopwatch16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateWithExpression)), "AnimateWithExpression16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Storyboard)), "Storyboard16.png" },

        };

        private static readonly Dictionary<Type, (NamespaceType Namespace, string Property, string Color)> namePropDefinitions = new()
        {
            { typeof(Animator.Engine.Elements.SceneElement), (NamespaceType.Default, nameof(Animator.Engine.Elements.SceneElement.Name), "#808080") },
            { typeof(Animator.Engine.Elements.Resource), (NamespaceType.Default, nameof(Animator.Engine.Elements.Resource.Key), "#0000ff") },
            { typeof(Animator.Engine.Elements.AnimateProperty), (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateProperty.PropertyRef), "#ff8000") },
            { typeof(Animator.Engine.Elements.For), (NamespaceType.Default, nameof(Animator.Engine.Elements.For.PropertyRef), "#ff8000") }
        };

        private static readonly Dictionary<Type, (NamespaceType Namespace, string Property)> valuePropDefinitions = new()
        {
            { typeof(Animator.Engine.Elements.Label), (NamespaceType.Default, nameof(Animator.Engine.Elements.Label.Text)) },
            { typeof(Animator.Engine.Elements.Image), (NamespaceType.Default, nameof(Animator.Engine.Elements.Image.Source)) },
            { typeof(Animator.Engine.Elements.SvgImage), (NamespaceType.Default, nameof(Animator.Engine.Elements.SvgImage.Source)) },
        };

        // Private fields -----------------------------------------------------

        private readonly ManagedPropertyViewModel contentProperty;
        private readonly MacroCollectionPropertyViewModel macros;
        private readonly ManagedSimplePropertyViewModel nameProperty;
        private readonly ObservableCollection<PropertyViewModel> properties = new();
        private readonly ManagedSimplePropertyViewModel valueProperty;
        private bool canMoveDown = false;
        private bool canMoveUp = false;
        private PropertiesProxyViewModel propertiesProxy;

        // Private methods ----------------------------------------------------

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

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        private void DoMoveDown()
        {
            Parent.RequestMoveDown(this);
        }

        private void DoMoveUp()
        {
            Parent.RequestMoveUp(this);
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

        private string GetNamespace(NamespaceType ns)
        {
            return ns switch
            {
                NamespaceType.Default => context.DefaultNamespace,
                NamespaceType.Engine => context.EngineNamespace,
                _ => throw new InvalidEnumArgumentException("Unsupported namespace type!")
            };
        }

        private void HandleContentPropertyCollectionChanged(object sender, EventArgs e)
        {
            NotifyDisplayChildrenChanged();
        }

        private void HandleContentPropertyValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ManagedPropertyViewModel.Value))
                NotifyDisplayChildrenChanged();
        }

        private void HandleNameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(DisplayName));
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

        private void HandleValueChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Value));
        }

        private void NotifyDisplayChildrenChanged()
        {
            OnPropertyChanged(nameof(DisplayChildren));
        }

        private string TruncateValue(string value)
        {
            if (value == null)
                return null;

            if (value.Length > MAX_VALUE_LENGTH)
                value = $"{value[..61]}...";

            return value;
        }

        // Public methods -----------------------------------------------------

        public ManagedObjectViewModel(WrapperContext context, string ns, string className, Type type)
            : base(context)
        {
            macros = new MacroCollectionPropertyViewModel(this, context, context.EngineNamespace, "Macros");

            this.Name = className;
            this.Namespace = ns;
            this.Type = type;

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
            var canMoveUpCondition = Condition.PropertyWatch(this, vm => vm.CanMoveUp, false);
            var canMoveDownCondition = Condition.PropertyWatch(this, vm => vm.CanMoveDown, false);
           
            DeleteCommand = new AppCommand(obj => DoDelete(), notRootCondition);
            MoveUpCommand = new AppCommand(obj => DoMoveUp(), canMoveUpCondition);
            MoveDownCommand = new AppCommand(obj => DoMoveDown(), canMoveDownCondition);

            // Icon

            NamespaceType namespaceType = (NamespaceType)0;
            if (Namespace == context.DefaultNamespace)
                namespaceType = NamespaceType.Default;
            else if (Namespace == context.EngineNamespace)
                namespaceType = NamespaceType.Engine;

            if (icons.TryGetValue((namespaceType, Name), out string icon))
                Icon = icon;
        }

        public override XmlElement Serialize(XmlDocument document)
        {
            // Prepare

            var engineNsDef = context.Namespaces.First(ns => ns.NamespaceUri == context.EngineNamespace);
            var objectNsDef = context.Namespaces.First(ns => ns.NamespaceUri == Namespace);

            // Create node

            XmlElement result = CreateRootElement(document);

            // Macros

            if (macros.Value.Items.Any())
            {
                var macrosNode = document.CreateElement(engineNsDef.Prefix, "Macros", engineNsDef.NamespaceUri);
                result.AppendChild(macrosNode);

                foreach (var macro in macros.Value.Items.OfType<MacroDefinitionViewModel>())
                {
                    XmlElement macroNode = macro.Serialize(document);
                    macrosNode.AppendChild(macroNode);
                }
            }

            // Non-content properties

            bool contentPropertyProcessed = false;

            foreach (var property in properties)
            {
                if (property is ManagedPropertyViewModel managedProperty)
                {
                    if (managedProperty == contentProperty)
                    {
                        if (managedProperty.Value is DefaultValueViewModel or ReferenceValueViewModel or CollectionValueViewModel)
                        {
                            continue;
                        }

                        contentPropertyProcessed = true;
                    }


                    switch (managedProperty.Value)
                    {
                        case DefaultValueViewModel:
                            continue;
                        case StringValueViewModel stringValue:
                            {
                                XmlAttribute propAttr = CreateAttributeProp(document, managedProperty);

                                propAttr.Value = stringValue.Value;
                                result.Attributes.Append(propAttr);
                                break;
                            }
                        case ReferenceValueViewModel refValue:
                            {
                                XmlElement propElement = CreateNestedProperty(document, objectNsDef, managedProperty);

                                XmlElement refValueElement = refValue.Value.Serialize(document);
                                propElement.AppendChild(refValueElement);

                                result.AppendChild(propElement);
                                break;
                            }
                        case CollectionValueViewModel collectionValue:
                            {
                                if (!collectionValue.Items.Any())
                                    continue;

                                XmlElement propElement = CreateNestedProperty(document, objectNsDef, managedProperty);

                                foreach (var item in collectionValue.Items)
                                {
                                    XmlElement itemElement = item.Serialize(document);
                                    propElement.AppendChild(itemElement);
                                }

                                result.AppendChild(propElement);
                                break;
                            }
                        case MarkupExtensionValueViewModel markupExtension:
                            {
                                XmlAttribute propAttr = CreateAttributeProp(document, managedProperty);
                                propAttr.Value = markupExtension.Value.SerializeToString();

                                result.Attributes.Append(propAttr);
                                break;
                            }                        
                        default:
                            throw new InvalidOperationException($"Unsupported managed property value: {managedProperty.Value}");
                    }
                }
                else if (property == macros)
                {
                    // Macros were processed before all other properties
                    continue;
                }
                else
                {
                    throw new InvalidOperationException("Invalid property in ManagedObjectViewModel's property list!");
                }
            }

            // Content property

            if (contentProperty != null && !contentPropertyProcessed)
            {
                switch (contentProperty.Value)
                {
                    case DefaultValueViewModel:
                        break;
                    case ReferenceValueViewModel refValue:
                        {
                            var refElement = refValue.Value.Serialize(document);
                            result.AppendChild(refElement);

                            break;
                        }
                    case CollectionValueViewModel collectionValue:
                        {
                            if (collectionValue.Items.Any())
                            {
                                foreach (var item in collectionValue.Items)
                                {
                                    var itemElement = item.Serialize(document);
                                    result.AppendChild(itemElement);
                                }
                            }
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Unsupported content property value: {contentProperty.Value}!");
                }
            }

            return result;
        }

        // Public properties --------------------------------------------------

        public ICommand AddInstanceCommand => contentProperty?.AddInstanceCommand;

        public IEnumerable<TypeViewModel> AvailableMarkupExtensions => contentProperty?.AvailableMarkupExtensions;

        public IEnumerable<TypeViewModel> AvailableTypes => contentProperty?.AvailableTypes;

        public bool CanMoveDown
        {
            get => canMoveDown;
            set => Set(ref canMoveDown, value);
        }

        public bool CanMoveUp
        {
            get => canMoveUp;
            set => Set(ref canMoveUp, value);
        }

        public ManagedPropertyViewModel ContentProperty => contentProperty;

        public ICommand DeleteCommand { get; }

        public override IEnumerable<BaseObjectViewModel> DisplayChildren => GetDisplayChildren();

        public string DisplayName => (nameProperty?.Value as StringValueViewModel)?.Value;

        public ICommand InsertGeneratorCommand => contentProperty?.InsertGeneratorCommand;

        public ICommand InsertIncludeCommand => contentProperty?.InsertIncludeCommand;

        public ICommand InsertMacroCommand => contentProperty?.InsertMacroCommand;

        public MacroCollectionPropertyViewModel Macros => macros;

        public string NameColor { get; }

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        // Transported from the content property
        public ICommand SetToInstanceCommand => contentProperty?.SetToInstanceCommand;

        public ICommand SetToMarkupExtensionCommand => contentProperty?.SetToMarkupExtensionCommand;

        public ICommand SetToSpecificMacroCommand => contentProperty?.SetToSpecificMacroCommand;

        public ICommand AddSpecificMacroCommand => contentProperty?.AddSpecificMacroCommand;

        public IEnumerable<MacroKeyViewModel> AvailableMacros => contentProperty?.AvailableMacros;

        public Type Type { get; }

        public string Value => TruncateValue((valueProperty?.Value as StringValueViewModel)?.Value);
    }
}
