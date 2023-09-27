using Animator.Designer.BusinessLogic.Models.MovieSerialization;
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
using Animator.Designer.BusinessLogic.Helpers;

namespace Animator.Designer.BusinessLogic.Infrastructure
{
    public class MovieSerializer : BaseManagedObjectSerializer
    {
        // Private classes ----------------------------------------------------

        private sealed class DeserializationContext
        {
            public DeserializationContext(Models.MovieSerialization.DeserializationOptions options)
            {
                Options = options;
            }

            public Dictionary<string, NamespaceDefinition> Namespaces { get; } = new();
            public Models.MovieSerialization.DeserializationOptions Options { get; }
        }

        // Private types ------------------------------------------------------

        private static readonly HashSet<Type> staticallyInitializedTypes = new();

        // Private methods ----------------------------------------------------

        private static void StaticInitializeRecursively(Type type)
        {
            do
            {
                // If type is initialized, its base types must have been initialized too,
                // don't waste time on them
                if (staticallyInitializedTypes.Contains(type))
                    return;

                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

                staticallyInitializedTypes.Add(type);

                type = type.BaseType;
            }
            while (type != typeof(ManagedObject) && type != typeof(object));
        }

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

                            BaseObjectViewModel macroContent = DeserializeElement(macroNode, context);

                            var macroItem = new MacroEntryViewModel(context.Namespaces[string.Empty].ToString(), ENGINE_NAMESPACE, ENGINE_NAMESPACE);
                            macroItem.Property<StringPropertyViewModel>(ENGINE_NAMESPACE, "Key").Value = xKey;
                            macroItem.Content = macroContent;

                            deserializedObject.Macros.Add(macroItem);
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
                        var value = new ReferenceValueViewModel();
                        value.Value = content;

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
                    var referenceValue = new ReferenceValueViewModel();
                    referenceValue.Value = content;

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

            string defaultNamespace = context.Namespaces[string.Empty].ToString();

            var markupExt = new MarkupExtensionViewModel(defaultNamespace, ns, markupData.Name, markupData.TypeData.Type);

            foreach (var param in markupData.Params)
            {
                markupExt[defaultNamespace, param.property].Value = param.value;
            }

            if (managedProperty is ManagedSimplePropertyViewModel simple)
            {
                simple.Value = markupExt;
            }
            else if (managedProperty is ManagedReferencePropertyViewModel reference)
            {
                reference.Value = markupExt;
            }
            else if (managedProperty is ManagedCollectionPropertyViewModel collection)
            {
                collection.Value = markupExt;
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
                        deserializedObject.Property<StringPropertyViewModel>(ENGINE_NAMESPACE, KEY_ATTRIBUTE).Value = attribute.Value;
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

        private BaseObjectViewModel DeserializeElement(XmlNode node, DeserializationContext context)
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

                    var includeViewModel = new IncludeViewModel(context.Namespaces[string.Empty].ToString(), ENGINE_NAMESPACE, ENGINE_NAMESPACE);
                    includeViewModel.Property<StringPropertyViewModel>(SOURCE_ATTRIBUTE).Value = filename;

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

                    var macroViewModel = new MacroViewModel(context.Namespaces[string.Empty].ToString(), ENGINE_NAMESPACE, ENGINE_NAMESPACE);
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

                    var generateViewModel = new GenerateViewModel(context.Namespaces[string.Empty].ToString(), ENGINE_NAMESPACE, ENGINE_NAMESPACE);
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

                StaticInitializeRecursively(objectTypeData.Type);

                var ns = objectTypeData.Type.ToNamespaceDefinition().ToString();

                deserializedObject = new ManagedObjectViewModel(context.Namespaces[string.Empty].ToString(), ENGINE_NAMESPACE, ns, node.Name, objectTypeData.Type);

                // 2. Load attributes

                var setProperties = new HashSet<string>();

                DeserializeAttributes(node, deserializedObject, context, setProperties);

                // 3. Load children

                DeserializeChildren(node, deserializedObject, context, setProperties);

                // 4. Return deserialized object

                return deserializedObject;
            }
        }

        private BaseObjectViewModel InternalDeserialize(XmlDocument document, string documentPath, Models.MovieSerialization.DeserializationOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.DefaultNamespace == null)
                throw new ArgumentException("Default namespace is null!", nameof(options));

            var context = new DeserializationContext(options);
            context.Namespaces[string.Empty] = options.DefaultNamespace;

            return DeserializeElement(document.ChildNodes.OfType<XmlElement>().Single(), context);
        }

        private Models.MovieSerialization.DeserializationOptions CreateDefaultDeserializationOptions()
        {
            return new Models.MovieSerialization.DeserializationOptions
            {
                DefaultNamespace = typeof(Animator.Engine.Elements.Movie).ToNamespaceDefinition()
            };
        }

        // Public methods -----------------------------------------------------

        public BaseObjectViewModel Deserialize(string filename)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(filename);

            string documentPath = Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(documentPath))
                documentPath = Directory.GetCurrentDirectory();

            return InternalDeserialize(document, documentPath, CreateDefaultDeserializationOptions());
        }

        public BaseObjectViewModel Deserialize(XmlDocument document, string documentPath)
        {
            return InternalDeserialize(document, documentPath, CreateDefaultDeserializationOptions());
        }
    }
}
