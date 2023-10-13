using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Base.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System.Collections;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using System.Reflection;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Engine.Base.Extensions;

namespace Animator.Designer.BusinessLogic.Infrastructure
{
    public class MovieSerializer : BaseManagedObjectSerializer
    {
        // Private classes ----------------------------------------------------

        private sealed class DeserializationContext
        {
            public DeserializationContext(NamespaceDefinition defaultNamespace,
                WrapperContext wrapperContext)
            {
                WrapperContext = wrapperContext;
                Namespaces[string.Empty] = defaultNamespace;
            }

            public Dictionary<string, NamespaceDefinition> Namespaces { get; } = new();
            public WrapperContext WrapperContext { get; }

            public NamespaceDefinition DefaultNamespace => Namespaces[string.Empty];
        }

        private sealed class NamespaceContext
        {
            public HashSet<string> FreeNamespaces { get; } = new();
            public HashSet<(string prefix, string @namespace)> PrefixedNamespaces { get; } = new();
        } 

        // Private methods ----------------------------------------------------

        private void DeserializeChildren(XmlNode node, 
            ManagedObjectViewModel deserializedObject, 
            DeserializationContext context, 
            HashSet<string> propertiesSet)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child is XmlComment)
                    continue;

                // 0. Check if it is any of internal elements

                if (child.NamespaceURI == ENGINE_NAMESPACE)
                {
                    if (child.LocalName == MACROS_ELEMENT)
                    {
                        foreach (var macroNode in child.ChildNodes.OfType<XmlElement>())
                        {
                            var keyAttribute = macroNode.Attributes
                                .OfType<XmlAttribute>()
                                .Where(a => a.NamespaceURI == ENGINE_NAMESPACE && a.LocalName == KEY_ATTRIBUTE)
                                .FirstOrDefault();

                            if (keyAttribute == null)
                                throw new SerializerException("Macro is missing its key!", macroNode.FindXPath());

                            var xKey = keyAttribute.Value;

                            ObjectViewModel macroContent = DeserializeElement(macroNode, context);

                            var macroItem = new MacroDefinitionViewModel(context.WrapperContext);
                            macroItem.Property<StringPropertyViewModel>(ENGINE_NAMESPACE, "Key").Value = xKey;
                            macroItem.Property<ReferencePropertyViewModel>(context.DefaultNamespace.ToString(), "Content").Value = new ReferenceValueViewModel(macroContent);

                            deserializedObject.Macros.Value.Items.Add(macroItem);
                        }

                        // This doesn't need immediate processing
                        continue;
                    }
                    else if (child.LocalName == MACRO_ELEMENT)
                    {
                        // This one will be processed later, no action required
                    }
                    else if (child.LocalName == INCLUDE_ELEMENT)
                    {
                        // This one will be processed later, no action required
                    }
                    else if (child.LocalName == GENERATE_ELEMENT)
                    {
                        // This one will be processed later, no action required
                    }
                    else
                        throw new SerializerException($"Not recognized internal element: {child.LocalName}!", child.FindXPath());
                }

                // 1. Check if it is a property with extended notation

                if (child.NamespaceURI == node.NamespaceURI &&
                    child.LocalName.StartsWith($"{node.LocalName}."))
                {
                    // 1.1.1 Loading property

                    string propertyName = child.LocalName.Substring(node.LocalName.Length + 1);

                    var property = deserializedObject.Property<ManagedPropertyViewModel>(propertyName);
                    
                    if (property == null)
                        throw new SerializerException($"Property {propertyName} not found on object of type {deserializedObject.GetType().Name}",
                            node.FindXPath());

                    if (property.ManagedProperty.Metadata.NotSerializable)
                        throw new SerializerException($"Property {propertyName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.",
                            node.FindXPath());

                    DeserializeObjectProperty(child, deserializedObject, property, context, propertiesSet);
                }
                else
                {
                    // 1.2.1 Find, which property is set as a so-called content property

                    var property = deserializedObject.ContentProperty;

                    if (property == null)
                        throw new SerializerException($"Type {deserializedObject.GetType().Name} does not have ContentProperty specified. You have to specify property explicitly.",
                            node.FindXPath());

                    if (property.ManagedProperty.Metadata.NotSerializable)
                        throw new SerializerException($"Property {property.ManagedProperty.Name} on object {deserializedObject.GetType().Name} is not serializable!\r\nThe data structure is ill-formed: content property should be serializable.",
                            node.FindXPath());

                    // 1.2.2 Deserialize object

                    var content = DeserializeElement(child, context);

                    if (propertiesSet.Contains(property.Name))
                        throw new SerializerException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}",
                            node.FindXPath());

                    if (property is ManagedSimplePropertyViewModel)
                    {
                        throw new SerializerException($"Property {property.Name} is a simple managed property and cannot be content property of object {deserializedObject.GetType().Name}",
                            node.FindXPath());
                    }
                    else if (property is ManagedReferencePropertyViewModel referenceProperty)
                    {
                        var value = new ReferenceValueViewModel(content);

                        referenceProperty.Value = value;
                        propertiesSet.Add(string.Format(CONTENT_DECORATION, property.Name));
                    }
                    else if (property is ManagedCollectionPropertyViewModel collectionProperty)
                    {
                        var collectionValue = collectionProperty.Value as CollectionValueViewModel;
                        if (collectionValue == null)
                        {
                            collectionValue = new CollectionValueViewModel();
                            collectionProperty.Value = collectionValue;
                        }

                        collectionValue.Items.Add(content);
                        propertiesSet.Add(string.Format(CONTENT_DECORATION, property.Name));
                    }
                    else
                        throw new InvalidOperationException("Unsupported managed property!");
                }
            }
        }

        private void DeserializeObjectProperty(XmlNode propertyNode, 
            ManagedObjectViewModel deserializedObject, 
            ManagedPropertyViewModel property, 
            DeserializationContext context, 
            HashSet<string> propertiesSet)
        {
            if (property.ManagedProperty.Metadata.NotSerializable)
                throw new SerializerException($"Cannot deserialize non-serializable property {property.Name}!",
                    propertyNode.FindXPath());

            if (propertiesSet.Contains(property.Name) || propertiesSet.Contains(string.Format(CONTENT_DECORATION, property.Name)))
                throw new SerializerException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}",
                    propertyNode.FindXPath());

            if (property is ManagedSimplePropertyViewModel simple)
            {
                if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 0)
                {
                    var stringValue = new StringValueViewModel(propertyNode.InnerText);
                    simple.Value = stringValue;

                    propertiesSet.Add(property.Name);
                }
                else if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 1)
                {
                    throw new SerializerException($"Cannot set managed object to a simple property {property.Name} of object {deserializedObject.GetType().Name}",
                        propertyNode.FindXPath());
                }
                else
                    throw new SerializerException($"Property {property.Name} on type {deserializedObject.GetType().Name} is a simple property, but is provided with multiple values.",
                        propertyNode.FindXPath());
            }
            if (property is ManagedReferencePropertyViewModel reference)
            {
                if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 0)
                {
                    var stringValue = new StringValueViewModel(propertyNode.InnerText);
                    reference.Value = stringValue;
                    
                    propertiesSet.Add(property.Name);
                }
                else if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 1)
                {
                    var content = DeserializeElement(propertyNode.ChildNodes.OfType<XmlElement>().Single(), context);
                    var referenceValue = new ReferenceValueViewModel(content);

                    reference.Value = referenceValue;

                    propertiesSet.Add(property.Name);
                }
                else
                    throw new SerializerException($"Property {property.Name} on type {deserializedObject.GetType().Name} is a simple property, but is provided with multiple values.",
                        propertyNode.FindXPath());
            }
            else if (property is ManagedCollectionPropertyViewModel collectionProperty)
            {
                if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 0)
                {
                    var value = new StringValueViewModel(propertyNode.InnerText);
                    collectionProperty.Value = value;
                }
                else
                {
                    var collectionValue = collectionProperty.Value as CollectionValueViewModel;
                    if (collectionValue == null)
                    {
                        collectionValue = new CollectionValueViewModel();
                        collectionProperty.Value = collectionValue;
                    }

                    foreach (XmlNode child in propertyNode.ChildNodes.OfType<XmlElement>())
                    {
                        var content = DeserializeElement(child, context);
                        collectionValue.Items.Add(content);
                    }

                    propertiesSet.Add(property.Name);
                }
            }
            else
                throw new InvalidOperationException("Unsupported managed property!");
        }

        private void ProcessMarkupExtension(ManagedObjectViewModel deserializedObject, 
            ManagedPropertyViewModel managedProperty, 
            string value, 
            XmlNode node, 
            DeserializationContext context)
        {
            var markupData = DeserializeMarkupExtension(value, node, context.Namespaces);

            var ns = markupData.TypeData.Type.ToNamespaceDefinition().ToString();

            string defaultNamespace = context.DefaultNamespace.ToString();

            var markupExt = new MarkupExtensionViewModel(context.WrapperContext, ns, markupData.Name, markupData.TypeData.Type);

            foreach (var param in markupData.Params)
            {
                markupExt.Property<ClearableStringPropertyViewModel>(defaultNamespace, param.property).Value = new StringValueViewModel(param.value);
            }

            if (managedProperty is ManagedSimplePropertyViewModel simple)
            {
                simple.Value = new MarkupExtensionValueViewModel(markupExt);
            }
            else if (managedProperty is ManagedReferencePropertyViewModel reference)
            {
                reference.Value = new MarkupExtensionValueViewModel(markupExt);
            }
            else if (managedProperty is ManagedCollectionPropertyViewModel collection)
            {
                collection.Value = new MarkupExtensionValueViewModel(markupExt);
            }
            else
                throw new InvalidOperationException("Unsupported property type!");
        }
    
        private void DeserializeAttributes(XmlNode node, 
            ManagedObjectViewModel deserializedObject, 
            DeserializationContext context, 
            HashSet<string> propertiesSet)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                // 1. Omit xmlns definitions, for some reason they are
                // being treated as regular attributes too

                if (attribute.Name == "xmlns" || attribute.Name.StartsWith("xmlns:"))
                    continue;

                // 2. Omit internal attributes

                if (attribute.NamespaceURI == ENGINE_NAMESPACE)
                {
                    if (attribute.LocalName == KEY_ATTRIBUTE)
                    {
                        // deserializedObject.Property<StringPropertyViewModel>(ENGINE_NAMESPACE, KEY_ATTRIBUTE).Value = attribute.Value;
                        continue;
                    }
                    else
                        throw new SerializerException($"Not recognized internal attribute: {attribute.LocalName}", node.FindXPath());
                }

                // 3. Check if property hasn't been already set

                if (propertiesSet.Contains(attribute.LocalName))
                    throw new SerializerException($"Property {attribute.LocalName} has been already set on type {deserializedObject.GetType().Name}",
                        node.FindXPath());

                // 4. Find property with appropriate name

                ManagedPropertyViewModel managedProperty = deserializedObject.Property<ManagedPropertyViewModel>(attribute.LocalName);

                if (managedProperty == null)
                    throw new SerializerException($"Property {attribute.LocalName} not found on object of type {deserializedObject.GetType().Name}",
                        node.FindXPath());
                if (managedProperty.ManagedProperty.Metadata.NotSerializable)
                    throw new SerializerException($"Property {attribute.LocalName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.",
                        node.FindXPath());

                // 5. Check for markup extension

                if (attribute.Value.StartsWith(MARKUP_EXTENSION_BEGIN) &&
                    !attribute.Value.StartsWith(MARKUP_EXTENSION_ESCAPED_BEGIN) &&
                    attribute.Value.EndsWith(MARKUP_EXTENSION_END))
                {
                    ProcessMarkupExtension(deserializedObject, managedProperty, attribute.Value, node, context);
                }
                else
                {
                    if (managedProperty is ManagedSimplePropertyViewModel simpleProperty)
                    {
                        simpleProperty.Value = new StringValueViewModel(attribute.Value);
                        propertiesSet.Add(attribute.LocalName);
                    }
                    else if (managedProperty is ManagedCollectionPropertyViewModel collectionProperty)
                    {
                        // This means that collection value is stored as attribute.
                        // In case of Designer this doesn't matter much - we simply
                        // set the StringValue to the property
                        
                        collectionProperty.Value = new StringValueViewModel(attribute.Value);
                        propertiesSet.Add(attribute.LocalName);
                    }
                    else if (managedProperty is ManagedReferencePropertyViewModel referenceProperty)
                    {
                        // This means that reference value is stored as attribute.
                        // In case of Designer this doesn't matter much - we simply
                        // set the StringValue to the property

                        referenceProperty.Value = new StringValueViewModel(attribute.Value);
                        propertiesSet.Add(attribute.LocalName);
                    }
                    else
                        throw new InvalidOperationException("Unsupported property type!");
                }
            }
        }

        private ObjectViewModel DeserializeElement(XmlNode node, DeserializationContext context)
        {
            // 1. Check control nodes

            if (string.Equals(node.NamespaceURI, ENGINE_NAMESPACE))
            {
                if (node.LocalName == INCLUDE_ELEMENT)
                {
                    var sourceAttribute = node.Attributes
                        .OfType<XmlAttribute>()
                        .FirstOrDefault(a => a.NamespaceURI == ENGINE_NAMESPACE && a.LocalName == SOURCE_ATTRIBUTE);

                    if (sourceAttribute == null)
                        throw new SerializerException("Include element must contain attribute x:Source!", node.FindXPath());

                    string filename = sourceAttribute.Value;

                    var includeViewModel = new IncludeViewModel(context.WrapperContext);
                    includeViewModel.Property<StringPropertyViewModel>(ENGINE_NAMESPACE, SOURCE_ATTRIBUTE).Value = filename;

                    return includeViewModel;
                }
                else if (node.LocalName == MACRO_ELEMENT)
                {
                    var keyAttribute = node.Attributes
                        .OfType<XmlAttribute>()
                        .FirstOrDefault(a => a.NamespaceURI == ENGINE_NAMESPACE && a.LocalName == KEY_ATTRIBUTE);

                    if (keyAttribute == null)
                        throw new SerializerException("Macro element misses Key attribute!", node.FindXPath());

                    var key = keyAttribute.Value;

                    if (node.ChildNodes.Count > 0)
                        throw new SerializerException("Macro may not contain any child elements!", node.FindXPath());

                    var macroViewModel = new MacroViewModel(context.WrapperContext);
                    macroViewModel.Property<StringPropertyViewModel>(ENGINE_NAMESPACE, KEY_ATTRIBUTE).Value = key;

                    foreach (var attribute in node.Attributes
                        .OfType<XmlAttribute>()
                        .Where(a => a.NamespaceURI != ENGINE_NAMESPACE || a.LocalName != KEY_ATTRIBUTE))
                    {
                        string name = attribute.LocalName;
                        string value = attribute.Value;

                        var prop = macroViewModel.AddProperty(name);
                        prop.Value = value;
                    }

                    return macroViewModel;
                }
                else if (node.LocalName == GENERATE_ELEMENT)
                {
                    if (node.ChildNodes.Count != 1)
                        throw new SerializerException("Generate element must contain exactly one child!", node.FindXPath());

                    var child = (XmlElement)node.ChildNodes[0];

                    var generateViewModel = new GenerateViewModel(context.WrapperContext);
                    generateViewModel.Property<MultilineStringPropertyViewModel>("Generator").Value = child.OuterXml;

                    return generateViewModel;
                }
                else
                {
                    throw new SerializerException($"Not recognized internal node: {node.LocalName}", node.FindXPath());
                }
            }
            else
            {
                // 1. Deserialize class

                ManagedObjectViewModel deserializedObject;

                var objectTypeData = ExtractObjectType(node.NamespaceURI, node.LocalName, typeof(ManagedObject), context.Namespaces);

                // We have to ensure that static ctor runs. It will not run by itself
                // if we deal with the type on the reflection level (aka the Type)

                var ns = objectTypeData.Type.ToNamespaceDefinition().ToString();

                deserializedObject = new ManagedObjectViewModel(context.WrapperContext, ns, node.Name, objectTypeData.Type);

                // 2. Load attributes

                var setProperties = new HashSet<string>();

                DeserializeAttributes(node, deserializedObject, context, setProperties);

                // 3. Load children

                DeserializeChildren(node, deserializedObject, context, setProperties);

                // 4. Return deserialized object

                return deserializedObject;
            }
        }

        private void CollectNamespacesRecursively(XmlElement xmlNode, NamespaceContext context)
        {
            if (!string.IsNullOrEmpty(xmlNode.NamespaceURI))
            {
                context.FreeNamespaces.Add(xmlNode.NamespaceURI);
            }

            foreach (XmlAttribute attribute in xmlNode.Attributes)
            {
                if (attribute.Name.StartsWith("xmlns:"))
                {
                    string prefix = attribute.Name[6..];
                    string @namespace = attribute.Value;

                    context.PrefixedNamespaces.Add((prefix, @namespace));
                }
            }

            foreach (XmlElement child in xmlNode.ChildNodes.OfType<XmlElement>())
                CollectNamespacesRecursively(child, context);
        }

        private void BuildNamespaces(XmlDocument document, WrapperContext wrapperContext)
        {
            var namespaceContext = new NamespaceContext();
            CollectNamespacesRecursively(document.ChildNodes.OfType<XmlElement>().First() as XmlElement, namespaceContext);

            // First, add all prefixed namespaces

            foreach (var ns in namespaceContext.PrefixedNamespaces)
            {
                if (ns.@namespace == ENGINE_NAMESPACE)
                {
                    // Special case only for engine namespace

                    if (!wrapperContext.Namespaces.Any(n => n.NamespaceUri == ns.@namespace))
                        wrapperContext.AddNamespace(new NamespaceViewModel(ns.prefix, ns.@namespace));
                }
                else if (ns.@namespace == wrapperContext.DefaultNamespace)
                {
                    // We will add default namespace later
                    continue;
                }
                else
                {
                    var definition = ParseNamespaceDefinition(ns.@namespace);
                    Assembly assembly = EnsureAssembly(definition);

                    wrapperContext.AddNamespace(new AssemblyNamespaceViewModel(ns.prefix, ns.@namespace, assembly, definition.Namespace));
                }
            }

            // Next, add artificial prefixes for all namespaces defined explicitly (if any)

            foreach (var ns in namespaceContext.FreeNamespaces)
            {
                // We will add default namespace later
                if (ns == wrapperContext.DefaultNamespace)
                    continue;

                if (!wrapperContext.Namespaces.Any(n => n.NamespaceUri == ns))
                {
                    var definition = ParseNamespaceDefinition(ns);
                    Assembly assembly = EnsureAssembly(definition);

                    int i = 1;

                    while (wrapperContext.Namespaces.Any(n => n.Prefix == $"n{i}"))
                        i++;

                    wrapperContext.AddNamespace(new AssemblyNamespaceViewModel($"n{i}", ns, assembly, definition.Namespace));
                }
            }

            // Add default namespace

            var defaultDefinition = typeof(Animator.Engine.Elements.Movie).ToNamespaceDefinition();
            var defaultAssembly = typeof(Animator.Engine.Elements.Movie).Assembly;

            wrapperContext.AddNamespace(new AssemblyNamespaceViewModel(null, defaultDefinition.ToString(), defaultAssembly, defaultDefinition.Namespace));

            // Make sure that engine namespace is defined

            if (!wrapperContext.Namespaces.Any(ns => ns.NamespaceUri == ENGINE_NAMESPACE))
            {
                wrapperContext.AddNamespace(new NamespaceViewModel("x", ENGINE_NAMESPACE));
            }            
        }

        private ObjectViewModel InternalDeserialize(XmlDocument document, WrapperContext wrapperContext)
        {
            var context = new DeserializationContext(typeof(Animator.Engine.Elements.Movie).ToNamespaceDefinition(), wrapperContext);

            var resultObject = DeserializeElement(document.ChildNodes.OfType<XmlElement>().Single(), context);
            BuildNamespaces(document, wrapperContext);

            return resultObject;
        }

        private (ObjectViewModel Object, WrapperContext context) InternalDeserialize(XmlDocument document)
        {
            var defaultNamespace = typeof(Animator.Engine.Elements.Movie).ToNamespaceDefinition();

            var wrapperContext = new WrapperContext(ENGINE_NAMESPACE, defaultNamespace.ToString());
            
            return (InternalDeserialize(document, wrapperContext), wrapperContext);
        }


        // Public methods -----------------------------------------------------

        public (ObjectViewModel Object, WrapperContext Context) Deserialize(string filename)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(filename);

            return InternalDeserialize(document);
        }

        public (ObjectViewModel Object, WrapperContext Context) Deserialize(XmlDocument document)
        {
            return InternalDeserialize(document);
        }

        public ObjectViewModel Deserialize(XmlDocument document, WrapperContext wrapperContext)
        {
            return InternalDeserialize(document, wrapperContext);
        }
    }
}
