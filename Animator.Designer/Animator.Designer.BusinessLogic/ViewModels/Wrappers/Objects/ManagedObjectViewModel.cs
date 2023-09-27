﻿using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class ManagedObjectViewModel : BaseObjectViewModel
    {
        private enum NamespaceType
        {
            Default,
            Engine
        }

        private static readonly Dictionary<Type, (NamespaceType Namespace, string Property, string Color)> namePropDefinitions = new()
        {
            { typeof(Animator.Engine.Elements.SceneElement), (NamespaceType.Default, nameof(Animator.Engine.Elements.SceneElement.Name), "#000000") },
            { typeof(Animator.Engine.Elements.Resource), (NamespaceType.Default, nameof(Animator.Engine.Elements.Resource.Key), "#0000ff") },
            { typeof(Animator.Engine.Elements.AnimateProperty), (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateProperty.PropertyRef), "#ff8000") }
        };

        private readonly ObservableCollection<PropertyViewModel> properties = new();
        private readonly ObservableCollection<MacroEntryViewModel> macros = new();
        private readonly ManagedPropertyViewModel contentProperty;
        private readonly ManagedSimplePropertyViewModel nameProperty;

        private void HandleNameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Name));
        }

        private void NotifyDisplayChildrenChanged()
        {
            OnPropertyChanged(nameof(DisplayChildren));
        }

        private IEnumerable<BaseObjectViewModel> GetDisplayChildren()
        {
            // Collect all reference or collection properies,
            // which currently have reference or collection value

            List<ManagedPropertyViewModel> proxyProperties = new();

            foreach (var property in properties)
            {
                if (property == contentProperty)
                    continue;

                if (property is ManagedReferencePropertyViewModel reference && reference.Value is ReferenceValueViewModel)
                    proxyProperties.Add(reference);
                else if (property is ManagedCollectionPropertyViewModel collection && collection.Value is CollectionValueViewModel)
                    proxyProperties.Add(collection);
            }

            if (proxyProperties.Any())
                yield return new PropertyProxyViewModel(defaultNamespace, engineNamespace, proxyProperties);

            if (contentProperty != null)
            {
                if (contentProperty is ManagedReferencePropertyViewModel reference)
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

        public ManagedObjectViewModel(string defaultNamespace, string engineNamespace, string ns, string className, Type type)
            : base(defaultNamespace, engineNamespace)
        {
            this.ClassName = className;
            this.Namespace = ns;

            foreach (var property in ManagedProperty.FindAllByType(type, true))
            {
                if (property.Metadata.NotSerializable)
                    continue;

                switch (property)
                {
                    case ManagedSimpleProperty simple:
                        {
                            var prop = new ManagedSimplePropertyViewModel(defaultNamespace, simple);
                            properties.Add(prop);
                            break;
                        }
                    case ManagedCollectionProperty collection:
                        {
                            var prop = new ManagedCollectionPropertyViewModel(defaultNamespace, collection);
                            properties.Add(prop);
                            break;
                        }
                    case ManagedReferenceProperty reference:
                        {
                            var prop = new ManagedReferencePropertyViewModel(defaultNamespace, reference);
                            properties.Add(prop);
                            break;
                        }
                    default:
                        throw new InvalidOperationException("Unsupported managed property type!");
                }
            }

            var keyProperty = new StringPropertyViewModel(engineNamespace, "Key");
            properties.Add(keyProperty);

            // Content property

            var contentPropertyAttribute = type.GetCustomAttribute<ContentPropertyAttribute>(true);
            if (contentPropertyAttribute != null)
            {
                contentProperty = properties.OfType<ManagedPropertyViewModel>().Single(prop => prop.Name == contentPropertyAttribute.PropertyName);
                contentProperty.ReferenceValueChanged += (s, e) => NotifyDisplayChildrenChanged();
                contentProperty.CollectionChanged += (s, e) => NotifyDisplayChildrenChanged();
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

                string nameNamespace = namePropDefinition.Namespace switch
                {
                    NamespaceType.Default => defaultNamespace,
                    NamespaceType.Engine => engineNamespace,
                    _ => throw new InvalidEnumArgumentException("Unsupported namespace type!")
                };

                var property = properties.OfType<ManagedSimplePropertyViewModel>().FirstOrDefault(prop => prop.Name == namePropDefinition.Property && prop.Namespace == nameNamespace);
                if (property != null)
                {
                    nameProperty = property;
                    nameProperty.StringValueChanged += HandleNameChanged;
                }

                NameColor = namePropDefinition.Color;
            }
        }

        public IList<MacroEntryViewModel> Macros => macros;

        public override IReadOnlyList<PropertyViewModel> Properties => properties;

        public ManagedPropertyViewModel ContentProperty => contentProperty;

        public string ClassName { get; }

        public string Name => (nameProperty?.Value as StringValueViewModel)?.Value;

        public string NameColor { get; }

        public string Namespace { get; }

        public IEnumerable<BaseObjectViewModel> DisplayChildren => GetDisplayChildren();
    }
}
