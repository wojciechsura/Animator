using Animator.Engine.Base;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Base.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Base.Persistence
{
    public class ManagedObjectSerializer
    {
        // Private constants --------------------------------------------------

        private const string EngineNamespace = "https://spooksoft.pl/animator";

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
        }

        // Private constants --------------------------------------------------

        private const string contentDecoration = "content:{0}";

        // Private methods ----------------------------------------------------

        /// <summary>
        /// Parses namespace from string into instance of NamespaceDefinition
        /// </summary>        
        /// <remarks>
        /// Namespace must be in format: <code>assembly=A.B.C;namespace=X.Y.Z</code>,
        /// whitespace- and case-sensitive.
        /// </remarks>
        private NamespaceDefinition ParseNamespaceDefinition(string namespaceDefinition)
        {
            string[] elements = namespaceDefinition.Split(';');

            if (elements.Length != 2)
                throw new ParseException($"Invalid namespace definition: {namespaceDefinition}\r\nThere should be exactly two sections separated with semicolon (;).");

            // Assembly

            if (!elements[0].StartsWith("assembly=") || elements[0].Length < 10)
                throw new ParseException($"Invalid namespace definition: {namespaceDefinition}\r\nMissing or invalid assembly section!");

            string assembly = elements[0].Substring(9);

            // Namespace

            if (!elements[1].StartsWith("namespace=") || elements[1].Length < 11)
                throw new ParseException($"Invalid namespace definition: {namespaceDefinition}\r\nMissing or invalid namespace section!");

            string @namespace = elements[1][10..];

            return new NamespaceDefinition(assembly, @namespace);
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

                    if (property is ManagedSimpleProperty simpleProperty)
                    {
                        deserializedObject.SetValue(simpleProperty, content);
                        propertiesSet.Add(string.Format(contentDecoration, property.Name));
                    }
                    else if (property is ManagedReferenceProperty referenceProperty)
                    {
                        deserializedObject.SetValue(referenceProperty, content);
                        propertiesSet.Add(string.Format(contentDecoration, property.Name));
                    }
                    else if (property is ManagedCollectionProperty collectionProperty)
                    {
                        ManagedCollection collection = (ManagedCollection)deserializedObject.GetValue(property);
                        ((IList)collection).Add(content);

                        propertiesSet.Add(string.Format(contentDecoration, property.Name));
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

            if (propertiesSet.Contains(property.Name) || propertiesSet.Contains(string.Format(contentDecoration, property.Name)))
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

                // 2. Check if property hasn't been already set

                if (propertiesSet.Contains(attribute.LocalName))
                    throw new SerializerException($"Property {attribute.LocalName} has been already set on type {deserializedObject.GetType().Name}",
                        node.FindXPath());

                // 3. Find property with appropriate name

                ManagedProperty managedProperty = deserializedObject.GetProperty(attribute.LocalName);

                if (managedProperty == null)
                    throw new SerializerException($"Property {attribute.LocalName} not found on object of type {deserializedObject.GetType().Name}",
                        node.FindXPath());
                if (managedProperty.Metadata.NotSerializable)
                    throw new SerializerException($"Property {attribute.LocalName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.",
                        node.FindXPath());

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

        private ManagedObject DeserializeElement(XmlNode node, DeserializationContext context)
        {
            // 1. Check control nodes

            if (string.Equals(node.NamespaceURI, EngineNamespace))
            {
                if (node.LocalName == "Include")
                {
                    var sourceAttribute = node.Attributes["Source"];
                    if (sourceAttribute == null)
                        throw new SerializerException("Include element must contain attribute Source!", node.FindXPath());

                    string filename = node.Attributes["Source"].Value;
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
                else
                {
                    throw new SerializerException($"Not recognized internal node: {node.LocalName}", node.FindXPath());
                }
            }
            else
            {
                // 1. Figure out, which object to instantiate

                string className = node.LocalName;

                if (!context.Namespaces.TryGetValue(node.NamespaceURI, out NamespaceDefinition namespaceDefinition))
                {
                    namespaceDefinition = ParseNamespaceDefinition(node.NamespaceURI);
                    context.Namespaces[node.NamespaceURI] = namespaceDefinition;
                }

                // 2. Get type from the specified assembly + namespace + class name

                var assembly = AppDomain.CurrentDomain.GetAssemblies().
                    SingleOrDefault(assembly => assembly.GetName().Name == namespaceDefinition.Assembly);

                if (assembly == null)
                    try
                    {
                        assembly = Assembly.Load(namespaceDefinition.Assembly);
                    }
                    catch (FileNotFoundException)
                    {
                        // Intentionally left empty, will leave assembly as null.
                    }

                if (assembly == null)
                    throw new SerializerException($"Cannot access assembly {namespaceDefinition.Assembly}\r\nMake sure, it is loaded or accessible to load.",
                        node.FindXPath());

                string fullClassTypeName = string.Join('.', namespaceDefinition.Namespace, className);
                Type objType = assembly.GetType(fullClassTypeName, false);

                if (objType == null)
                    throw new SerializerException($"Cannot find type {fullClassTypeName} in assembly {namespaceDefinition.Assembly}",
                        node.FindXPath());

                // 3. Make sure, that object derives from ManagedObject - only those are supported

                if (!objType.IsAssignableTo(typeof(ManagedObject)))
                    throw new SerializerException($"Type {fullClassTypeName} does not derive from ManagedObject!",
                        node.FindXPath());

                // 4. Try to instantiate object

                ManagedObject deserializedObject;

                try
                {
                    if (context.CustomActivator != null)
                        deserializedObject = (ManagedObject)context.CustomActivator.CreateInstance(objType);
                    else
                        deserializedObject = (ManagedObject)Activator.CreateInstance(objType);
                }
                catch (Exception e)
                {
                    throw new SerializerException($"Cannot instantiate type {fullClassTypeName}. Check inner exception for details.",
                        node.FindXPath(),
                        e);
                }

                // 5. Load attributes

                var setProperties = new HashSet<string>();

                DeserializeAttributes(node, deserializedObject, context, setProperties);

                // 6. Load children

                DeserializeChildren(node, deserializedObject, context, setProperties);

                // 7. Return deserialized object

                return deserializedObject;
            }
        }

        private ManagedObject InternalDeserialize(XmlDocument document, DeserializationOptions options, string documentPath)
        {
            var context = new DeserializationContext(options?.CustomActivator, options?.CustomSerializers, documentPath, options);

            if (options != null)
            {
                if (options.DefaultNamespace != null)
                {
                    context.Namespaces[string.Empty] = options.DefaultNamespace;
                }
            }

            return DeserializeElement(document.ChildNodes.OfType<XmlElement>().Single(), context);
        }

        // Public methods -----------------------------------------------------

        public ManagedObject Deserialize(string filename, DeserializationOptions options = null)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filename);

            string documentPath = System.IO.Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(documentPath))
                documentPath = Directory.GetCurrentDirectory();

            return InternalDeserialize(document, options, documentPath);
        }

        public ManagedObject Deserialize(XmlDocument document, DeserializationOptions options = null)
        {            
            return InternalDeserialize(document, options, Directory.GetCurrentDirectory());
        }
    }
}
