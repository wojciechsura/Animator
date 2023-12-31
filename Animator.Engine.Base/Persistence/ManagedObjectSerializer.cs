﻿using Animator.Engine.Base.Exceptions;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Base.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Base.Persistence
{
    public class ManagedObjectSerializer : BaseManagedObjectSerializer
    {
        // Private types ------------------------------------------------------

        private class DeserializationContext
        {
            public DeserializationContext(CustomActivator customActivator,
                Dictionary<Type, TypeSerializer> customTypeSerializers,
                string documentPath,
                DeserializationOptions originalOptions)
            {
                CustomActivator = customActivator;
                CustomTypeSerializers = customTypeSerializers;
                DocumentPath = documentPath;
                OriginalOptions = originalOptions;
            }

            public Dictionary<string, NamespaceDefinition> Namespaces { get; } = new();
            public HashSet<Type> StaticallyInitializedTypes { get; } = new();
            public CustomActivator CustomActivator { get; }
            public Dictionary<Type, TypeSerializer> CustomTypeSerializers { get; }
            public string DocumentPath { get; }
            public DeserializationOptions OriginalOptions { get; }
            public List<PendingMarkupExtension> PendingMarkupExtensions { get; } = new();
        }

        // Private methods ----------------------------------------------------

        /// <summary>
        /// Deserializes markup extension and adds it to context's PendingMarkupExtensions
        /// </summary>
        private void ProcessMarkupExtension(ManagedObject deserializedObject,
            ManagedProperty managedProperty,
            string value,
            XmlNode node,
            DeserializationContext context)
        {
            // 1. Deserialize data related to markup extension

            var markupData = DeserializeMarkupExtension(value, node, context.Namespaces);

            // 2. Instantiate object

            object extObj = CreateObject(context, markupData.TypeData);

            if (extObj is not BaseMarkupExtension)
                throw new SerializerException($"Markup extensions must derive from BaseMarkupExtension!", node.FindXPath());

            BaseMarkupExtension extension = extObj as BaseMarkupExtension;

            // 3. Enter property values

            foreach (var param in markupData.Params)
            {
                PropertyInfo info = extension.GetType().GetProperty(param.property);
                if (info == null)
                    throw new SerializerException($"Markup extension {markupData.Name} does not have property {param.property}!", node.FindXPath());

                object propertyValue;

                if (context.CustomTypeSerializers != null &&
                    context.CustomTypeSerializers.TryGetValue(info.PropertyType, out TypeSerializer customSerializer) &&
                    customSerializer.CanDeserialize(param.value))
                {
                    propertyValue = customSerializer.Deserialize(param.value);
                }
                else if (TypeSerialization.CanDeserialize(param.value, info.PropertyType))
                {
                    propertyValue = TypeSerialization.Deserialize(param.value, info.PropertyType);
                }
                else
                    throw new InvalidCastException($"Cannot deserialize value {param.value} to type {info.PropertyType.Name}!");

                info.SetValue(extension, propertyValue);
            }

            context.PendingMarkupExtensions.Add(new PendingMarkupExtension(extension, managedProperty, deserializedObject));
        }

        /// <summary>
        /// Deserializes child nodes of given node. Those may be
        /// either properties stored in extended form, values
        /// of content property, or contents of collection (if
        /// deserialized node is one)
        /// </summary>
        private void DeserializeChildren(XmlNode node,
            ManagedObject deserializedObject,
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

                    var property = deserializedObject.GetProperty(propertyName);
                    if (property == null)
                        throw new SerializerException($"Property {propertyName} not found on object of type {deserializedObject.GetType().Name}",
                            node.FindXPath());
                    if (property.Metadata.NotSerializable)
                        throw new SerializerException($"Property {propertyName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.",
                            node.FindXPath());

                    DeserializeObjectProperty(child, deserializedObject, property, context, propertiesSet);
                }
                else
                {
                    // 1.2.1 Find, which property is set as a so-called content property

                    var contentPropertyAttribute = deserializedObject.GetType().GetCustomAttribute<ContentPropertyAttribute>(true);
                    if (contentPropertyAttribute == null)
                        throw new SerializerException($"Type {deserializedObject.GetType().Name} does not have ContentProperty specified. You have to specify property explicitly.",
                            node.FindXPath());

                    var property = deserializedObject.GetProperty(contentPropertyAttribute.PropertyName);
                    if (property == null)
                        throw new SerializerException($"Managed property {contentPropertyAttribute.PropertyName} specified as ContentProperty on type {deserializedObject.GetType().Name} does not exist!",
                            node.FindXPath());
                    if (property.Metadata.NotSerializable)
                        throw new SerializerException($"Property {contentPropertyAttribute.PropertyName} on object {deserializedObject.GetType().Name} is not serializable!\r\nThe data structure is ill-formed: content property should be serializable.",
                            node.FindXPath());

                    // 1.2.2 Deserialize object

                    var content = DeserializeElement(child, context);

                    if (propertiesSet.Contains(property.Name))
                        throw new SerializerException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}",
                            node.FindXPath());

                    if (property is ManagedSimpleProperty)
                    {
                        throw new SerializerException($"Property {property.Name} is a simple managed property and cannot be content property of object {deserializedObject.GetType().Name}",
                            node.FindXPath());
                    }
                    else if (property is ManagedReferenceProperty referenceProperty)
                    {
                        deserializedObject.SetValue(referenceProperty, content);
                        propertiesSet.Add(string.Format(CONTENT_DECORATION, property.Name));
                    }
                    else if (property is ManagedCollectionProperty collectionProperty)
                    {
                        ManagedCollection collection = (ManagedCollection)deserializedObject.GetValue(property);
                        ((IList)collection).Add(content);

                        propertiesSet.Add(string.Format(CONTENT_DECORATION, property.Name));
                    }
                    else
                        throw new InvalidOperationException("Unsupported managed property!");
                }
            }
        }

        /// <summary>
        /// Deserializes property node (eg. for &lt;Element&gt;, a property
        /// node would be an &lt;Element.Property&gt;) into specific
        /// deserialized object's property.
        /// </summary>
        private void DeserializeObjectProperty(XmlNode propertyNode,
            ManagedObject deserializedObject,
            ManagedProperty property,
            DeserializationContext context,
            HashSet<string> propertiesSet)
        {
            if (property.Metadata.NotSerializable)
                throw new SerializerException($"Cannot deserialize non-serializable property {property.Name}!",
                    propertyNode.FindXPath());

            if (propertiesSet.Contains(property.Name) || propertiesSet.Contains(string.Format(CONTENT_DECORATION, property.Name)))
                throw new SerializerException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}",
                    propertyNode.FindXPath());

            // TODO get rid of duplicated code

            if (property is ManagedValueProperty valueProperty)
            {
                if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 0)
                {
                    var content = DeserializePropertyValue(context, valueProperty, propertyNode.InnerText);
                    deserializedObject.SetValue(property, content);

                    propertiesSet.Add(property.Name);
                }
                else if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 1)
                {
                    var content = DeserializeElement(propertyNode.ChildNodes.OfType<XmlElement>().Single(), context);
                    deserializedObject.SetValue(property, content);

                    propertiesSet.Add(property.Name);
                }
                else
                    throw new SerializerException($"Property {property.Name} on type {deserializedObject.GetType().Name} is a simple property, but is provided with multiple values.",
                        propertyNode.FindXPath());
            }
            else if (property is ManagedCollectionProperty collectionProperty)
            {
                if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 0)
                {
                    var value = DeserializeCollectionPropertyValue(context, collectionProperty, propertyNode.InnerText);

                    if (value != null)
                    {
                        var collection = (ManagedCollection)deserializedObject.GetValue(collectionProperty);

                        foreach (object obj in value)
                            ((IList)collection).Add(obj);

                        propertiesSet.Add(propertyNode.LocalName);
                    }
                    else
                        throw new SerializerException($"Property {propertyNode.LocalName} of {deserializedObject.GetType().Name} is a collection property, but its value is stored in attribute and no custom serializer is provided!",
                            propertyNode.FindXPath());
                }
                else
                {
                    var collection = (ManagedCollection)deserializedObject.GetValue(property);

                    foreach (XmlNode child in propertyNode.ChildNodes.OfType<XmlElement>())
                    {
                        var content = DeserializeElement(child, context);
                        ((IList)collection).Add(content);
                    }

                    propertiesSet.Add(property.Name);
                }
            }
            else
                throw new InvalidOperationException("Unsupported managed property!");
        }

        /// <summary>
        /// Deserialize attributes of given node into object's properties
        /// </summary>
        private void DeserializeAttributes(XmlNode node,
            ManagedObject deserializedObject,
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
                        // This attribute may be safely ignored.
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

                ManagedProperty managedProperty = deserializedObject.GetProperty(attribute.LocalName);

                if (managedProperty == null)
                    throw new SerializerException($"Property {attribute.LocalName} not found on object of type {deserializedObject.GetType().Name}",
                        node.FindXPath());
                if (managedProperty.Metadata.NotSerializable)
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
                    // TODO merge duplicated code

                    if (managedProperty is ManagedValueProperty valueProperty)
                    {
                        string propertyValue = attribute.Value;
                        object value;

                        try
                        {
                            value = DeserializePropertyValue(context, valueProperty, attribute.Value);
                        }
                        catch (Exception e)
                        {
                            throw new SerializerException($"Failed to deserialize attribute {attribute.LocalName}", attribute.FindXPath(), e);
                        }

                        deserializedObject.SetValue(valueProperty, value);
                        propertiesSet.Add(attribute.LocalName);
                    }
                    else if (managedProperty is ManagedCollectionProperty collectionProperty)
                    {
                        IList value = DeserializeCollectionPropertyValue(context, collectionProperty, attribute.Value);

                        if (value != null)
                        {
                            var collection = (ManagedCollection)deserializedObject.GetValue(collectionProperty);

                            foreach (object obj in value)
                                ((IList)collection).Add(obj);

                            propertiesSet.Add(attribute.LocalName);
                        }
                        else
                            throw new SerializerException($"Property {attribute.LocalName} of {deserializedObject.GetType().Name} is a collection property, but its value is stored in attribute and no custom serializer is provided!",
                                node.FindXPath());
                    }
                    else
                        throw new InvalidOperationException("Unsupported property type!");
                }
            }
        }

        private object DeserializePropertyValue(DeserializationContext context, ManagedValueProperty valueProperty, string propertyValue)
        {
            if (valueProperty.Metadata.CustomSerializer != null && valueProperty.Metadata.CustomSerializer.CanDeserialize(propertyValue))
            {
                return valueProperty.Metadata.CustomSerializer.Deserialize(propertyValue);
            }
            else if (context.CustomTypeSerializers != null && context.CustomTypeSerializers.TryGetValue(valueProperty.Type, out TypeSerializer customSerializer) && customSerializer.CanDeserialize(propertyValue))
            {
                return customSerializer.Deserialize(propertyValue);
            }
            else if (TypeSerialization.CanDeserialize(propertyValue, valueProperty.Type))
            {
                return TypeSerialization.Deserialize(propertyValue, valueProperty.Type);
            }
            else
                throw new InvalidCastException($"Cannot deserialize value {propertyValue} to type {valueProperty.Type.Name}!");
        }

        private static IList DeserializeCollectionPropertyValue(DeserializationContext context, ManagedCollectionProperty collectionProperty, string propertyValue)
        {
            if (collectionProperty.Metadata.CustomSerializer != null && collectionProperty.Metadata.CustomSerializer.CanDeserialize(propertyValue))
                return (IList)collectionProperty.Metadata.CustomSerializer.Deserialize(propertyValue);
            if (context.CustomTypeSerializers != null && context.CustomTypeSerializers.TryGetValue(collectionProperty.Type, out TypeSerializer customSerializer) && customSerializer.CanDeserialize(propertyValue))
                return (IList)customSerializer.Deserialize(propertyValue);

            return null;
        }

        private object CreateObject(DeserializationContext context, ObjectTypeData typeData)
        {
            // Try to instantiate object

            object deserializedObject;

            try
            {
                if (context.CustomActivator != null)
                    deserializedObject = context.CustomActivator.CreateInstance(typeData.Type);
                else
                    deserializedObject = Activator.CreateInstance(typeData.Type);
            }
            catch (Exception e)
            {
                throw new ActivatorException($"Cannot instantiate type {typeData.FullClassName}. Check inner exception for details.", e);
            }

            return deserializedObject;
        }

        private object Instantiate(string namespaceUri, string className, Type requiredBaseType, DeserializationContext context)
        {
            var typeData = ExtractObjectType(namespaceUri, className, requiredBaseType, context.Namespaces);
            return CreateObject(context, typeData);
        }

        private ManagedObject FindMacro(XmlNode child, string key, DeserializationContext context)
        {
            XmlElement foundMacro = null;

            var node = child;
            while (node != null)
            {
                var macroChild = node.ChildNodes
                    .OfType<XmlElement>()
                    .Where(c => c.NamespaceURI == ENGINE_NAMESPACE && c.LocalName == MACROS_ELEMENT)
                    .FirstOrDefault();

                if (macroChild != null)
                {
                    List<XmlElement> macro = macroChild.ChildNodes
                        .OfType<XmlElement>()
                        .Where(m => m.Attributes
                            .OfType<XmlAttribute>()
                            .Where(a => a.NamespaceURI == ENGINE_NAMESPACE && a.LocalName == KEY_ATTRIBUTE && a.Value == key)
                            .FirstOrDefault() != null)
                        .ToList();

                    if (macro.Count == 1)
                    {
                        foundMacro = macro[0];
                        break;
                    }
                    else if (macro.Count > 1)
                    {
                        throw new SerializerException($"There are multiple macros on a node, which match the same key {key}!", node.FindXPath());
                    }
                }

                node = node.ParentNode;
                continue;
            }

            if (foundMacro == null)
                throw new SerializerException($"Cannot find macro with key {key}! Make sure, that it is reachable and key is not misspelled (keys are case-sensitive)", child.FindXPath());

            ManagedObject result = DeserializeElement(foundMacro, context);
            return result;
        }

        private ManagedObject DeserializeElement(XmlNode node, DeserializationContext context)
        {
            // 1. Check control nodes

            if (string.Equals(node.NamespaceURI, ENGINE_NAMESPACE))
            {
                if (node.LocalName == INCLUDE_ELEMENT)
                {
                    var sourceAttribute = node.Attributes
                            .OfType<XmlAttribute>()
                            .Where(a => a.NamespaceURI == ENGINE_NAMESPACE && a.LocalName == SOURCE_ATTRIBUTE)
                            .FirstOrDefault();

                    if (sourceAttribute == null)
                        throw new SerializerException("Include element must contain attribute x:Source!", node.FindXPath());

                    string filename = sourceAttribute.Value;
                    if (!Path.IsPathRooted(filename))
                        filename = Path.Combine(context.DocumentPath, filename);

                    // Deserialize separate file and return object from inside

                    ManagedObject includedObject = null;

                    try
                    {
                        includedObject = Deserialize(filename, context.OriginalOptions);
                    }
                    catch (Exception e)
                    {
                        throw new SerializerException($"Cannot include file {filename}!", node.FindXPath(), e);
                    }

                    return includedObject;
                }
                else if (node.LocalName == MACRO_ELEMENT)
                {
                    var keyAttribute = node.Attributes
                        .OfType<XmlAttribute>()
                        .Where(a => a.NamespaceURI == ENGINE_NAMESPACE && a.LocalName == KEY_ATTRIBUTE)
                        .FirstOrDefault();

                    if (keyAttribute == null)
                        throw new SerializerException("Macro element misses Key attribute!", node.FindXPath());

                    var key = keyAttribute.Value;

                    if (node.ChildNodes.Count > 0)
                        throw new SerializerException("Macro may not contain any child elements!", node.FindXPath());

                    ManagedObject objectFromMacro = FindMacro(node, key, context);

                    // Using new HashSet here on purpose.
                    // This allows overriding attributes, which are set in macro.
                    // eg.
                    // <x:Macro x:Key="SomeMacro" Name="Test" Position="20,30" />
                    DeserializeAttributes(node, objectFromMacro, context, new HashSet<string>());

                    return objectFromMacro;
                }
                else if (node.LocalName == GENERATE_ELEMENT)
                {
                    if (node.ChildNodes.Count != 1)
                        throw new SerializerException("Generate element must contain exactly one child!", node.FindXPath());

                    var child = (XmlElement)node.ChildNodes[0];

                    BaseGenerator generator;

                    try
                    {
                        generator = (BaseGenerator)Instantiate(child.NamespaceURI, child.LocalName, typeof(BaseGenerator), context);
                    }
                    catch (ActivatorException e)
                    {
                        throw new SerializerException($"Failed to instantiate generator {child.LocalName} from namespace {child.NamespaceURI}.", node.FindXPath(), e);
                    }

                    var obj = generator.Generate(child);

                    if (obj == null)
                        throw new SerializerException("Generator failed to provide any object!", node.FindXPath());

                    return obj;
                }
                else
                {
                    throw new SerializerException($"Not recognized internal node: {node.LocalName}", node.FindXPath());
                }
            }
            else
            {
                // 1. Deserialize class

                ManagedObject deserializedObject;

                try
                {
                    object obj = Instantiate(node.NamespaceURI, node.LocalName, typeof(ManagedObject), context);

                    if (obj is not ManagedObject)
                        throw new ActivatorException("Element classes must derive from ManagedObject!");

                    deserializedObject = (ManagedObject)obj;
                }
                catch (Exception e)
                {
                    throw new SerializerException("Cannot instantiate class.", node.FindXPath(), e);
                }

                // 2. Load attributes

                var setProperties = new HashSet<string>();

                DeserializeAttributes(node, deserializedObject, context, setProperties);

                // 3. Load children

                DeserializeChildren(node, deserializedObject, context, setProperties);

                // 4. Return deserialized object

                return deserializedObject;
            }
        }

        private ManagedObject InternalDeserialize(XmlDocument document, DeserializationOptions options, string documentPath)
        {
            var context = new DeserializationContext(options?.CustomActivator, options?.CustomSerializers, documentPath, options);

            if (options != null && options.DefaultNamespace != null)
            {
                context.Namespaces[string.Empty] = options.DefaultNamespace;
            }

            var result = DeserializeElement(document.ChildNodes.OfType<XmlElement>().Single(), context);

            // Run pending markup extensions
            foreach (var extension in context.PendingMarkupExtensions)
            {
                extension.MarkupExtension.ProvideValue(extension.Object, extension.Property);
            }

            return result;
        }

        // Public methods -----------------------------------------------------

        public ManagedObject Deserialize(string filename, DeserializationOptions options = null)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filename);

            string documentPath = Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(documentPath))
                documentPath = Directory.GetCurrentDirectory();

            return InternalDeserialize(document, options, documentPath);
        }

        public ManagedObject Deserialize(XmlDocument document, DeserializationOptions options = null, string searchPath = null)
        {
            // Sanitize path
            return InternalDeserialize(document, options, searchPath ?? Directory.GetCurrentDirectory());
        }
    }
}
